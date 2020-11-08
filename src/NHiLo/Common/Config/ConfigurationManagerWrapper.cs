using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace NHiLo.Common.Config
{
    public class ConfigurationManagerWrapper : IConfigurationManager
    {

        private IConfigurationRoot _configuration;

        public ConfigurationManagerWrapper(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public T GetSection<T>(string sectionName)
        {
            return (T)_configuration.GetSection(sectionName);
        }
    }
}
