using CRM.Common.Enum;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using SocialCRM.Web.Models.GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.GoogleAnalytics
{
    public class AnalyticsHelper
    {
        private AnalyticsService _service;
        public AnalyticsHelper(UserCredential cred)
        {
            _service = new AnalyticsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred,
                ApplicationName = "SocialCRM Web"
            });
        }
        public AnalyticsHelper(UserCredential cred, bool isNew)
        {
            _service = new AnalyticsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred,
                ApplicationName = "SocialCRM Web"
            });
            if (isNew)
            {

            }
        }
        public AnalyticsHelper()
        {

        }
        public List<AnalyticsModel> GetAnalyticDetails()
        {
            List<AnalyticsModel> result = null;
            try
            {
                var profiles = this._service.Management.Profiles.List("~all", "~all").Execute();
                result = profiles.Items.Select
                    (
                    p => new AnalyticsModel
                    {
                        AccountId = Convert.ToInt64(p.AccountId),
                        AccountName = p.Name,
                        AccountUrl = p.WebsiteUrl,
                        IsConnected = false,
                        ProfileId=p.Id
                    }
                    ).ToList();
            }
            catch (Exception exp)
            {
                var exps = exp;
            }
            return result;
        }
        public List<string> GetAnalyticsLists()
        {
            var response = new List<string>();
            try
            {
                var profiles = this._service.Management.Profiles.List("~all", "~all").Execute();
                foreach (var p in profiles.Items)
                {

                    response.Add((string.Concat("Account ID : ", p.AccountId, " Name : ", p.Name, " Tracking Id : ", p.WebPropertyId, " Default Page : ", p.DefaultPage)));
                    var r = this._service.Data.Ga.Get(string.Format("ga:{0}", p.Id), "2016-01-01", "2016-02-22", "ga:visits,ga:bounces,ga:bounceRate,ga:users,ga:newUsers,ga:avgTimeOnSite");
                    var d = r.Execute();
                    response.Add(string.Concat("Visits : ", d.Rows[0][0], " Bounces : ", d.Rows[0][1], " BounceRate : ", d.Rows[0][2], " Users : ", d.Rows[0][3], " New Users : ", d.Rows[0][4]));
                }
            }
            catch (Exception exp)
            {
                var exps = exp;
            }
            return response;
        }
        public AnalyticsResultModel GetAnalyticsByAccountId(CRM.Entities.UserAnalyticAccount analyticAccounts,DateFilter dateFilter)
        {
            AnalyticsResultModel response = null;
            string startDate, endDate;
            try
            {
                switch (dateFilter)
                {
                    case DateFilter.Last15Days:
                        startDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last30Days:
                        startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last60Days:
                        startDate = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last90Days:
                        startDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    default:
                        startDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;

                }

                var profiles = this._service.Management.Profiles.List(Convert.ToString(analyticAccounts.AccountId), "~all").Execute().Items.Where(x => x.Id == analyticAccounts.ProfileId).FirstOrDefault();
                if (profiles != null)
                {
                    var analyticData = this._service.Data.Ga.Get(string.Format("ga:{0}", profiles.Id), startDate, endDate, "ga:visits,ga:bounces,ga:bounceRate,ga:users,ga:newUsers,ga:avgTimeOnSite").Execute();
                    response = new AnalyticsResultModel { BounceRate = analyticData.Rows[0][2], Bounces = analyticData.Rows[0][1], Traffic = analyticData.Rows[0][0], Users = analyticData.Rows[0][3], Visits = analyticData.Rows[0][0] };
                }
            }
            catch (Exception exp)
            {
                var exps = exp;
            }
            return response;
        }
        public AnalyticChartModel GetAnalyticsChartsByAccountId(CRM.Entities.UserAnalyticAccount analyticAccounts, DateFilter dateFilter, string customDateFilter="")
        {
            AnalyticChartModel response = new AnalyticChartModel();
            string startDate, endDate;
            try
            {
                switch (dateFilter)
                {
                    case DateFilter.Last15Days:
                        startDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last30Days:
                        startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last60Days:
                        startDate = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last90Days:
                        startDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Custom:
                        var datelist = customDateFilter.Split('-');
                        if (datelist.Length != 2)
                            return response;
                        startDate = DateTime.ParseExact(datelist[0], "MM|dd|yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");//DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        endDate = DateTime.ParseExact(datelist[1], "MM|dd|yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");//datelist[1].Replace('|', '-');
                        break;
                    default:
                        startDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                }
                var profiles = this._service.Management.Profiles.List(Convert.ToString(analyticAccounts.AccountId), "~all").Execute().Items.Where(x => x.Id == analyticAccounts.ProfileId).FirstOrDefault();
                if (profiles != null)
                {
                    string formatString = "yyyyMMdd";
                    var analyticData = this._service.Data.Ga.Get(string.Format("ga:{0}", profiles.Id), startDate, endDate, "ga:sessions,ga:bounces,ga:bounceRate");
                    analyticData.Dimensions = "ga:date";
                    var resultset=analyticData.Execute();
                    response.AnalyticGraph = new List<GraphModel>();
                    var totalTrafic = resultset.TotalsForAllResults["ga:sessions"];
                    var totalBounceRate = resultset.TotalsForAllResults["ga:bounceRate"];
                    var totalBounces = resultset.TotalsForAllResults["ga:bounces"];
                    response.TotalWebsiteTraffic = string.IsNullOrEmpty(totalTrafic) ? 0 : Convert.ToInt32(totalTrafic);
                    decimal value=0;
                    Decimal.TryParse(totalBounceRate, out value);
                    response.BounceRate = value >= 100 ? value.ToString("0") : value == 0 ? "0" : value.ToString("0.##");
                    response.TotalBounces = string.IsNullOrEmpty(totalBounces) ? "" : totalBounces;
                    switch (dateFilter)
                    {
                        case DateFilter.Last15Days:
                            var filtered = resultset.Rows.Select((x, i) => new { Index = i, Value = x }).
                                GroupBy(x => x.Index / 3).ToList();
                            foreach (var item in filtered)
                            {
                                response.AnalyticGraph.Add(new GraphModel
                                {
                                    TrafficDate = string.Format("{0} - {1}", String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.First().Value[0], formatString, CultureInfo.InvariantCulture)), String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.Last().Value[0], formatString, CultureInfo.InvariantCulture))),
                                    StartDate = item.First().Value[0],
                                EndDate=item.Last().Value[0],WebsiteTraffic=item.Sum(x=>int.Parse(x.Value[1]))});
                            }
                            break;
                        case DateFilter.Last30Days:
                            var filtered1 = resultset.Rows.Select((x, i) => new { Index = i, Value = x }).
                                GroupBy(x => x.Index / 5).ToList();
                            foreach (var item in filtered1)
                            {
                                response.AnalyticGraph.Add(new GraphModel
                                {
                                    TrafficDate = string.Format("{0}-{1}", String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.First().Value[0], formatString, CultureInfo.InvariantCulture)), String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.Last().Value[0], formatString, CultureInfo.InvariantCulture))),
                                    StartDate = item.First().Value[0],
                                    EndDate = item.Last().Value[0],
                                    WebsiteTraffic = item.Sum(x => int.Parse(x.Value[1]))
                                });
                            }
                            break;
                        case DateFilter.Last60Days:
                        case DateFilter.Last90Days:
                        case DateFilter.Custom:
                            var filtered2 = resultset.Rows.Select((x, i) => new { Index = i, Value = x }).
                                GroupBy(x => x.Index /9).ToList();
                            foreach (var item in filtered2)
                            {
                                response.AnalyticGraph.Add(new GraphModel
                                {
                                    TrafficDate = string.Format("{0}-{1}", String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.First().Value[0], formatString, CultureInfo.InvariantCulture)), String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.Last().Value[0], formatString, CultureInfo.InvariantCulture))),
                                    StartDate = item.First().Value[0],
                                    EndDate = item.Last().Value[0],
                                    WebsiteTraffic = item.Sum(x => int.Parse(x.Value[1]))
                                });
                            }
                            break;
                    }
                }
            }
            catch (Exception exp)
            {
                var exps = exp;
            }
            return response;
        }
        public AnalyticChartModel GetAnalyticsChartsByAccountId(long AccountId,string profileId, DateFilter dateFilter)
        {
            AnalyticChartModel response = new AnalyticChartModel();
            string startDate, endDate;
            try
            {
                switch (dateFilter)
                {
                    case DateFilter.Last15Days:
                        startDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last30Days:
                        startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last60Days:
                        startDate = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case DateFilter.Last90Days:
                        startDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    default:
                        startDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                        endDate = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                }
                var profiles = this._service.Management.Profiles.List(Convert.ToString(AccountId), "~all").Execute().Items.Where(x => x.Id == profileId).FirstOrDefault();
                if (profiles != null)
                {
                    string formatString = "yyyyMMdd";
                    var analyticData = this._service.Data.Ga.Get(string.Format("ga:{0}", profiles.Id), startDate, endDate, "ga:sessions,ga:bounces,ga:bounceRate");
                    analyticData.Dimensions = "ga:date";
                    var resultset = analyticData.Execute();
                    response.AnalyticGraph = new List<GraphModel>();
                    var totalTrafic = resultset.TotalsForAllResults["ga:sessions"];
                    var totalBounceRate = resultset.TotalsForAllResults["ga:bounceRate"];
                    var totalBounces = resultset.TotalsForAllResults["ga:bounces"];
                    response.TotalWebsiteTraffic = string.IsNullOrEmpty(totalTrafic) ? 0 : Convert.ToInt32(totalTrafic);
                    decimal value = 0;
                    Decimal.TryParse(totalBounceRate, out value);
                    response.BounceRate = value >= 100 ? value.ToString("0") : value == 0 ? "0" : value.ToString("0.##");
                    response.TotalBounces = string.IsNullOrEmpty(totalBounces) ? "" : totalBounces;
                    switch (dateFilter)
                    {
                        case DateFilter.Last15Days:
                            var filtered = resultset.Rows.Select((x, i) => new { Index = i, Value = x }).
                                GroupBy(x => x.Index / 3).ToList();
                            foreach (var item in filtered)
                            {
                                response.AnalyticGraph.Add(new GraphModel
                                {
                                    TrafficDate = string.Format("{0} - {1}", String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.First().Value[0], formatString, CultureInfo.InvariantCulture)), String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.Last().Value[0], formatString, CultureInfo.InvariantCulture))),
                                    StartDate = item.First().Value[0],
                                    EndDate = item.Last().Value[0],
                                    WebsiteTraffic = item.Sum(x => int.Parse(x.Value[1]))
                                });
                            }
                            break;
                        case DateFilter.Last30Days:
                            var filtered1 = resultset.Rows.Select((x, i) => new { Index = i, Value = x }).
                                GroupBy(x => x.Index / 5).ToList();
                            foreach (var item in filtered1)
                            {
                                response.AnalyticGraph.Add(new GraphModel
                                {
                                    TrafficDate = string.Format("{0}-{1}", String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.First().Value[0], formatString, CultureInfo.InvariantCulture)), String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.Last().Value[0], formatString, CultureInfo.InvariantCulture))),
                                    StartDate = item.First().Value[0],
                                    EndDate = item.Last().Value[0],
                                    WebsiteTraffic = item.Sum(x => int.Parse(x.Value[1]))
                                });
                            }
                            break;
                        case DateFilter.Last60Days:
                        case DateFilter.Last90Days:
                            var filtered2 = resultset.Rows.Select((x, i) => new { Index = i, Value = x }).
                                GroupBy(x => x.Index / 9).ToList();
                            foreach (var item in filtered2)
                            {
                                response.AnalyticGraph.Add(new GraphModel
                                {
                                    TrafficDate = string.Format("{0}-{1}", String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.First().Value[0], formatString, CultureInfo.InvariantCulture)), String.Format("{0:MMMM dd, yyyy}", DateTime.ParseExact(item.Last().Value[0], formatString, CultureInfo.InvariantCulture))),
                                    StartDate = item.First().Value[0],
                                    EndDate = item.Last().Value[0],
                                    WebsiteTraffic = item.Sum(x => int.Parse(x.Value[1]))
                                });
                            }
                            break;
                    }
                }
            }
            catch (Exception exp)
            {
                var exps = exp;
            }
            return response;
        }
    }
}