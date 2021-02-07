using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NHiLo.Tests.Integration.Repository.MSSql
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
        public async void ShouldConnectToABrandNewDatabaseAndGetKeyUsingTable()
        {
            string entityName = "myMSSqlTableEntity";
            string funcAppSettings(string connectionString) => $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{ connectionString }"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}";

            long validateNextHi(SqlCommand cmd)
            {
                cmd.CommandText = $"SELECT * FROM NHILO WHERE ENTITY = '{ entityName }'";
                using var reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetInt64(reader.GetOrdinal("NEXT_HI"));
            }

            await TestDatabase(funcAppSettings, validateNextHi, entityName);
        }
        [Fact]
        [Trait("Category", "Integration")]
        public async void ShouldConnectToABrandNewDatabaseAndGetKeyUsingSequence()
        {
            string entityName = "myMSSqlSequenceEntity";
            string funcAppSettings(string connectionString) => $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100"",
                        ""StorageType"" : ""Sequence""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{ connectionString }"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}";

            long validateNextHi(SqlCommand cmd)
            {
                cmd.CommandText = $"SELECT current_value FROM sys.sequences WHERE name = 'SQ_HiLo_{ entityName }'";
                using var reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetInt64(0) + 1; // +1 because Sequence works different from the table implementation
            }

            await TestDatabase(funcAppSettings, validateNextHi, entityName);
        }
        private async Task TestDatabase(Func<string, string> funcAppSettings, Func<SqlCommand, long> validateNextHi, string entityName)
        {
            var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithDatabase(new MsSqlTestcontainerConfiguration
                {
                    Password = "myP@ssword100",
                });

            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(funcAppSettings(testcontainer.ConnectionString))));

            var factory = new HiLoGeneratorFactory(builder.Build());
            var generator = factory.GetKeyGenerator(entityName);
            long key = generator.GetKey();
            _output.WriteLine($"Key generated: '{key}'");
            Assert.True(key > 0, "Expected key to be greater than 0.");

            await using var connection = new SqlConnection(testcontainer.ConnectionString);
            connection.Open();
            await using var cmd = new SqlCommand();
            cmd.Connection = connection;
            long nexttHi = validateNextHi(cmd);
            _output.WriteLine($"Next Hi value: '{nexttHi}'");
            Assert.True(nexttHi == 2, "Expected next Hi value to be equal to 2 (first execution).");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void ShouldRaiseErrorForLackOfSELECTPermission()
        {
            var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithDatabase(new MsSqlTestcontainerConfiguration
                {
                    Password = "myP@ssword100",
                });

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
            await ExecuteScript(prepareDbScript, new SqlConnection(testcontainer.ConnectionString));

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(
                $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""Server={ testcontainer.Hostname },{ testcontainer.Port };Database=testDB;User Id=nhilo_user;Password=nhilo_p@ssW0rd;"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}"
                )));
            var factory = new HiLoGeneratorFactory(builder.Build());
            try
            {
                factory.GetKeyGenerator("myMSSqlSequenceEntity");
                Assert.True(false, "Expected exception has not been thrown.");
            }
            catch (NHiLoException ex)
            {
                _output.WriteLine($"InnerException: '{ex.InnerException.Message}'");
                Assert.Equal(229, (ex.InnerException as SqlException).Number);
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void ShouldRaiseErrorForLackOfCREATE_TABLEPermission()
        {
            var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithDatabase(new MsSqlTestcontainerConfiguration
                {
                    Password = "myP@ssword100",
                });

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
            await ExecuteScript(prepareDbScript, new SqlConnection(testcontainer.ConnectionString));

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(
                $@"{{
                    ""NHiLo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""Server={ testcontainer.Hostname },{ testcontainer.Port };Database=testDB;User Id=nhilo_user;Password=nhilo_p@ssW0rd;"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}"
                )));
            var factory = new HiLoGeneratorFactory(builder.Build());
            try
            {
                factory.GetKeyGenerator("myMSSqlSequenceEntity");
                Assert.True(false, "Expected exception has not been thrown.");
            }
            catch (NHiLoException ex)
            {
                _output.WriteLine($"InnerException: '{ex.InnerException.Message}'");
                Assert.Equal(262, (ex.InnerException as SqlException).Number);
            }
        }

        private async Task ExecuteScript(string prepareDbScript, SqlConnection connection)
        {
            var commandStrings = Regex.Split(prepareDbScript, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            connection.Open();
            try
            {
                foreach (string commandString in commandStrings)
                {
                    if (!string.IsNullOrWhiteSpace(commandString.Trim()))
                    {
                        using (var command = new SqlCommand(commandString, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
