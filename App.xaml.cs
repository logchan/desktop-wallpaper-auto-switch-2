using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
namespace DWAS2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // prevent multi-launch
            string procName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(procName);
            if(processes.Length > 1)
            {
                foreach(var proc in processes)
                {
                    if (proc.Id != Process.GetCurrentProcess().Id)
                        proc.Kill();
                }
            }

            base.OnStartup(e);
        }
    }
}
