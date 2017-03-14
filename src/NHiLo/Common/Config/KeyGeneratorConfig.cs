using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NHiLo.HiLo.Config;

namespace NHiLo.Common
{
    public class KeyGeneratorConfig : ConfigurationSection, IKeyGeneratorConfiguration
    {
        [ConfigurationProperty("hiloKeyGenerator", IsRequired = false)]
        private HiLoConfigElement PrivateHiloKeyGenerator
        {
            get { return this["hiloKeyGenerator"] as HiLoConfigElement; }
        }

        public IHiLoConfiguration HiloKeyGenerator
        {
            get { return PrivateHiloKeyGenerator; }
        }
    }
}
