using Insight360.DAL;
using Insight360.Instagram;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class InstagramProfileProcessor : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //here we Fetch Data from. and apply processing.
                var minningAccounts = new InstagramDBOperations().GetInstagramAccounts();
                new InstagramClient().FetchProfileInformation(minningAccounts);
            }
            catch (Exception ex)
            {
            }
        }
    }
}