using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
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
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("mcr.microsoft.com/mssql/server")
              .WithName("mssql-nhilo")
              .WithEnvironment("ACCEPT_EULA", "Y")
              .WithEnvironment("SA_PASSWORD", "myP@ssword100")
              .WithPortBinding(1433)
              .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted($"/opt/mssql-tools/bin/sqlcmd -S 'localhost,1433' -U 'sa' -P 'myP@ssword100'"));

            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();
            string connectionString = $"Server={ testcontainer.Hostname };Database=master;User Id=sa;Password=myP@ssword100;";

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(funcAppSettings(connectionString))));

            var factory = new HiLoGeneratorFactory(builder.Build());
            var generator = factory.GetKeyGenerator(entityName);
            long key = generator.GetKey();
            _output.WriteLine($"Key generated: '{key}'");
            Assert.True(key > 0, "Expected key to be greater than 0.");

            await using var connection = new SqlConnection(connectionString);
            connection.Open();
            await using var cmd = new SqlCommand();
            cmd.Connection = connection;
            long nexttHi = validateNextHi(cmd);
            _output.WriteLine($"Next Hi value: '{nexttHi}'");
            Assert.True(nexttHi == 2, "Expected next Hi value to be equal to 2 (first execution).");
        }
    }
}
