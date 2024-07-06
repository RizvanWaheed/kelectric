using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Insight360.Common;
using System.Net;

namespace Insight360.Scheduling
{
    public class DataProcessor : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                WebClient http = new WebClient();
                logger.Info(AppSettings.SchedulerURL + " website called it self");
                string Result = http.DownloadString(AppSettings.SchedulerURL);
                DAL.DBUtil.ConnectionString = AppSettings.WebMiner_Production;
                new SchedulingInitiator().Process();
            }
            catch (Exception ex)
            { }
        }
    }
}