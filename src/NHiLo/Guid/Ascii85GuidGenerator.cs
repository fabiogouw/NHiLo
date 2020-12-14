using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NHiLo.Guid
{
    /// <summary>
    /// Create sequential guids to be used as keys encoding them as Ascii85 to produce 20 byte strings (smaller).
    /// </summary>
    public class Ascii85GuidGenerator : IKeyGenerator<string>
    {
        private readonly Ascii85 _encoder = new Ascii85();
        public string GetKey()
        {
            var guid = System.Guid.NewGuid();
            return _encoder.Encode(guid.ToByteArray());
        }
    }
}
