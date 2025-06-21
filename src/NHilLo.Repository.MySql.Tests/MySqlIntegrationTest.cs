using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Testcontainers.MySql;

namespace NHiLo.Tests.Integration.HiLo.Repository.MySql
{
    [Collection("Database Integration")]
    public class MySqlIntegrationTest
    {
        private readonly ITestOutputHelper _output;

        public MySqlIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ConnectToABrandNewDatabaseAndGetKey()
        {
            var testcontainersBuilder = new MySqlBuilder()
                .WithDatabase("myDataBase")
                .WithPassword("myUser")
                .WithUsername("myPassword");

            await using (var testcontainer = testcontainersBuilder.Build())
            {
                await testcontainer.StartAsync();
                var appSettings = $@"{{
                    ""NHiLo"":{{
                        ""Providers"": [{{ ""Name"": ""MySqlConnector"", ""Type"": ""NHiLo.HiLo.Repository.MySqlHiloRepositoryProvider, NHilLo.Repository.MySql"" }}],
                        ""DefaultMaxLo"" : ""100""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""{testcontainer.GetConnectionString()}"",
                            ""ProviderName"":""MySqlConnector""
                        }}
                    }}
                }}";
                var builder = new ConfigurationBuilder();
                builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
                var factory = new HiLoGeneratorFactory(builder.Build());

                var generator = factory.GetKeyGenerator("myMySqlEntity");
                long key = generator.GetKey();
                _output.WriteLine($"Key generated: '{key}'");
                key.Should().BeGreaterThan(0, "is expected the key to be greater than 0.");

                await using (var connection = new MySqlConnection(testcontainer.GetConnectionString()))
                {
                    connection.Open();
                    await using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM NHILO WHERE ENTITY = 'myMySqlEntity'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            long nexttHi = reader.GetInt64("NEXT_HI");
                            _output.WriteLine($"Next Hi value: '{nexttHi}'");
                            nexttHi.Should().Be(2, "is expected the next Hi value to be equal to 2 (first execution).");
                        }
                    }
                }
            }
        }
    }
}
