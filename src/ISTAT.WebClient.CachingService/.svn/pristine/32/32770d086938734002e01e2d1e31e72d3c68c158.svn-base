using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ISTAT.WebClientCachingMonitor
{
    public class TaskTrayApplicationContext : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        Configuration configWindow = new Configuration();
        ShowLog logWindow = new ShowLog();

        public TaskTrayApplicationContext()
        {
            //MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.Icon = ISTAT.WebClientCachingMonitor.Properties.Resources.Treetog_Junior_Monitor_helix;
            //notifyIcon.DoubleClick += new EventHandler(ShowMessage);
            notifyIcon.DoubleClick += new EventHandler(ShowLogs);
            //notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, exitMenuItem });
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { exitMenuItem });
            notifyIcon.Visible = true;
        }

        void ShowLogs(object sender, EventArgs e)
        {
            // If we are already showing the window meerly focus it.
            if (logWindow.Visible)
                logWindow.Focus();
            else
                logWindow.ShowDialog();
        }

        void ShowMessage(object sender, EventArgs e)
        {
            // Only show the message if the settings say we can.
            if (ISTAT.WebClientCachingMonitor.Properties.Settings.Default.ShowMessage)
                MessageBox.Show("Hello World");
        }

        void ShowConfig(object sender, EventArgs e)
        {
            // If we are already showing the window meerly focus it.
            if (configWindow.Visible)
                configWindow.Focus();
            else
                configWindow.ShowDialog();
        }

        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;

            Application.Exit();
        }


    }
}
