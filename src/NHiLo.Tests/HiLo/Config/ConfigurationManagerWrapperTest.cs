﻿using Microsoft.Extensions.Configuration;
using NHiLo.HiLo.Config;
using System.IO;
using System.Text;
using Xunit;

namespace NHiLo.Tests.HiLo.Config
{
    public class ConfigurationManagerWrapperTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldGetTheConnectiongStringWithNoProviderInformation()
        {
            var configuration = CreateConfiguration(@"{
                ""ConnectionStrings"":{
                    ""NHiLo"": ""test""
                }
            }");
            var wrapper = new ConfigurationManagerWrapper(configuration);
            Assert.Equal("test", wrapper.GetConnectionStringsSection().ConnectionStrings[0].ConnectionString);
            Assert.Equal(string.Empty, wrapper.GetConnectionStringsSection().ConnectionStrings[0].ProviderName);
        }
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldGetTheConnectiongStringAndProviderName()
        {
            var configuration = CreateConfiguration(@"{
                ""ConnectionStrings"":{
                    ""NHiLo"":{
                        ""ConnectionString"":""test"",
                        ""ProviderName"":""NHiLo.InMemory""
                    }
                }
            }");
            var wrapper = new ConfigurationManagerWrapper(configuration);
            Assert.Equal("test", wrapper.GetConnectionStringsSection().ConnectionStrings[0].ConnectionString);
            Assert.Equal("NHiLo.InMemory", wrapper.GetConnectionStringsSection().ConnectionStrings[0].ProviderName);
        }

        private static IConfiguration CreateConfiguration(string appSettings)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            return builder.Build();
        }
    }
}
