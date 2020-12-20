using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using System;
using System.Net;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using Xunit.Abstractions;

namespace NHiLo.Tests.Integration.Repository.MySql
{
    public class MySqlIntegrationTest
    {
        private readonly ITestOutputHelper _output;

        public MySqlIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void ShouldConnectToABrandNewDatabaseAndGetKey()
        {
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("mysql:latest")
              .WithName("mysql-nhilo")
              .WithEnvironment("MYSQL_ROOT_PASSWORD", "my-secret-pw")
              .WithEnvironment("MYSQL_DATABASE", "myDataBase")
              .WithEnvironment("MYSQL_USER", "myUser")
              .WithEnvironment("MYSQL_PASSWORD", "myPassword")
              .WithPortBinding(3306)
              .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(3306));

            await using (var testcontainer = testcontainersBuilder.Build())
            {
                await testcontainer.StartAsync();
                string connectionString = $"Server={ testcontainer.Hostname };Database=myDataBase;Uid=myUser;Pwd=myPassword;";
                var appSettings = $@"{{
                    ""NHilo"":{{
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{ connectionString }"",
                            ""ProviderName"":""MySql.Data.MySqlClient""
                        }}
                    }}
                }}";
                var builder = new ConfigurationBuilder();
                builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
                var factory = new HiLoGeneratorFactory(builder.Build());

                var generator = factory.GetKeyGenerator("myEntity");
                long key = generator.GetKey();
                _output.WriteLine($"Key generated: '{key}'");
                Assert.True(key > 0, "Expected key to be greater than 0.");

                await using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    await using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM NHILO WHERE ENTITY = 'myEntity'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            long nexttHi = reader.GetInt64("NEXT_HI");
                            _output.WriteLine($"Next Hi value: '{nexttHi}'");
                            Assert.True(nexttHi == 2, "Expected next Hi value to be equal to 2 (first execution).");
                        }
                    }
                }
            }
        }
    }
}
