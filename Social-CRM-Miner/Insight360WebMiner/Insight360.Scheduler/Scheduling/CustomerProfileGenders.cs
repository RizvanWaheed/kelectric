using Insight360.Common;
using Insight360.DAL;
using Insight360.Scheduling;
using Insight360.Util;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class CustomerProfileGenders : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    DateTimeOffset? previousTime = context.Trigger.GetPreviousFireTimeUtc();
                    var profiles = CommonDBOperations.GetCustomerSocialProfiles().ToList();
                    if (profiles != null && profiles.Count > 0)
                    {
                        profiles.ForEach(delegate (CustomerSocialProfile profile)
                        {
                            string gender = Helpers.DetectGender(profile.Name).ToLower();
                            if (gender != "male" && gender != "female")
                            {
                                gender = "unspecified";
                            }
                            profile.Gender = gender;
                        });

                        CommonDBOperations.UpdateCustomerSocialProfile(profiles);
                    }
                }
                catch (Exception ex)
                { }
            }
        }
    }
}