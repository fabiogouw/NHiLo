using FluentAssertions;
using NHiLo.HiLo;
using NHiLo.HiLo.Config;
using NHiLo.HiLo.Repository;
using NHiLo.Tests.TestDoubles.Config;
using System;
using System.Collections.Generic;
using Xunit;

namespace NHiLo.Tests.HiLo.Repository
{
    public class HiLoRepositoryFactoryTest
    {
        private class InMemoryDummyRepositoryProvider : IHiLoRepositoryProvider
        {
            public string Name => "InMemoryDummy";
            public IHiLoRepository Build(IHiLoConfiguration config)
            {
                return new InMemoryHiloRepository();
            }
        }

        [Fact]
        public void Should_Register_And_Resolve_Custom_Provider()
        {
            var providerType = typeof(InMemoryDummyRepositoryProvider);
            var config = new HiloConfigurationStub
            {
                ProviderName = "InMemoryDummy",
                Providers = new List<IRepositoryProviderElement>
                {
                    new RepositoryProviderElementStub { Type = providerType.AssemblyQualifiedName }
                },
                GetEntityConfigFunction = _ => new EntityConfigurationStub { Name = "entity", MaxLo = 10 }
            };

            var factory = new HiLoRepositoryFactory(config);
            var repo = (ExceptionWrapperRepository)factory.GetRepository("entity", config);
            repo.RepositoryType.Should().Be<InMemoryHiloRepository>();
        }

        [Fact]
        public void Should_Throw_When_Provider_Type_Is_Invalid()
        {
            var config = new HiloConfigurationStub
            {
                ProviderName = "Invalid",
                Providers = new List<IRepositoryProviderElement>
                {
                    new RepositoryProviderElementStub { Type = "Non.Existent.Type, NonExistentAssembly" }
                },
                GetEntityConfigFunction = _ => new EntityConfigurationStub { Name = "entity", MaxLo = 10 }
            };

            Action act = () => new HiLoRepositoryFactory(config);
            act.Should().Throw<NHiLoException>()
                .Where(e => e.ErrorCode == ErrorCodes.ProviderInstantiationFailed);
        }

        [Fact]
        public void Should_Throw_When_Provider_Does_Not_Implement_Interface()
        {
            var config = new HiloConfigurationStub
            {
                ProviderName = "String",
                Providers = new List<IRepositoryProviderElement>
                {
                    new RepositoryProviderElementStub { Type = typeof(string).AssemblyQualifiedName }
                },
                GetEntityConfigFunction = _ => new EntityConfigurationStub { Name = "entity", MaxLo = 10 }
            };

            Action act = () => new HiLoRepositoryFactory(config);
            act.Should().Throw<NHiLoException>()
                .Where(e => e.ErrorCode == ErrorCodes.ProviderInstantiationFailed);
        }

        private class NoParameterlessCtorProvider : IHiLoRepositoryProvider
        {
            public string Name => "NoCtor";
            public IHiLoRepository Build(IHiLoConfiguration config) => new InMemoryHiloRepository();
            public NoParameterlessCtorProvider(string arg) { }
        }

        [Fact]
        public void Should_Throw_When_Provider_Has_No_Parameterless_Constructor()
        {
            var config = new HiloConfigurationStub
            {
                ProviderName = "NoCtor",
                Providers = new List<IRepositoryProviderElement>
                {
                    new RepositoryProviderElementStub { Type = typeof(NoParameterlessCtorProvider).AssemblyQualifiedName }
                },
                GetEntityConfigFunction = _ => new EntityConfigurationStub { Name = "entity", MaxLo = 10 }
            };

            Action act = () => new HiLoRepositoryFactory(config);
            act.Should().Throw<NHiLoException>()
                .Where(e => e.ErrorCode == ErrorCodes.ProviderInstantiationFailed);
        }
    }
}
