using NHiLo.Example.Repository;
using Ninject.Modules;

namespace NHiLo.Example.IoC
{
    internal class NHiLoExampleModule : NinjectModule
    {
        public override void Load()
        {
            var factoryLong = new HiLoGeneratorFactory();
            Bind<IKeyGenerator<long>>().ToMethod((ctx) => factoryLong.GetKeyGenerator("person")).Named("person");
            var factoryGuid = new GuidGeneratorFactory();
            Bind<IKeyGenerator<string>>().ToMethod((ctx) => factoryGuid.GetKeyGenerator("contact")).Named("contact");
            Bind<IPersonRepository>().To<SqlServerCePersonRepository>();
        }
    }
}
