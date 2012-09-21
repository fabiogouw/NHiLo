using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo.Common
{
    public interface IKeyGenerator<T>
    {
        T GetKey();
    }
}
