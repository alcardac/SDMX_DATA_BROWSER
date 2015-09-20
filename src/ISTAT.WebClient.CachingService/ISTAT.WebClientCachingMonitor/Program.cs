using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ISTAT.WebClientCachingMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //if (StartUpManager.IsUserAdministrator() && !StartUpManager.CheckApplicationRunning())
            //    StartUpManager.AddApplicationToAllUserStartup();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Instead of running a form, we run an ApplicationContext.
            Application.Run(new TaskTrayApplicationContext());
        }
    }
}