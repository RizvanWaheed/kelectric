using CRM.Common;
using CRM.Common.Enum;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Models.Report;
using SocialCRM.Web.Models.TaskManager;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SocialCRM.Web.Controllers.API
{
    [RoutePrefix("api/Report")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReportController : ApiController
    {
        IReportRepository reportRepository;
        ITaskRepository taskRepository;
        IProfileRepository proRepository;
        public ReportController(IReportRepository repo, ITaskRepository repo2, IProfileRepository repo3)
        {
            reportRepository = repo;
            taskRepository = repo2;
            proRepository = repo3;
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetTaskPerformanceReport(TaskPerformanceReportFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = await reportRepository.GetPerformanceReport(userId, model.groupId, model.companyId, model.dateFrom, model.dateTo, model.dateRange);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetTeamReport(TaskPerformanceReportFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = await reportRepository.GetTeamReport(userId, model.groupId, model.companyId, model.dateFrom, model.dateTo, model.dateRange, model.selectedTeamMembers, model.isCurrUserAdmin, model.currUserTeamId);
            return Ok(status);
        }

        [HttpPost]
        public IHttpActionResult GetTwitterProfilePostsAndConversation(FilterModel model)
        {
            var status = reportRepository.GetTwitterProfilePostsAndConversation(model);
            return Ok(status);
        }

        [HttpPost]
        public IHttpActionResult CreateReportPDF(ReportModel model)
        {
            var status = CreateReportFromReportModel(model);
            return Ok(status);
        }

        //PDF Generator
        bool CreateReportFromReportModel(ReportModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<strong>Hello</strong>");
            ExportPDF("TwitterReport", sb.ToString());

            return false;
        }

        public static void ExportPDF(string FileName, string html)
        {
            string filename = FileName + ".pdf";
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            HttpResponse Response = System.Web.HttpContext.Current.Response;
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");

            System.IO.StringReader sr = new System.IO.StringReader(html + tw.ToString());
            var pageSize = new Rectangle(421, 596);
            Document pdfDoc = new Document(pageSize, 7f, 7f, 7f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);

            pdfDoc.Close();
            System.Web.HttpContext.Current.Response.Write(pdfDoc);
            System.Web.HttpContext.Current.Response.End();
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetCommentAndChatByTeam(TeamReportFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DateTime? fromDate = null, toDate = null;

            if ((!String.IsNullOrEmpty(model.dateFrom)))
                fromDate = DateTime.ParseExact(model.dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if ((!String.IsNullOrEmpty(model.dateTo)))
                toDate = DateTime.ParseExact(model.dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var status = await reportRepository.GetCommentAndChatByTeam(model.teamId, model.profileType, model.activityType, model.profileId, fromDate, toDate, model.dateRange);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetQuickOrSlowReplyByTeam(TeamReportReplyFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DateTime? fromDate = null, toDate = null;

            if ((!String.IsNullOrEmpty(model.dateFrom)))
                fromDate = DateTime.ParseExact(model.dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if ((!String.IsNullOrEmpty(model.dateTo)))
                toDate = DateTime.ParseExact(model.dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var status = await reportRepository.GetQuickOrSlowReplyByTeam(model.teamId, model.profileType, model.profileId, model.replyType, fromDate, toDate, model.dateRange);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetInOutBoundReport(InOutBoundFilterModel model)
        {
            model.dateFrom = model.dateFrom.Date;
            model.dateTo = model.dateTo.Date.Add(new TimeSpan(23, 59, 59));
            var userId = 0;
            if (model.userId.HasValue)
                userId = model.userId.Value;
            else
                userId = User.Identity.GetUserId<int>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            InOutBoundReportContainer retObj = new InOutBoundReportContainer();

            string report = model.report, reportType = model.reportType;
            var status = reportRepository.GetInOutBoundReport(model);

            InOutBoundStats inOutStats = new InOutBoundStats();
            inOutStats.InboundSum = status.Where(x => x.Volume == "Inbound").Count();
            inOutStats.OutboundSum = status.Where(x => x.Volume == "Outbound").Count();

            inOutStats.Answered = status.Where(x => x.TaskCompleted.HasValue && x.TaskCompleted.Value == 1).Count();
            inOutStats.UnAnswered = status.Count() - inOutStats.Answered;

            List<InOutBoundReport> result = new List<InOutBoundReport>();
            List<InOutBoundReport> result2 = new List<InOutBoundReport>();
            List<InOutBoundChannelReport> channelsReport = new List<InOutBoundChannelReport>();

            List<SentimentsPerChannel> sentimentsReport = new List<SentimentsPerChannel>();
            List<TagsPerChannel> tagsReport = new List<TagsPerChannel>();

            List<BrandKeywordViewModel> tagsTemp = await taskRepository.GetBrandKeywordByCompanyId(model.companyId);
            List<BrandKeywordViewModel> tags = new List<BrandKeywordViewModel>();

            List<ProfileModel> profilesTemp = await proRepository.GetProfilesByGroupIdForUnifiedInbox(userId, model.groupId, model.companyId); //await proRepository.GetProfilesByCompanyId(model.companyId);
            List<ProfileModel> profiles = new List<ProfileModel>();

            if (model.tagIds != "0")
            {
                var tagIdsList = model.tagIds.Split(',').ToList();
                foreach (var t in tagsTemp)
                {
                    if (tagIdsList.Contains(t.Id.ToString()))
                    {
                        tags.Add(t);
                    }
                }
            }
            else
            {
                tags = tagsTemp;
            }
            // profiles
            if (model.profileIds != "0")
            {
                var profileIdsList = model.profileIds.Split(',').ToList();
                foreach (var p in profilesTemp)
                {
                    if (profileIdsList.Contains(p.ProfileId.ToString()) && !p.IsPage)
                    {
                        profiles.Add(p);
                    }
                }
            }

            if (model.profilePageIds != "0")
            {
                var profilePageIdsList = model.profilePageIds.Split(',').ToList();
                foreach (var p in profilesTemp)
                {
                    if (profilePageIdsList.Contains(p.ProfileId.ToString()) && p.IsPage)
                    {
                        profiles.Add(p);
                    }
                }
            }

            if (model.profilePageIds == "0" && model.profileIds == "0")
            {
                profiles = profilesTemp;
            }

            if (report == "InOutBound")
            {
                if (reportType == "date")
                {
                    result = status.GroupBy(p => new { Date = p.CreatedDate.Date, p.Volume })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = g.Key.Volume,
                                                Date = g.Key.Date,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Date).ToList();
                }
                else if (reportType == "day")
                {
                    result = status.GroupBy(p => new { p.CreatedDate.DayOfWeek, p.Volume })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = g.Key.Volume,
                                                Day = g.Key.DayOfWeek.ToString(),
                                                Count = g.Count()
                                            }).OrderBy(x => x.Day).ToList();
                }
                else if (reportType == "hour")
                {
                    result = status.GroupBy(p => new { p.CreatedDate.Hour, p.Volume })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = g.Key.Volume,
                                                Hour = g.Key.Hour,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Hour).ToList();
                }
                else if (reportType == "month")
                {
                    result = status.GroupBy(p => new { p.CreatedDate.Month, p.Volume })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = g.Key.Volume,
                                                Month = g.Key.Month,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Hour).ToList();
                }
            }
            else if (report == "Sentiment")
            {
                if (reportType == "date")
                {
                    result = status.Where(x => x.Sentiment != null && x.Sentiment != 4 && x.Volume == "Inbound").GroupBy(p => new { Date = p.CreatedDate.Date, p.Sentiment })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = GetSentiment(g.Key.Sentiment),
                                                Date = g.Key.Date,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Date).ToList();
                }
                else if (reportType == "day")
                {
                    result = status.Where(x => x.Sentiment != null && x.Sentiment != 4 && x.Volume == "Inbound").GroupBy(p => new { p.CreatedDate.DayOfWeek, p.Sentiment })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = GetSentiment(g.Key.Sentiment),
                                                Day = g.Key.DayOfWeek.ToString(),
                                                Count = g.Count()
                                            }).OrderBy(x => x.Day).ToList();
                }
                else if (reportType == "hour")
                {
                    result = status.Where(x => x.Sentiment != null && x.Sentiment != 4 && x.Volume == "Inbound").GroupBy(p => new { p.CreatedDate.Hour, p.Sentiment })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = GetSentiment(g.Key.Sentiment),
                                                Hour = g.Key.Hour,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Hour).ToList();
                }
                else if (reportType == "month")
                {
                    result = status.Where(x => x.Sentiment != null && x.Sentiment != 4 && x.Volume == "Inbound").GroupBy(p => new { p.CreatedDate.Month, p.Sentiment })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = GetSentiment(g.Key.Sentiment),
                                                Month = g.Key.Month,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Hour).ToList();
                }
            }
            else if (report == "Tag")
            {
                //List<BrandKeywordViewModel> tagsTemp = await taskRepository.GetBrandKeywordByCompanyId(model.companyId);
                //List<BrandKeywordViewModel> tags = new List<BrandKeywordViewModel>();
                //if (model.tagIds != "0")
                //{
                //    var tagIdsList = model.tagIds.Split(',').ToList();
                //    foreach (var t in tagsTemp)
                //    {
                //        if (tagIdsList.Contains(t.Id.ToString()))
                //        {
                //            tags.Add(t);
                //        }
                //    }
                //}
                //else
                //{
                //    tags = tagsTemp;
                //}

                if (reportType == "date")
                {
                    foreach (var t in tags)
                    {
                        result.AddRange(status.Where(x => x.Tags != null && x.Volume == "Inbound" && x.Tags.Split(',').ToList().Contains(t.Id.ToString())).GroupBy(p => new { Date = p.CreatedDate.Date }).Select(
                            g => new InOutBoundReport
                            {
                                Name = t.KeywordText,
                                Date = g.Key.Date,
                                Count = g.Count()
                            }).ToList());
                    }
                    result = result.OrderBy(x => x.Date).ToList();
                }
                else if (reportType == "day")
                {
                    foreach (var t in tags)
                    {
                        result.AddRange(status.Where(x => x.Tags != null && x.Volume == "Inbound" && x.Tags.Split(',').ToList().Contains(t.Id.ToString())).GroupBy(p => new { Date = p.CreatedDate.DayOfWeek }).Select(
                            g => new InOutBoundReport
                            {
                                Name = t.KeywordText,
                                Day = g.Key.Date.ToString(),
                                Count = g.Count()
                            }).ToList());
                    }
                    result = result.OrderBy(x => x.Date).ToList();
                }
                else if (reportType == "hour")
                {
                    foreach (var t in tags)
                    {
                        result.AddRange(status.Where(x => x.Tags != null && x.Volume == "Inbound" && x.Tags.Split(',').ToList().Contains(t.Id.ToString())).GroupBy(p => new { Date = p.CreatedDate.Hour }).Select(
                            g => new InOutBoundReport
                            {
                                Name = t.KeywordText,
                                Hour = g.Key.Date,
                                Count = g.Count()
                            }).ToList());
                    }
                    result = result.OrderBy(x => x.Date).ToList();
                }
                else if (reportType == "month")
                {
                    foreach (var t in tags)
                    {
                        result.AddRange(status.Where(x => x.Tags != null && x.Volume == "Inbound" && x.Tags.Split(',').ToList().Contains(t.Id.ToString())).GroupBy(p => new { Date = p.CreatedDate.Month }).Select(
                            g => new InOutBoundReport
                            {
                                Name = t.KeywordText,
                                Month = g.Key.Date,
                                Count = g.Count()
                            }).ToList());
                    }
                    result = result.OrderBy(x => x.Date).ToList();
                }
            }
            else if (report == "Channel")
            {
                if (reportType == "date")
                {
                    channelsReport = status.GroupBy(p => new { Date = p.CreatedDate.Date, p.ProfileId, p.ProfilePageId, p.Volume })
                                        .Select(
                                            g => new InOutBoundChannelReport
                                            {
                                                Name = (g.Key.ProfileId.HasValue ? "P|" + g.Key.ProfileId.Value.ToString() : "PP|" + g.Key.ProfilePageId.Value.ToString()),
                                                Date = g.Key.Date,
                                                Volume = g.Key.Volume,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Date).ToList();
                }
                else if (reportType == "day")
                {
                    channelsReport = status.GroupBy(p => new { p.CreatedDate.DayOfWeek, p.ProfileId, p.ProfilePageId, p.Volume })
                                        .Select(
                                            g => new InOutBoundChannelReport
                                            {
                                                Name = (g.Key.ProfileId.HasValue ? "P|" + g.Key.ProfileId.Value.ToString() : "PP|" + g.Key.ProfilePageId.Value.ToString()),
                                                Day = g.Key.DayOfWeek.ToString(),
                                                Volume = g.Key.Volume,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Day).ToList();
                }
                else if (reportType == "hour")
                {
                    channelsReport = status.GroupBy(p => new { p.CreatedDate.Hour, p.ProfileId, p.ProfilePageId, p.Volume })
                                        .Select(
                                            g => new InOutBoundChannelReport
                                            {
                                                Name = (g.Key.ProfileId.HasValue ? "P|" + g.Key.ProfileId.Value.ToString() : "PP|" + g.Key.ProfilePageId.Value.ToString()),
                                                Hour = g.Key.Hour,
                                                Volume = g.Key.Volume,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Hour).ToList();
                }
                else if (reportType == "month")
                {
                    channelsReport = status.GroupBy(p => new { p.CreatedDate.Month, p.ProfileId, p.ProfilePageId, p.Volume })
                                        .Select(
                                            g => new InOutBoundChannelReport
                                            {
                                                Name = (g.Key.ProfileId.HasValue ? "P|" + g.Key.ProfileId.Value.ToString() : "PP|" + g.Key.ProfilePageId.Value.ToString()),
                                                Month = g.Key.Month,
                                                Volume = g.Key.Volume,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Hour).ToList();
                }
            }

            //Crisis Report
            var status2 = status;
            CrisisReportDataModel crisis_report = reportRepository.CrisisReportData();

            if (crisis_report != null)
            {
                if (Convert.ToDateTime(crisis_report.LastOffTime) > DateTime.Now)
                {
                    model.dateFrom = Convert.ToDateTime(crisis_report.LastOnTime).AddHours(-4);
                    model.dateTo = Convert.ToDateTime(crisis_report.LastOffTime);
                    status2 = reportRepository.GetInOutBoundReport(model).Where(x => x.Volume == "Inbound").ToList();

                    result2 = status2.GroupBy(p => new { p.PostMedium, p.CreatedDate.Date, p.CreatedDate.Hour })
                                              .Select(
                                                       g => new InOutBoundReport
                                                       {
                                                           Name = g.Key.PostMedium,
                                                           Date = g.Key.Date,
                                                           Hour = g.Key.Hour,
                                                           Count = g.Count()
                                                       }).OrderBy(x => x.Date).ThenBy(y => y.Hour).ToList();

                    retObj.CrisisReport_Ontime = crisis_report.LastOnTime;
                    retObj.CrisisReport_Offtime = crisis_report.LastOffTime;
                }
                else
                {
                    //turn off Crisis Reporting after 12 ohurs
                    reportRepository.UpdateCrisisData(true);
                }
            }
            //////////////////

            try
            {
                foreach (var profile in profiles)
                {
                    if (profile.ProfileTypeId == 1 || profile.ProfileTypeId == 5)
                    {
                        profile.ProfileName = profile.ProfileName + "(FB)";
                    }
                    else if (profile.ProfileTypeId == 4)
                    {
                        profile.ProfileName = profile.ProfileName + "(TW)";
                    }
                    else if (profile.ProfileTypeId == 6)
                    {
                        profile.ProfileName = profile.ProfileName + "(IN)";
                    }

                    if (profile.IsPage)
                    {
                        sentimentsReport.Add(
                                    new SentimentsPerChannel()
                                    {
                                        Negative = status.Where(x => x.Sentiment == (int)SentimentType.Negative && x.Volume == "Inbound" && x.ProfilePageId.HasValue && x.ProfilePageId.Value == profile.ProfileId).Count(),
                                        Neutral = status.Where(x => x.Sentiment == (int)SentimentType.Neutral && x.Volume == "Inbound" && x.ProfilePageId.HasValue && x.ProfilePageId.Value == profile.ProfileId).Count(),
                                        Positive = status.Where(x => x.Sentiment == (int)SentimentType.Positive && x.Volume == "Inbound" && x.ProfilePageId.HasValue && x.ProfilePageId.Value == profile.ProfileId).Count(),
                                        Profile = profile
                                    }
                                );
                        TagsPerChannel tc = new TagsPerChannel();
                        tc.Profile = profile;
                        tc.Tags = new List<string>();
                        tc.TagCounts = new List<int>();
                        foreach (var t in tags)
                        {
                            tc.TagCounts.Add(status.Where(x => x.Tags != null && x.Volume == "Inbound" && x.Tags.Split(',').ToList().Contains(t.Id.ToString()) && x.ProfilePageId.HasValue && x.ProfilePageId.Value == profile.ProfileId).Count());
                            tc.Tags.Add(t.KeywordName);
                        }
                        tagsReport.Add(tc);
                    }
                    else
                    {
                        sentimentsReport.Add(
                                    new SentimentsPerChannel()
                                    {
                                        Negative = status.Where(x => x.Sentiment == (int)SentimentType.Negative && x.Volume == "Inbound" && x.ProfileId == profile.ProfileId).Count(),
                                        Neutral = status.Where(x => x.Sentiment == (int)SentimentType.Neutral && x.Volume == "Inbound" && x.ProfileId == profile.ProfileId).Count(),
                                        Positive = status.Where(x => x.Sentiment == (int)SentimentType.Positive && x.Volume == "Inbound" && x.ProfileId == profile.ProfileId).Count(),
                                        Profile = profile
                                    }
                                );
                        TagsPerChannel tc = new TagsPerChannel();
                        tc.Profile = profile;
                        tc.Tags = new List<string>();
                        tc.TagCounts = new List<int>();
                        foreach (var ta in tags)
                        {
                            tc.TagCounts.Add(status.Where(x => x.Tags != null && x.Volume == "Inbound" && x.Tags.Split(',').ToList().Contains(ta.Id.ToString()) && x.ProfileId == profile.ProfileId).Count());
                            tc.Tags.Add(ta.KeywordName);
                        }
                        tagsReport.Add(tc);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            retObj.InOutStats = inOutStats;
            retObj.InOutBoundReport = result;
            retObj.CrisisReport = result2;
            retObj.SentimentsPerChannel = sentimentsReport;
            retObj.TagsPerChannel = tagsReport;
            retObj.ChannelReport = channelsReport;

            return Ok(retObj);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetShiftReport(ShiftFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ShiftReport retObj = new ShiftReport();
            retObj.TagOccourances = new List<TagOccurance>();

            try
            {
                DateTime from = model.date, to = model.date;
                TimeSpan shiftFrom = new TimeSpan(), shiftTo = new TimeSpan();

                List<ProfileModel> profiles = await proRepository.GetProfilesByGroupIdForUnifiedInbox(userId, model.groupId, model.companyId);
                List<int> profileIds = profiles.Where(x => (x.ProfileType == ProfilesType.Twitter || x.ProfileType == ProfilesType.Facebook) && !x.IsPage).Select(x => x.ProfileId).ToList();
                List<int> profilePageIds = profiles.Where(x => x.IsPage).Select(x => x.ProfileId).ToList();

                List<BrandKeywordViewModel> tags = await taskRepository.GetBrandKeywordByCompanyId(model.companyId);
                //if (model.tagIds.Count > 0)
                //    tags = tags.ToList();
                //else
                //    tags = new List<BrandKeywordViewModel>();
                List<string> tagNames = new List<string>();
                List<int> tagIds = new List<int>();
                List<int> tagOccourances = new List<int>();

                if (model.shiftId == (int)Shifts.Morning) // 8am - 4pm
                {
                    model.date = model.date.Date;
                    from = model.date.AddHours(8);
                    to = model.date.AddHours(8 + 8).AddSeconds(-1);

                    shiftFrom = new TimeSpan(8, 0, 0);
                    shiftTo = new TimeSpan(15, 59, 59);
                }
                else if (model.shiftId == (int)Shifts.Evening) //4pm - 12AM
                {
                    model.date = model.date.Date;
                    from = model.date.AddHours(16);
                    to = model.date.AddHours(16 + 8).AddSeconds(-1);

                    shiftFrom = new TimeSpan(16, 0, 0);
                    shiftTo = new TimeSpan(23, 59, 59);
                }
                else if (model.shiftId == (int)Shifts.Night) //12am - 8am
                {
                    model.date = model.date.Date;
                    from = model.date.AddHours(0);
                    to = model.date.AddHours(8).AddSeconds(-1);

                    shiftFrom = new TimeSpan(00, 0, 0);
                    shiftTo = new TimeSpan(7, 59, 59);
                }

                InOutBoundFilterModel obj = new InOutBoundFilterModel()
                {
                    companyId = model.companyId,
                    groupId = model.groupId,
                    dateFrom = from.AddDays(-6).Date,
                    dateTo = to.Date.AddHours(24),
                    profileIds = "0",
                    profilePageIds = "0",
                    msgIds = "0",
                    sentimentIds = "0",
                    tagIds = "0"
                };
                var status = reportRepository.GetInOutBoundReport(obj);
                var status2 = status.Where(x => ((x.ProfileId.HasValue && profileIds.Contains(x.ProfileId.Value)) || (x.ProfilePageId.HasValue && profilePageIds.Contains(x.ProfilePageId.Value))) && x.Volume == "Inbound").ToList();
                status = status2.Where(x => x.CreatedDate >= from && x.CreatedDate <= to).ToList();

                retObj.TotalInboundTraffic = status.Count;

                List<ShiftWeekData> WeekData = new List<ShiftWeekData>();
                from = from.AddDays(-6);

                for (int i = 0; i < 7; i++)
                {
                    var temp = new ShiftWeekData();
                    var tempDate = from.AddDays(i);
                    temp.Date = from.AddDays(i).ToString("dd MMM");
                    temp.TwData = status2.Where(x => (x.PostType == "TP" || x.PostType == "TM") && x.CreatedDate.Date == tempDate.Date && x.CreatedDate.TimeOfDay >= shiftFrom && x.CreatedDate.TimeOfDay <= shiftTo).Count();
                    temp.FbData = status2.Where(x => (x.PostType == "FP" || x.PostType == "FCP" || x.PostType == "FC") && x.CreatedDate.Date == tempDate.Date && x.CreatedDate.TimeOfDay >= shiftFrom && x.CreatedDate.TimeOfDay <= shiftTo).Count();
                    WeekData.Add(temp);
                }

                retObj.WeekData = WeekData;

                int daysInMonth = DateTime.DaysInMonth(model.date.Year, model.date.Month);
                InOutBoundFilterModel obj3 = new InOutBoundFilterModel()
                {
                    companyId = model.companyId,
                    groupId = model.groupId,
                    dateFrom = new DateTime(model.date.Year - 1, model.date.Month, 1, 0, 0, 0),
                    dateTo = new DateTime(model.date.Year - 1, model.date.Month, daysInMonth, 23, 59, 59),
                    profileIds = "0",
                    profilePageIds = "0",
                    msgIds = "0",
                    sentimentIds = "0",
                    tagIds = "0"
                };

                var status3 = reportRepository.GetInOutBoundReport(obj3);
                retObj.FbAverage = status3.Where(x => ((x.ProfileId.HasValue && profileIds.Contains(x.ProfileId.Value)) || (x.ProfilePageId.HasValue && profilePageIds.Contains(x.ProfilePageId.Value))) && ((x.PostType == "FP" || x.PostType == "FCP" || x.PostType == "FC")) && x.Volume == "Inbound" && x.CreatedDate.TimeOfDay >= shiftFrom && x.CreatedDate.TimeOfDay <= shiftTo).Count();
                retObj.TwAverage = status3.Where(x => ((x.ProfileId.HasValue && profileIds.Contains(x.ProfileId.Value)) || (x.ProfilePageId.HasValue && profilePageIds.Contains(x.ProfilePageId.Value))) && ((x.PostType == "TP" || x.PostType == "TM")) && x.Volume == "Inbound" && x.CreatedDate.TimeOfDay >= shiftFrom && x.CreatedDate.TimeOfDay <= shiftTo).Count();

                if (retObj.FbAverage != 0)
                    retObj.FbAverage = retObj.FbAverage / daysInMonth;
                if (retObj.TwAverage != 0)
                    retObj.TwAverage = retObj.TwAverage / daysInMonth;

                foreach (var tag in tags)
                {
                    retObj.TagOccourances.Add(new TagOccurance() { Id = tag.Id, Name = tag.KeywordName, Count = status.Where(x => x.Tags != null && x.Tags.Split(',').ToList().Contains(tag.Id.ToString())).Count() });
                }

                InOutBoundFilterModel obj2 = new InOutBoundFilterModel()
                {
                    companyId = model.companyId,
                    groupId = model.groupId,
                    dateFrom = from.AddDays(-6).Date,
                    dateTo = to.Date.AddHours(24),
                    profileIds = "0",
                    profilePageIds = "0",
                    sentimentIds = "0",
                    tagIds = "0"
                };

                var onDutyAgents = reportRepository.GetOnDutyAgents(model.companyId, model.date.Date, shiftFrom, shiftTo);
                retObj.OnDutyAgents = "";
                for (int i = 0; i < onDutyAgents.Count; i++)
                {
                    retObj.OnDutyAgents += onDutyAgents[i].FirstName + " " + onDutyAgents[i].LastName + ", ";
                }
                if (retObj.OnDutyAgents.Length > 2)
                    retObj.OnDutyAgents = retObj.OnDutyAgents.Substring(0, retObj.OnDutyAgents.Length - 2);
            }
            catch (Exception ex)
            {
            }

            return Ok(retObj);
        }

        [HttpPost]
        public async Task<IHttpActionResult> EmailShiftReport(EmailShiftReportModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string serverFileName = Guid.NewGuid().ToString() + ".png";
                string path = ConfigurationManager.AppSettings["ReportPath"].ToString();
                string serverPath = string.Concat(path + serverFileName);
                string filePath = HttpContext.Current.Server.MapPath(serverPath);
                while (System.IO.File.Exists(filePath))
                {
                    serverFileName = Guid.NewGuid().ToString() + ".png";
                    serverPath = string.Concat(path + serverFileName);
                    filePath = HttpContext.Current.Server.MapPath(serverPath);
                }

                if (model.shiftReportImage.ToLower().Contains("base64"))
                {
                    var bytes = Convert.FromBase64String(model.shiftReportImage.Substring(model.shiftReportImage.IndexOf("base64,") + 7));

                    try
                    {
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            System.Drawing.Image.FromStream(ms).Save(filePath);
                            using (System.Drawing.Bitmap bm2 = new System.Drawing.Bitmap(ms))
                            {
                                bm2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                System.Drawing.Image.FromStream(ms).Save(filePath);
                            }
                        }
                    }
                    catch (Exception ex) { }
                }

                List<string> filePaths = new List<string>();
                filePaths.Add(filePath);
                List<string> contentIds = new List<string>();
                contentIds.Add("shiftReportContentId");

                EmailHandler email = new EmailHandler();
                string emailsStr = "";
                var emails = reportRepository.GetEmails(model.companyId);
                if (emails.Count == 0)
                {
                    return Ok(new { success = false, msg = "No recievers found." });
                }
                else
                {
                    emailsStr = string.Join(";", emails);
                }

                if (model.onDutyAgents == "" || model.onDutyAgents == null)
                    model.onDutyAgents = "None";

                string emailTemplate = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/EmailTemplates/ShiftReport.html"));
                string tagsTemplate = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/EmailTemplates/_ShiftReportTag.html"));
                string tagsTemplateCompiled = "";
                string weekDayTemplate = "<td style='text-align:center; border-bottom:solid 3px #bdd6ee; border-right:solid 1px #bdd6ee;'>##WeekDay##</td>";
                string TwWeekDayTemplate = "<td style='text-align:center; border-bottom:solid 1px #bdd6ee; border-right:solid 1px #bdd6ee;'>##TwWeekDay##</td>";
                string FbWeekDayTemplate = "<td style='text-align:center; border-bottom:solid 1px #bdd6ee; border-right:solid 1px #bdd6ee;'>##FbWeekDay##</td>";
                string TotalWeekDayTemplate = "<td style='text-align:center'>##TotalWeekDay##</td>";
                string weekDayTemplateCompiled = "", TwWeekDayTemplateCompiled = "", FbWeekDayTemplateCompiled = "", TotalWeekDayTemplateCompiled = "";

                for (var i = 0; i < model.tagOccurances.Count; i++)
                {
                    string temp = tagsTemplate;
                    temp = temp.Replace("##TotalInboundTraffic##", model.totalInboundTraffic.ToString());
                    temp = temp.Replace("##TagName##", model.tagOccurances[i].Name);
                    temp = temp.Replace("##TagCount##", model.tagOccurances[i].Count.ToString());

                    tagsTemplateCompiled += temp;
                }

                for (var i = model.WeekData.Count - 1; i >= 0; i--)
                {
                    string temp = weekDayTemplate;
                    string temp2 = TwWeekDayTemplate;
                    string temp3 = FbWeekDayTemplate;
                    string temp4 = TotalWeekDayTemplate;
                    temp = temp.Replace("##WeekDay##", model.WeekData[i].Date);
                    temp2 = temp2.Replace("##TwWeekDay##", model.WeekData[i].TwData.ToString());
                    temp3 = temp3.Replace("##FbWeekDay##", model.WeekData[i].FbData.ToString());
                    temp4 = temp4.Replace("##TotalWeekDay##", (model.WeekData[i].FbData + model.WeekData[i].TwData).ToString());

                    weekDayTemplateCompiled += temp;
                    TwWeekDayTemplateCompiled += temp2;
                    FbWeekDayTemplateCompiled += temp3;
                    TotalWeekDayTemplateCompiled += temp4;
                }

                emailTemplate = emailTemplate.Replace("##ShiftTiming##", model.shiftTiming);
                emailTemplate = emailTemplate.Replace("##SelectedDate##", model.date.ToString("dd MMMM yyyy"));
                emailTemplate = emailTemplate.Replace("##TotalInboundTraffic##", model.totalInboundTraffic.ToString());
                emailTemplate = emailTemplate.Replace("##TopThreeAreas##", model.top3Areas);
                emailTemplate = emailTemplate.Replace("##OnDutyAgents##", model.onDutyAgents);
                emailTemplate = emailTemplate.Replace("##TAGS##", tagsTemplateCompiled);
                emailTemplate = emailTemplate.Replace("##WeekDays##", weekDayTemplateCompiled);
                emailTemplate = emailTemplate.Replace("##TwWeekDays##", TwWeekDayTemplateCompiled);
                emailTemplate = emailTemplate.Replace("##FbWeekDays##", FbWeekDayTemplateCompiled);
                emailTemplate = emailTemplate.Replace("##TotalWeekDays##", TotalWeekDayTemplateCompiled);

                email.SendEmail(emailsStr, "KE Social Care - Shift Report: " + model.shift + " (" + model.date.ToString("dd MMMM yyyy") + ")", email.getEmbeddedImage(filePaths.ToArray(), emailTemplate, contentIds.ToArray()));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, msg = ex.ToString() });
            }
            return Ok(new { success = true, msg = "Email sent successfully." });
        }

        [HttpGet]
        [Route("UpdateCrisisData/{isCrisis}")]
        public async Task<IHttpActionResult> UpdateCrisisData(bool isCrisis)
        {
            string message = "";

            try
            {
                bool status = reportRepository.UpdateCrisisData(isCrisis);
                if (isCrisis)
                {
                    message = "Crisis Reporting is turned off.";
                }
                else
                {
                    message = "Crisis Reporting is turned on.";
                }

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, msg = ex.ToString() });
            }
            return Ok(new { success = true, msg = message });
        }


        [HttpPost]
        public async Task<IHttpActionResult> EmailCrisisReport(EmailCrisisReportModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string serverFileName = Guid.NewGuid().ToString() + ".png";
                string path = ConfigurationManager.AppSettings["ReportPath"].ToString();
                string serverPath = string.Concat(path + serverFileName);
                string filePath = HttpContext.Current.Server.MapPath(serverPath);
                while (System.IO.File.Exists(filePath))
                {
                    serverFileName = Guid.NewGuid().ToString() + ".png";
                    serverPath = string.Concat(path + serverFileName);
                    filePath = HttpContext.Current.Server.MapPath(serverPath);
                }

                if (model.crisisReportImage.ToLower().Contains("base64"))
                {
                    var bytes = Convert.FromBase64String(model.crisisReportImage.Substring(model.crisisReportImage.IndexOf("base64,") + 7));

                    try
                    {
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            System.Drawing.Image.FromStream(ms).Save(filePath);
                            using (System.Drawing.Bitmap bm2 = new System.Drawing.Bitmap(ms))
                            {
                                bm2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                System.Drawing.Image.FromStream(ms).Save(filePath);
                            }
                        }
                    }
                    catch (Exception ex) { }
                }

                List<string> filePaths = new List<string>();
                filePaths.Add(filePath);
                List<string> contentIds = new List<string>();
                contentIds.Add("crisisReportContentId");

                EmailHandler email = new EmailHandler();
                string emailsStr = "";
                var emails = reportRepository.GetEmails(model.companyId);
                if (emails.Count == 0)
                {
                    return Ok(new { success = false, msg = "No recievers found." });
                }
                else
                {
                    emailsStr = string.Join(";", emails);
                }

                string emailTemplate = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/EmailTemplates/CrisisReport.html"));
                string HourTemplate = "<td style='text-align:center; border-bottom:solid 3px #bdd6ee; border-right:solid 1px #bdd6ee;'>##Hours##</td>";
                string TWTemplate = "<td style='text-align:center; border-bottom:solid 1px #bdd6ee; border-right:solid 1px #bdd6ee;'>##TWCrisisData##</td>";
                string FBTemplate = "<td style='text-align:center; border-bottom:solid 1px #bdd6ee; border-right:solid 1px #bdd6ee;'>##FBCrisisData##</td>";
                string TotalTemplate = "<td style='text-align:center'>##TotalCrisisData##</td>";
                string HourTemplateCompiled = "", TWTemplateCompiled = "", FBTemplateCompiled = "", TotalTemplateCompiled = "";

                for (var i = 0; i < model.crisisTotalInboundData.Count; i++)
                {
                    string temp = HourTemplate;
                    string temp2 = TotalTemplate;
                    temp = temp.Replace("##Hours##", model.crisisTotalInboundData[i].hour.ToString() + ":00<br/>hrs");
                    temp2 = temp2.Replace("##TotalCrisisData##", model.crisisTotalInboundData[i].y.ToString());

                    HourTemplateCompiled += temp;
                    TotalTemplateCompiled += temp2;
                }

                if (model.crisisFBInboundData != null)
                {
                    for (var i = 0; i < model.crisisFBInboundData.Count; i++)
                    {
                        string temp = FBTemplate;
                        temp = temp.Replace("##FBCrisisData##", model.crisisFBInboundData[i].y.ToString());

                        FBTemplateCompiled += temp;
                    }
                }

                if (model.crisisTWInboundData != null)
                {
                    for (var i = 0; i < model.crisisTWInboundData.Count; i++)
                    {
                        string temp = TWTemplate;
                        temp = temp.Replace("##TWCrisisData##", model.crisisTWInboundData[i].y.ToString());

                        TWTemplateCompiled += temp;
                    }
                }

                emailTemplate = emailTemplate.Replace("##ReportOnTime##", model.reportOnTime);
                emailTemplate = emailTemplate.Replace("##TopThreeAreas##", model.top3Areas);
                emailTemplate = emailTemplate.Replace("##Hours##", HourTemplateCompiled);
                emailTemplate = emailTemplate.Replace("##FBCrisisData##", FBTemplateCompiled);
                emailTemplate = emailTemplate.Replace("##TWCrisisData##", TWTemplateCompiled);
                emailTemplate = emailTemplate.Replace("##TotalCrisisData##", TotalTemplateCompiled);

                email.SendEmail(emailsStr, "Crisis Report: " + DateTime.Now.Date.ToString("dd MMMM yyyy"), email.getEmbeddedImage(filePaths.ToArray(), emailTemplate, contentIds.ToArray()));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, msg = ex.ToString() });
            }
            return Ok(new { success = true, msg = "Email sent successfully." });
        }

        private string GetSentiment(int? sentiment)
        {
            switch (sentiment)
            {
                case (int)SentimentType.Positive:
                    return SentimentType.Positive.ToString();
                case (int)SentimentType.Neutral:
                    return SentimentType.Neutral.ToString();
                case (int)SentimentType.Negative:
                    return SentimentType.Negative.ToString();
                default:
                    return "Unknown";
            }
        }
    }
}