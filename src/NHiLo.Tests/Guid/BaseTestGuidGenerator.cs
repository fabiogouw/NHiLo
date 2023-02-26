using Xunit;

namespace NHiLo.Tests.Guid
{
    public class BaseTestGuidGenerator
    {
        public abstract class GetKey
        {
            protected static void ShouldGetANonNullGuid(IKeyGenerator<string> generator)
            {
                // Act
                string key = generator.GetKey();
                // Assert
                Assert.NotEqual(System.Guid.Empty.ToString(), key);
            }

            protected static void ShouldGetDifferentKeys(IKeyGenerator<string> generator)
            {
                // Act
                string key1 = generator.GetKey();
                string key2 = generator.GetKey();
                // Assert
                Assert.NotEqual(key1, key2);
            }
        }
    }
}
