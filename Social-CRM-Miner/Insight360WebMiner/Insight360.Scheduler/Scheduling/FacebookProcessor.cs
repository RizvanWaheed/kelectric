using Insight360.DAL;
using Insight360.Facebook;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class FacebookProcessor : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //here we Fetch Data from. and apply processing.
                var dataMap = context.MergedJobDataMap;
                var IsPage = (bool)dataMap["IsPage"];
                var FetchType = (string)dataMap["FetchType"];
                var minningAccounts = FacebookDBOperations.GetFacebookAccounts(IsPage);
                foreach (var account in minningAccounts)
                {
                    if (FetchType == "PF")
                        new DataCollector().FetchFBPostsThread(account);
                    else if (FetchType == "CF")
                        new DataCollector().FetchFBCommentsThread(account);
                    else if (FetchType == "CNF")
                        new DataCollector().FetchFBNestedCommentsThread(account);
                    else if (FetchType == "CPF")
                        new DataCollector().FetchFBPagesChatsThread(account);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}