using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Quartz;
using Insight360.Common;

namespace Insight360.Scheduling.Quartz
{
    public class JobListener : IJobListener
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Name
        {
            get { return "CurrentTimeListener"; }
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            logger.Info($"Job {context.JobDetail.Key.Name} in group {context.JobDetail.Key.Group} is about to be executed");
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            logger.Info($"Job {context.JobDetail.Key.Name} in group {context.JobDetail.Key.Group} is Vetoed");
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            logger.Info($"Job {context.JobDetail.Key.Name} in group {context.JobDetail.Key.Group} was executed");

            // only run second job if first job was executed successfully
            if (jobException == null)
            {
                // fetching name of the job to be executed sequentially
            }
            else
            {
                Debug.WriteLine("An exception occured while executing job: {0} in group {1} with following details : {2}",
                    context.JobDetail.Key.Name, context.JobDetail.Key.Group, jobException.Message);
            }
        }
    }
}