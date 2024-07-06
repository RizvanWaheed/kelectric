using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Insight360.Common;
using Insight360.Scheduler.Scheduling;

namespace Insight360.Scheduling.Quartz
{
    public class JobScheduler
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Start()
        {
            logger.Info("**Started...........**");
            //new SchedulingInitiator().MineWeb();
            
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            //-----------------------------------Instragram Mining -----------------------------------------------------------------------------
            //IJobDetail instagramJob = JobBuilder.Create<InstagramProcessor>().WithIdentity("InstagramMinning", "InstagramMinningGroup").Build();
            //ITrigger instagramTrigger = TriggerBuilder.Create()
            //    .WithIdentity("InstagramMinningTrigger", "InstagramMinningGroup").StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(1)
            //        .RepeatForever())
            //    .Build();
            //scheduler.ScheduleJob(instagramJob, instagramTrigger);

            //IJobDetail instagramProfileJob = JobBuilder.Create<InstagramProfileProcessor>().WithIdentity("InstagramProfileMinning", "InstagramProfileMinningGroup").Build();
            //ITrigger instagramProfileTrigger = TriggerBuilder.Create()
            //    .WithIdentity("InstagramProfileMinningTrigger", "InstagramProfileMinningGroup").StartNow()
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(23)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(12, 00))
            //      )
            //    .Build();
            //scheduler.ScheduleJob(instagramProfileJob, instagramProfileTrigger);

            //-----------------------------------Feedly Keywords Mining -----------------------------------------------------------------------------
            /*IJobDetail feedlyJob = JobBuilder.Create<FeedlyProcessor>().WithIdentity("FeedlyMinning", "FeedlyMinningGroup").Build();
            feedlyJob.JobDataMap.Put("FetchType", "Keywords");
            ITrigger feedlyTrigger = TriggerBuilder.Create()
                .WithIdentity("FeedlyMinningTrigger", "FeedlyMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(feedlyJob, feedlyTrigger);
            
            //-----------------------------------Feedly Pages Mining -----------------------------------------------------------------------------
            IJobDetail feedlyPageJob = JobBuilder.Create<FeedlyProcessor>().WithIdentity("FeedlyPageMinning", "FeedlyPageMinningGroup").Build();
            feedlyPageJob.JobDataMap.Put("FetchType", "Pages");
            ITrigger feedlyPageTrigger = TriggerBuilder.Create()
                .WithIdentity("FeedlyPageMinningTrigger", "FeedlyPageMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(feedlyPageJob, feedlyPageTrigger);
            */

            //-----------------------------------Twitter Mining -----------------------------------------------------------------------------
            IJobDetail twitterTweetJob = JobBuilder.Create<TwitterProcessor>().WithIdentity("TwitterTweetMinning", "TwitterTweetMinningGroup").Build();
            twitterTweetJob.JobDataMap.Put("FetchType", "PT");
            ITrigger twitterTweetTrigger = TriggerBuilder.Create()
                .WithIdentity("TwitterTweetMinningTrigger", "TwitterTweetMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(twitterTweetJob, twitterTweetTrigger);

            IJobDetail twitterMsgJob = JobBuilder.Create<TwitterProcessor>().WithIdentity("TwitterMsgMinning", "TwitterMsgMinningGroup").Build();
            twitterMsgJob.JobDataMap.Put("FetchType", "MT");
            ITrigger twitterMsgTrigger = TriggerBuilder.Create()
                .WithIdentity("TwitterMsgMinningTrigger", "TwitterMsgMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(twitterMsgJob, twitterMsgTrigger);

            /*IJobDetail twitterSearchJob = JobBuilder.Create<TwitterProcessor>().WithIdentity("TwitterSearchMinning", "TwitterSearchMinningGroup").Build();
            twitterSearchJob.JobDataMap.Put("FetchType", "ST");
            ITrigger twitterSearchTrigger = TriggerBuilder.Create()
                .WithIdentity("TwitterSearchMinningTrigger", "TwitterSearchMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(twitterSearchJob, twitterSearchTrigger);
            */

            //-----------------------------------Facebook Mining -----------------------------------------------------------------------------
            IJobDetail fbPostPageJob = JobBuilder.Create<FacebookProcessor>().WithIdentity("fbPostPageMinning", "fbPostPageMinningGroup").Build();
            fbPostPageJob.JobDataMap.Put("FetchType", "PF");
            fbPostPageJob.JobDataMap.Put("IsPage", true);
            ITrigger fbPostPageTrigger = TriggerBuilder.Create()
                .WithIdentity("fbPostPageMinningTrigger", "fbPostPageMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(fbPostPageJob, fbPostPageTrigger);

            IJobDetail fbComPageJob = JobBuilder.Create<FacebookProcessor>().WithIdentity("fbComPageMinning", "fbComPageMinningGroup").Build();
            fbComPageJob.JobDataMap.Put("FetchType", "CF");
            fbComPageJob.JobDataMap.Put("IsPage", true);
            ITrigger fbComPageTrigger = TriggerBuilder.Create()
                .WithIdentity("fbComPageMinningTrigger", "fbComPageMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(fbComPageJob, fbComPageTrigger);

            IJobDetail fbNesComPageJob = JobBuilder.Create<FacebookProcessor>().WithIdentity("fbNesComPageMinning", "fbNesComPageMinningGroup").Build();
            fbNesComPageJob.JobDataMap.Put("FetchType", "CNF");
            fbNesComPageJob.JobDataMap.Put("IsPage", true);
            ITrigger fbNesComPageTrigger = TriggerBuilder.Create()
                .WithIdentity("fbNesComPageMinningTrigger", "fbNesComPageMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(fbNesComPageJob, fbNesComPageTrigger);

            IJobDetail fbChatPageJob = JobBuilder.Create<FacebookProcessor>().WithIdentity("fbChatPageMinning", "fbChatPageMinningGroup").Build();
            fbChatPageJob.JobDataMap.Put("FetchType", "CPF");
            fbChatPageJob.JobDataMap.Put("IsPage", true);
            ITrigger fbChatPageTrigger = TriggerBuilder.Create()
                .WithIdentity("fbChatPageMinningTrigger", "fbChatPageMinningGroup").StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(fbChatPageJob, fbChatPageTrigger);

            //IJobDetail fbPostProfileJob = JobBuilder.Create<FacebookProcessor>().WithIdentity("fbPostProfileMinning", "fbPostProfileMinningGroup").Build();
            //fbPostProfileJob.JobDataMap.Put("FetchType", "PF");
            //fbPostProfileJob.JobDataMap.Put("IsPage", false);
            //ITrigger fbPostProfileTrigger = TriggerBuilder.Create()
            //    .WithIdentity("fbPostProfileMinningTrigger", "fbPostProfileMinningGroup").StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(1)
            //        .RepeatForever())
            //    .Build();
            //scheduler.ScheduleJob(fbPostProfileJob, fbPostProfileTrigger);

            //IJobDetail fbComProfileJob = JobBuilder.Create<FacebookProcessor>().WithIdentity("fbComProfileMinning", "fbComProfileMinningGroup").Build();
            //fbComProfileJob.JobDataMap.Put("FetchType", "CF");
            //fbComProfileJob.JobDataMap.Put("IsPage", false);
            //ITrigger fbComProfileTrigger = TriggerBuilder.Create()
            //    .WithIdentity("fbComProfileMinningTrigger", "fbComProfileMinningGroup").StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(1)
            //        .RepeatForever())
            //    .Build();
            //scheduler.ScheduleJob(fbComProfileJob, fbComProfileTrigger);

            //IJobDetail fbNesComProfileJob = JobBuilder.Create<FacebookProcessor>().WithIdentity("fbNesComProfileMinning", "fbNesComProfileMinningGroup").Build();
            //fbNesComProfileJob.JobDataMap.Put("FetchType", "CNF");
            //fbNesComProfileJob.JobDataMap.Put("IsPage", false);
            //ITrigger fbNesComProfileTrigger = TriggerBuilder.Create()
            //    .WithIdentity("fbNesComProfileMinningTrigger", "fbNesComProfileMinningGroup").StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(1)
            //        .RepeatForever())
            //    .Build();
            //scheduler.ScheduleJob(fbNesComProfileJob, fbNesComProfileTrigger);

            //-----------------------------------Social Profiles -------------------------------------------------------------------------------
            IJobDetail jobProfiling = JobBuilder.Create<SocialProfilesExtraction>().WithIdentity("SocialProfilesExtraction", "SocialProfilesGroup").Build();
            ITrigger triggerProfiling = TriggerBuilder.Create()
                .WithIdentity("SocialProfilesTrigger", "SocialProfilesGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(jobProfiling, triggerProfiling);

            //-----------------------------------Customer Profile Gender------------------------------------------------------------------------------
            /*IJobDetail jobTagging = JobBuilder.Create<CustomerProfileGenders>().WithIdentity("CustomerProfileGenders", "CustomerProfileGendersGroup").Build();
            ITrigger triggerTagging = TriggerBuilder.Create()
                .WithIdentity("CustomerProfileGendersTrigger", "CustomerProfileGendersGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(jobTagging, triggerTagging);

            //-----------------------------------Publisher -> Schedular -------------------------------------------------------------------------------
            IJobDetail jobScheduling = JobBuilder.Create<Schedular>().WithIdentity("Schedular", "SchedularGroup").Build();
            ITrigger triggerScheduling = TriggerBuilder.Create()
                .WithIdentity("SchedularTrigger", "SchedularGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(jobScheduling, triggerScheduling);

            //-----------------------------------Publisher -> Queue -------------------------------------------------------------------------------
            IJobDetail jobQueuing = JobBuilder.Create<Queue>().WithIdentity("Queue", "QueueGroup").Build();
            ITrigger triggerQueuing = TriggerBuilder.Create()
                .WithIdentity("QueueTrigger", "QueueGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(jobQueuing, triggerQueuing);
            */
            //-----------------------------------Top Influencers Notification -------------------------------------------------------------------------------
            IJobDetail jobNotification = JobBuilder.Create<TopInfluencerNotification>().WithIdentity("TopInfluencerNotification", "InfluencersGroup").Build();
            ITrigger triggerNotification = TriggerBuilder.Create()
                .WithIdentity("InfluencersTrigger", "InfluencersGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(jobNotification, triggerNotification);

            //-----------------------------------Post Sentiment------------------------------------------------------------------------------
            IJobDetail jobSentiment = JobBuilder.Create<Sentiments>().WithIdentity("Sentiments", "SentimentsGroup").Build();
            ITrigger triggerSentiment = TriggerBuilder.Create()
                .WithIdentity("SentimentsTrigger", "SentimentsGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(jobSentiment, triggerSentiment);

            //-----------------------------------Posts Grading -------------------------------------------------------------------------------            
            IJobDetail jobGrading = JobBuilder.Create<Grading>().WithIdentity("Grading", "GradingGroup").Build();
            ITrigger triggerGrading = TriggerBuilder.Create()
                .WithIdentity("GradingTrigger", "GradingGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(jobGrading, triggerGrading);

        }
    }
}