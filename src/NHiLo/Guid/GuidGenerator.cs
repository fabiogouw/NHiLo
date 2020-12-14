using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NHiLo.Guid
{
    /// <summary>
    /// Create sequential guids to be used as keys.
    /// </summary>
    /// <remarks>
    /// These are v1 guids with the same logic as SQL Server's NEWSEQUENTIALID function. 
    /// Be aware that these guids are generated with the MAC address of the computer, so this security flaw might be exploited.
    /// Be sure this isn't a problem for you.
    /// </remarks>
    public class GuidGenerator : IKeyGenerator<string>
    {
        public string GetKey()
        {
            System.Guid guid;
            NativeMethods.UuidCreateSequential(out guid);
            var s = guid.ToByteArray();
            var t = new byte[16];
            t[3] = s[0];
            t[2] = s[1];
            t[1] = s[2];
            t[0] = s[3];
            t[5] = s[4];
            t[4] = s[5];
            t[7] = s[6];
            t[6] = s[7];
            t[8] = s[8];
            t[9] = s[9];
            t[10] = s[10];
            t[11] = s[11];
            t[12] = s[12];
            t[13] = s[13];
            t[14] = s[14];
            t[15] = s[15];
            return new System.Guid(t).ToString();
        }
    }
}
