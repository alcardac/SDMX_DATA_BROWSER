using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace ISTAT.WebClient.Caching
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + @"/log4net.xml";
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(path));

            Caching.CachingManager manager = new CachingManager();
            manager.Start();
            while (Properties.Settings.Default.EnableCaching)
            {
                Console.ReadKey();
            }

        }
    }
}
