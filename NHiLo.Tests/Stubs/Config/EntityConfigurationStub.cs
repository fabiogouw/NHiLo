using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.HiLo.Config;

namespace NHiLo.Tests.Stubs.Config
{
    public class EntityConfigurationStub : IEntityConfiguration
    {
        public bool Name { get; set; }

        public int MaxLo { get; set; }
    }
}
