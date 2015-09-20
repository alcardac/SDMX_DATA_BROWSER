using ISTAT.WebClient.CacheManager.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ISTAT.WebClient.CacheManager
{
    public class CacheManager
    {
        private static bool IsRun { get; set; }
        private static object objlock = new object();
        private static ThreadBackgroundManager tbm = null;
        private static Thread threadBackgroundManager = null;

        public static void RunManager()
        {
            lock (objlock)
            {
                if (IsRun)
                    return;
                IsRun = true;
                tbm = new ThreadBackgroundManager();
                threadBackgroundManager = new Thread(new ThreadStart(tbm.Start));
                threadBackgroundManager.Name = "ThreadBackgroundManager";
                threadBackgroundManager.Priority = ThreadPriority.Lowest;
                threadBackgroundManager.Start();
            }
        }

        public static void StopManager()
        {
            lock (objlock)
            {
                if (threadBackgroundManager != null)
                {
                    threadBackgroundManager.Abort();
                    threadBackgroundManager = null;
                }
                if (tbm != null)
                {
                    tbm.Stop();
                    tbm = null;
                }
            }
        }

    }
}
