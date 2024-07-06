using Insight360.Common.BO;
using Insight360.DAL;
using Insight360.Util;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class SocialProfilesExtraction : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                Util.Log.WriteOperationStarted(typeof(SocialProfilesExtraction), "SocialProfileExtraction - ExtractCustomerSocialProfiles", "Thread Enter");
                try
                {
                    DateTimeOffset? previousTime = context.Trigger.GetPreviousFireTimeUtc();
                    if (previousTime.HasValue)
                    {
                        db.ExtractSocialProfilesCustomers(((DateTimeOffset)previousTime).LocalDateTime.AddMinutes(-4));
                    }
                    else
                    {
                        db.ExtractSocialProfilesCustomers(null);
                    }
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(typeof(SocialProfilesExtraction), "SocialProfileExtraction - ExtractCustomerSocialProfiles", ex);
                }
                finally
                {
                    Util.Log.WriteOperationStarted(typeof(SocialProfilesExtraction), "SocialProfileExtraction - ExtractCustomerSocialProfiles", "Thread Exit");
                }
            }
        }

        
    }
}