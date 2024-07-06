using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Net;
using Insight360.Common;


namespace Insight360.Scheduling
{
    public class KeepAlive : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                WebClient http = new WebClient();
                logger.Info(AppSettings.SchedulerURL + " website called it self");
                string Result = http.DownloadString(AppSettings.SchedulerURL);
            }
            catch (Exception ex)
            {
                logger.Info("KeepAlive --- " + ex.Message);
            }
        }
    }
}