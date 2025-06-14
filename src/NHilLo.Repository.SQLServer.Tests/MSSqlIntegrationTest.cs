using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Testcontainers.MsSql;
using System.Runtime.InteropServices;
using Renci.SshNet.Security;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace NHiLo.Tests.Integration.HiLo.Repository.MSSql
{
    [Collection("Database Integration")]
    public class MSSqlIntegrationTest
    {
        private readonly ITestOutputHelper _output;

        public MSSqlIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ConnectToABrandNewDatabaseAndGetKey_When_UsingTable()
        {
            string entityName = "myMSSqlTableEntity";
            string funcAppSettings(string connectionString) => $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{connectionString}"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}";

            long validateNextHi(SqlCommand cmd)
            {
                cmd.CommandText = $"SELECT * FROM NHILO WHERE ENTITY = '{entityName}'";
                using var reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetInt64(reader.GetOrdinal("NEXT_HI"));
            }

            await TestDatabase(funcAppSettings, validateNextHi, entityName);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ConnectToABrandNewDatabaseAndGetKey_When_UsingSequence()
        {
            string entityName = "myMSSqlSequenceEntity";
            string funcAppSettings(string connectionString) => $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100"",
                        ""StorageType"" : ""Sequence""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{connectionString}"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}";

            long validateNextHi(SqlCommand cmd)
            {
                cmd.CommandText = $"SELECT current_value FROM sys.sequences WHERE name = 'SQ_HiLo_{entityName}'";
                using var reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetInt64(0) + 1; // +1 because Sequence works different from the table implementation
            }

            await TestDatabase(funcAppSettings, validateNextHi, entityName);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ThrowExceptionInvalidEntityName_When_UsingSequenceWithAnInvalidPrefix()
        {
            string entityName = "myMSSqlSequenceEntity2";
            string funcAppSettings(string connectionString) => $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100"",
                        ""StorageType"" : ""Sequence"",
                        ""ObjectPrefix"": ""'""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{connectionString}"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}";

            try
            {
                await TestDatabase(funcAppSettings, cmd => 0, entityName);
                Assert.Fail("This test must throw an exception");
            }
            catch (NHiLoException ex)
            {
                ex.ErrorCode.Should().Be(ErrorCodes.ErrorWhilePreparingRepository);
                ex.InnerException.As<NHiLoException>().ErrorCode.Should().Be(ErrorCodes.InvalidEntityName);
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ThrowExceptionEntityNameValidationTimedOut_When_UsingAnInvalidPrefixThatExceedsTheRegexValidationTimeout()
        {
            string entityName = "myMSSqlSequenceEntity1";
            string complexPrefix = new string('a', 99999) + "!";
            string funcAppSettings(string connectionString) => $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100"",
                        ""StorageType"" : ""Sequence"",
                        ""ObjectPrefix"": ""{complexPrefix}"",
                        ""EntityNameValidationTimeout"": ""1""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{connectionString}"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}";

            try
            {
                await TestDatabase(funcAppSettings, cmd => 0, entityName);
                Assert.Fail("This test must throw an exception");
            }
            catch (NHiLoException ex)
            {
                ex.ErrorCode.Should().Be(ErrorCodes.ErrorWhilePreparingRepository);
                ex.InnerException.As<NHiLoException>().ErrorCode.Should().Be(ErrorCodes.EntityNameValidationTimedOut);
            }
        }

        private MsSqlBuilder CreateImagemBuilder()
        {
            var testcontainersBuilder = new MsSqlBuilder()
                .WithPortBinding(1433, true)
                .WithPassword("myP@ssword100");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // FIX https://blog.rufer.be/2024/09/22/workaround-fix-testcontainers-sql-error-docker-dotnet-dockerapiexception-docker-api-responded-with-status-codeconflict/
                testcontainersBuilder = testcontainersBuilder.WithImage("mcr.microsoft.com/mssql/server:2022-latest");
            }
            return testcontainersBuilder;
        }

        private async Task TestDatabase(Func<string, string> funcAppSettings, Func<SqlCommand, long> validateNextHi, string entityName)
        {
            var testcontainersBuilder = CreateImagemBuilder();
            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(funcAppSettings(testcontainer.GetConnectionString()))));

            var factory = new HiLoGeneratorFactory(builder.Build());
            var generator = factory.GetKeyGenerator(entityName);
            long key = generator.GetKey();
            _output.WriteLine($"Key generated: '{key}'");
            key.Should().BeGreaterThan(0, "is expected the key to be greater than 0.");

            await using var connection = new SqlConnection(testcontainer.GetConnectionString());
            connection.Open();
            await using var cmd = new SqlCommand();
            cmd.Connection = connection;
            long nexttHi = validateNextHi(cmd);
            _output.WriteLine($"Next Hi value: '{nexttHi}'");
            nexttHi.Should().Be(2, "is expected the next Hi value to be equal to 2 (first execution).");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_RaiseError_When_LackOfSELECTPermission()
        {
            var testcontainersBuilder = CreateImagemBuilder();

            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();

            string prepareDbScript = @"
                sp_configure 'show advanced options',1
                GO
                RECONFIGURE WITH OVERRIDE
                GO
                sp_configure 'contained database authentication', 1
                GO
                RECONFIGURE WITH OVERRIDE
                GO

                CREATE DATABASE [testDB] CONTAINMENT = PARTIAL;
                GO

                CREATE LOGIN [nhilo_user] WITH PASSWORD = 'nhilo_p@ssW0rd';
                GO

                USE [testDB];
                GO

                CREATE USER [nhilo_user] FOR LOGIN [nhilo_user];
                GO

                CREATE TABLE [dbo].[NHILO](
                [ENTITY] [nvarchar](100) NOT NULL,
                [NEXT_HI] [bigint] NOT NULL,
                    CONSTRAINT [PK_NHILO] PRIMARY KEY CLUSTERED 
                    (
                        [ENTITY] ASC
                    )
                );
                GO

                DENY SELECT ON [dbo].[NHILO] TO [nhilo_user];
                GO
            ";
            await testcontainer.ExecScriptAsync(prepareDbScript);

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(
                $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""Server={testcontainer.Hostname},{testcontainer.GetMappedPublicPort(1433)};Database=testDB;User Id=nhilo_user;Password=nhilo_p@ssW0rd;TrustServerCertificate=True"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}
            "
                )));
            var factory = new HiLoGeneratorFactory(builder.Build());
            Action act = () => factory.GetKeyGenerator("MSSqlSequenceEntity");

            act.Should().Throw<NHiLoException>()
                .WithInnerException<SqlException>()
                .Where(ex => ex.Number == 229, "229 is the SQL Server's error code for lack of select permission");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_RaiseError_When_LackOfCreateTablePermission()
        {
            var testcontainersBuilder = CreateImagemBuilder();

            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();

            string prepareDbScript = @"
                sp_configure 'show advanced options',1
                GO
                RECONFIGURE WITH OVERRIDE
                GO
                sp_configure 'contained database authentication', 1
                GO
                RECONFIGURE WITH OVERRIDE
                GO

                CREATE DATABASE [testDB] CONTAINMENT = PARTIAL;
                GO

                CREATE LOGIN [nhilo_user] WITH PASSWORD = 'nhilo_p@ssW0rd';
                GO

                USE [testDB];
                GO

                CREATE USER [nhilo_user] FOR LOGIN [nhilo_user];
                GO
            ";
            await testcontainer.ExecScriptAsync(prepareDbScript);

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(
                $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""Server={testcontainer.Hostname},{testcontainer.GetMappedPublicPort(1433)};Database=testDB;User Id=nhilo_user;Password=nhilo_p@ssW0rd;TrustServerCertificate=True"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}"
                )));
            var factory = new HiLoGeneratorFactory(builder.Build());
            Action act = () => factory.GetKeyGenerator("MSSqlSequenceEntity");

            act.Should().Throw<NHiLoException>()
                .WithInnerException<SqlException>()
                .Where(ex => ex.Number == 262, "262 is the SQL Server's error code for lack of create table permission");
        }
    }
}