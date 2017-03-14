using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHiLo.HiLo;

namespace NHiLo.Tests.Integration.Hilo
{
    [TestClass]
    public class HiLoGeneratorFactoryTest
    {
        [TestClass]
        public class GetKeyGenerator
        {
            [TestMethod]
            public void ShouldReturnAValidKey()
            {
                // Arrange
                var factory = new HiLoGeneratorFactory();
                // Act
                var generator = factory.GetKeyGenerator("dummy5");
                var key1 = generator.GetKey();
                var key2 = generator.GetKey();
                // Assert
                Assert.IsTrue(key1 > 0);
                Assert.IsTrue(key2 > 0);
            }
        }
    }
}
