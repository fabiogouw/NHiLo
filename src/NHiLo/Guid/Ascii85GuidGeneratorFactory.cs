using NHiLo.Guid;

namespace NHiLo // this should be available at the root namespace
{
    /// <summary>
    /// Factory that creates <see cref="IKeyGeneratorFactory"/> for client usage.
    /// </summary>
    public class Ascii85GuidGeneratorFactory : IKeyGeneratorFactory<string>
    {
        public IKeyGenerator<string> GetKeyGenerator(string entityName)
        {
            return new Ascii85GuidGenerator();
        }
    }
}
