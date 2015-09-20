using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ISTAT.WebClientCachingService
{
    public partial class ISTATWebClientCachingService : ServiceBase
    {

        CachingManager manager = new CachingManager();

        public ISTATWebClientCachingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            manager.Start();
            //while (true)
            //{
            //        Console.ReadKey();
            //}
        }

        protected override void OnStop()
        {
            manager.Stop();
        }
    }
}
