using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Diagnostics;

namespace ISTAT.WebClientCachingService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller1.ServiceName).Start();
            //StartApplicationMonitor();
        }


        private void StartApplicationMonitor()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ISTAT.WebClientCachingMonitor.exe";

            System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
            appLog.Source = "PERCORSO";
            appLog.WriteEntry(path);

            //// Prepare the process to run
            //ProcessStartInfo start = new ProcessStartInfo();
            //// Enter in the command line arguments, everything you would enter after the executable name itself
            ////start.Arguments = arguments; 
            //// Enter the executable to run, including the complete path
            //start.FileName = path;
            //// Do you want to show a console window?
            //start.WindowStyle = ProcessWindowStyle.Hidden;
            //start.CreateNoWindow = true;
            //int exitCode;


            //// Run the external process & wait for it to finish
            //using (Process proc = Process.Start(start))
            //{
            //    proc.WaitForExit();
            //    // Retrieve the app's exit code
            //    exitCode = proc.ExitCode;
            //}
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

    }
}
