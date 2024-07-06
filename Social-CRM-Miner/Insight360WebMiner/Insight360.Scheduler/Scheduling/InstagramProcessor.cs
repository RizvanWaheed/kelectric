using Insight360.DAL;
using Insight360.Instagram;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insight360.Scheduling
{

    [DisallowConcurrentExecution]
    public class InstagramProcessor : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //here we Fetch Data from. and apply processing.
                var minningAccounts = new InstagramDBOperations().GetInstagramAccounts();
                foreach (var account in minningAccounts)
                {
                    new InstagramClient().FeatchReport(account);
                    new InstagramClient().FetchComments(account);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}