using Insight360.DAL;
using Insight360.Twitter;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class TwitterProcessor : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //here we Fetch Data from. and apply processing.
                var dataMap = context.MergedJobDataMap;
                var myInstance = (string)dataMap["FetchType"];
                var minningAccounts = TwitterDBOperations.GetTwitterAccounts();
                if(myInstance == "ST")
                {
                    var searchKeywords = TwitterDBOperations.GetSearchKeywords();
                    foreach (var keyword in searchKeywords)
                    {
                        new DataCollector().KeywordsSearchThread(minningAccounts[0], keyword);
                    }
                }
                else
                {
                    foreach (var account in minningAccounts)
                    {
                        if (myInstance == "PT")
                            new DataCollector().UserTweetsThread(account);
                        else if (myInstance == "MT")
                            new DataCollector().UserDirectMessageEventsThread(account);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}