using Insight360.DAL;
using Quartz;
using System;
using Insight360.Facebook;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class Grading : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var minningAccounts = FacebookDBOperations.GetFacebookAccounts(true);
                    foreach (var account in minningAccounts)
                    {
                        new DataCollector().CalculateFBPostGradesThread(account);
                    }
                }
                catch (Exception ex)
                { }
            }
        }
    }
}