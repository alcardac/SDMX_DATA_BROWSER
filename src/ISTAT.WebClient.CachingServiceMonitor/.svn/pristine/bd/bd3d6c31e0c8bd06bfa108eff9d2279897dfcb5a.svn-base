using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.ServiceProcess;

namespace ISTAT.WebClientCachingMonitor
{
    public partial class ShowLog : Form
    {
        string logFilePath;
        string serviceName = "ISTATWebClientCachingService";
        ServiceController service;

        public ShowLog()
        {
            InitializeComponent();
        }

        private void getService()
        {
            //service = new ServiceController(serviceName);
            ServiceController[] services = ServiceController.GetServices();
            service = services.Where(dp => dp.DisplayName == serviceName).FirstOrDefault();
        }

        private void CheckServiceStatus()
        {
            //ServiceController[] services = ServiceController.GetServices();
            //ServiceController cachingService = services.Where(dp => dp.DisplayName == serviceName).FirstOrDefault();

            getService();

            if (service == null)
            {
                lblServiceStatus.ForeColor = Color.Red;
                lblServiceStatus.Text = "Not Found";

                return;
            }

            switch (service.Status)
            {
                case ServiceControllerStatus.ContinuePending:
                case ServiceControllerStatus.PausePending:
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.StopPending:
                    btnStartService.Visible = false;
                    btnStopService.Visible = false;
                    break;
                case ServiceControllerStatus.Running:
                    lblServiceStatus.ForeColor = Color.Green;
                    btnStartService.Visible = false;
                    btnStopService.Visible = true;
                    btnStopService.Location = btnStartService.Location;
                    break;
                case ServiceControllerStatus.Stopped:
                    lblServiceStatus.ForeColor = Color.Orange;
                    btnStartService.Visible = true;
                    btnStopService.Visible = false;
                    btnStartService.Location = btnStopService.Location;
                    break;
                default:
                    break;
            }

            lblServiceStatus.Text = service.Status.ToString();
        }

        private void StartService()
        {
            getService();
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(8000);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopService()
        {
            getService();
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(8000);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowLog_Load(object sender, EventArgs e)
        {
            //RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            //string location = FindByDisplayName(regKey, "MSN"); 

            CheckServiceStatus();

            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(timer1_Tick);
            ReadFile();
            SetAutoRefresh();
        }

        private void ReadFile()
        {
            CheckServiceStatus();
            if (lblServiceStatus.Text.ToUpper() != "RUNNING")
                return;
            try
            {
                string filePath;
                filePath = AppDomain.CurrentDomain.BaseDirectory + @"logging\ISTAT.WebClientCaching.log";

                //if (!File.Exists(filePath))
                //    return;

                string textLog = "";

                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs))
                {
                    textLog = sr.ReadToEnd();
                }

                txtShowLog.Text = textLog;

                txtShowLog.SelectionStart = txtShowLog.Text.Length;
                txtShowLog.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReadFile();
        }

        private void chkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoRefresh();
        }

        private void SetAutoRefresh()
        {
            if (chkAutoRefresh.Checked)
                timer1.Start();
            else
                timer1.Stop();
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            ReadFile();
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            StartService();
            CheckServiceStatus();
        }

        private void btnStopService_Click(object sender, EventArgs e)
        {
            StopService();
            CheckServiceStatus();
        }


    }
}
