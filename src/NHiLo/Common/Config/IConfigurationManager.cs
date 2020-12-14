using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using NHiLo.HiLo.Config;

namespace NHiLo.Common.Config
{
    public interface IConfigurationManager
    {
        //NameValueCollection AppSettings { get; }
        //ConnectionStringSettingsCollection ConnectionStrings { get; }
        //T GetSection<T>(string sectionName);
        IHiLoConfiguration GetKeyGeneratorConfigurationSection();
        ConnectionStringsSection GetConnectionStringsSection();
    }
}
