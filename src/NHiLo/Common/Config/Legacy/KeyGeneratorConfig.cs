using NHiLo.HiLo.Config.Legacy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace NHiLo.Common.Legacy
{
    public class KeyGeneratorConfig : ConfigurationSection
    {
        [ConfigurationProperty("hiloKeyGenerator", IsRequired = false)]
        public HiLoConfigElement HiloKeyGenerator
        {
            get { return this["hiloKeyGenerator"] as HiLoConfigElement; }
        }
    }
}
