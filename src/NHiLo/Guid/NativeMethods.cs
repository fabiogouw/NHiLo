using System.Runtime.InteropServices;

namespace NHiLo.Guid
{
    internal class NativeMethods
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        internal static extern int UuidCreateSequential(out System.Guid guid);
    }
}
