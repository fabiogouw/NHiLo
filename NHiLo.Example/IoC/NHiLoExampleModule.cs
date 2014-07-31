using NHiLo.Example.Repository;
using Ninject.Modules;

namespace NHiLo.Example.IoC
{
    internal class NHiLoExampleModule : NinjectModule
    {
        public override void Load()
        {
            var factory = new HiLoGeneratorFactory();
            Bind<IKeyGenerator<long>>().ToMethod((ctx) => factory.GetKeyGenerator("person")).Named("person");
            Bind<IKeyGenerator<long>>().ToMethod((ctx) => factory.GetKeyGenerator("contact")).Named("contact");
            Bind<IPersonRepository>().To<SqlServerCePersonRepository>();
        }
    }
}
