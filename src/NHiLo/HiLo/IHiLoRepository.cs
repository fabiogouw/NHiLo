using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo.HiLo
{
    public interface IHiLoRepository
    {
        void PrepareRepository();
        long GetNextHi();
    }
}
