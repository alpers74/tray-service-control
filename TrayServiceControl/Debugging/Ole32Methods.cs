using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace TrayServiceControl.Debugging
{
    internal static class Ole32Methods
    {
        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
    }

}
