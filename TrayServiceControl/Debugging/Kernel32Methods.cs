using System;
using System.Runtime.InteropServices;

namespace TrayServiceControl.Debugging
{
    internal static class Kernel32Methods
    {
        [DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)]out bool isDebuggerPresent);
    }
}
