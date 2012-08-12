using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;

namespace TrayServiceControl.Service
{
    internal class ServiceControl
    {
        public IEnumerable<ServiceController>  Services
        {
            get
            {
                return ServiceController.GetServices();
            }
        }

        // TODO: dont expose this
        public static Process ServiceToProcess(ServiceController service)
        {
            var mo = new ManagementObject(@"Win32_service.Name='" + service.ServiceName + "'");
            var o = mo.GetPropertyValue("ProcessId");
            int processId = (int)((UInt32)o);
            var process = Process.GetProcessById(processId);
            return process;
        }
    }
}
