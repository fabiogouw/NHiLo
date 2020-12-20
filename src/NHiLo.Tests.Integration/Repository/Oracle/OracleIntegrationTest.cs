using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using Xunit.Abstractions;
using Oracle.ManagedDataAccess.Client;

namespace NHiLo.Tests.Integration.Repository.Oracle
{
    [Collection("Database Integration")]
    public class OracleIntegrationTest
    {
        private readonly ITestOutputHelper _output;

        public OracleIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void ShouldConnectToABrandNewDatabaseAndGetKey()
        {
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("wnameless/oracle-xe-11g-r2")
              .WithName("oracle-nhilo")
              .WithEnvironment("ORACLE_ALLOW_REMOTE", "true")
              .WithEnvironment("ORACLE_HOME", "/u01/app/oracle/product/11.2.0/xe")
              .WithPortBinding(1521)
              .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilCommandIsCompleted($"echo \"exit\" | $ORACLE_HOME/bin/sqlplus -L system/oracle@localhost:1521/xe | grep Connected > /dev/null"));

            await using (var testcontainer = testcontainersBuilder.Build())
            {
                await testcontainer.StartAsync();
                string connectionString = $@"Data Source={ testcontainer.Hostname }:1521/xe;User Id=system;Password=oracle;";

                var appSettings = $@"{{
                    ""NHilo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{ connectionString }"",
                            ""ProviderName"":""System.Data.OracleClient""
                        }}
                    }}
                }}";
                var builder = new ConfigurationBuilder();
                builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
                var factory = new HiLoGeneratorFactory(builder.Build());

                var generator = factory.GetKeyGenerator("myOracleEntity");
                long key = generator.GetKey();
                _output.WriteLine($"Key generated: '{key}'");
                Assert.True(key > 0, "Expected key to be greater than 0.");

                await using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    await using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM NHILO WHERE ENTITY = 'myOracleEntity'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            long nexttHi = reader.GetInt64(reader.GetOrdinal("NEXT_HI"));
                            _output.WriteLine($"Next Hi value: '{nexttHi}'");
                            Assert.True(nexttHi == 2, "Expected next Hi value to be equal to 2 (first execution).");
                        }
                    }
                }
            }
        }
    }
}
