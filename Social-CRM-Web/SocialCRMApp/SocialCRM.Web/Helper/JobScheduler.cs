using System;
using Quartz;
using Quartz.Impl;

namespace SocialCRM.Web
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<Jobclass>().Build();
            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule("0 0 1 * * ?").Build();
            scheduler.ScheduleJob(job, trigger);
        }
        
    }

    [DisallowConcurrentExecution]
    public class Jobclass : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            new Repositories.ReportRepository().BuildAgentPerformanceReport();
        }
    }
}