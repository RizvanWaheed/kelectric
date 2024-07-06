using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using Insight360.Common.LanguageService;
using System.Net.Http;
using Insight360.BO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Insight360.Common;

namespace Insight360.Util
{
    public static class Helpers
    {

        public static string CalculateSha256Hash(string data)
        {
            SHA256 shaM = new SHA256Managed();
            byte[] result = shaM.ComputeHash(System.Text.UnicodeEncoding.Unicode.GetBytes(data));
            return BitConverter.ToString(result);
        }

        public static string SeparateWords(string combined)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < combined.Length; i++)
            {
                if (char.IsUpper(combined[i]))
                    sb.Append(' ');
                sb.Append(combined[i]);
            }

            return sb.ToString();
        }

        public static string CombineWords(string seperated)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < seperated.Length; i++)
            {
                if (seperated[i] != ' ')
                    sb.Append(seperated[i]);
            }

            return sb.ToString();
        }

        public static DataTable RemoveDimensionFullName(DataTable dt)
        {
            /*foreach(DataRow dc in dt)
            {
               
            }*/

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string str = dt.Columns[i].ColumnName;
                try
                {
                    Util.Log.WriteOperationStarted(typeof(Helpers), "RemoveDimensionFullName", "COLUMN NAME Before = " + str);
                    if (str.Contains('['))
                    {
                        str = str.Remove(0, str.LastIndexOf("[") + 1);
                        str = str.Remove(str.Length - 1);
                    }

                }
                catch
                {

                    //str = "Col"+(i+1);
                }

                Util.Log.WriteOperationStarted(typeof(Helpers), "RemoveDimensionFullName", "COLUMN NAME After = " + str);

                dt.Columns[i].ColumnName = str;


                /*
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    object obj = dt.Rows[j].ItemArray[i];
                    if (obj.GetType() == str.GetType())
                    {
                        try
                        {
                            str = (string)dt.Rows[j].ItemArray[i];
                            str = str.Remove(str.LastIndexOf("&["));
                            str = str.Remove(str.Length);
                            dt.Rows[j].ItemArray[i] = str;
                        }
                        catch
                        { 
                            //str = 
                        }
                        
                        
                    }    
                    //dt.Rows[j].get
                }
                 */
            }

            return dt;
        }

        public static string DetectLanguage(string text)
        {

            try
            {
                using (LanguageServiceClient languageClient = new LanguageServiceClient())
                {
                    var languageCode = languageClient.GetLanguage(text);
                    return CheckLanguageCode(languageCode);
                }
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(Helpers), "Language Text :: " + text + " Exception in Language Detection  :: ", ex);
                return "N/A";
            }
        }

        public static string CheckLanguageCode(string languageCode)
        {
            if (languageCode.ToLower() == "eng")
            {
                return "en";
            }
            else if (languageCode.ToLower() == "urdu")
            {
                return "ur";
            }
            else if (languageCode.ToLower() == "arabic")
            {
                return "ar";
            }
            else if (languageCode.ToLower() == "azerbaijani")
            {
                return "az";
            }
            else if (languageCode.ToLower() == "belarusian")
            {
                return "be";
            }
            else if (languageCode.ToLower() == "bulgarian")
            {
                return "bg";
            }
            else if (languageCode.ToLower() == "bengali")
            {
                return "bn";
            }
            else if (languageCode.ToLower() == "tibetan")
            {
                return "bo";
            }
            else if (languageCode.ToLower() == "caucasian")
            {
                return "ca";
            }
            else if (languageCode.ToLower() == "czech")
            {
                return "cs";
            }
            else if (languageCode.ToLower() == "welsh")
            {
                return "cy";
            }
            else if (languageCode.ToLower() == "dannish")
            {
                return "da";
            }
            else if (languageCode.ToLower() == "romanurdu" || languageCode.ToLower() == "roman")
            {
                return "rd";
            }
            else if (languageCode.ToLower() == "german")
            {
                return "de";
            }
            else if (languageCode.ToLower() == "greek")
            {
                return "el";
            }
            else if (languageCode.ToLower() == "french")
            {
                return "fr";
            }
            else if (languageCode.ToLower() == "spanish")
            {
                return "es";
            }

            return languageCode;
        }

        public static int SentimentScore(string text)
        {
            try
            {
                string result = "";
                HttpResponseMessage response;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(AppSettings.SentimentAPI_URL);
                    var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("txt", text) });

                    response = client.PostAsync("/api/text/", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var resultSentiment = JsonConvert.DeserializeObject<ResultJson>(response.Content.ReadAsStringAsync().Result);
                        result = resultSentiment.result.sentiment.ToLower();
                    }

                    int sentiment = result == "negative" ? (int)Util.Sentiments.Negative : result == "positive" ? (int)Util.Sentiments.Positive : (int)Util.Sentiments.Neutral;

                    return sentiment;
                }
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(Helpers), "Sentiment Text :: " + text + " Exception in Sentiment Score  :: ", ex);
                return 4;
            }
        }

        public static async Task<string> SentimentScoreAsync(string text)
        {
            Util.Log.WriteOperationStarted(typeof(Helpers), "SentimentScore for " + text, "Started");
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    string result = "";
                    HttpResponseMessage response;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(AppSettings.SentimentAPI_URL);
                        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("txt", text) });

                        response = await client.PostAsync("/api/text/", content).ConfigureAwait(false);

                        if (response.IsSuccessStatusCode)
                        {
                            result = await response.Content.ReadAsStringAsync();
                        }

                        var resultSentiment = JsonConvert.DeserializeObject<ResultJson>(result);
                        result = resultSentiment.result.sentiment.ToLower();

                        int sentiment = result == "negative" ? 2 : result == "positive" ? 1 : 3;

                        return JsonConvert.DeserializeObject<string>(sentiment.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(typeof(Helpers), "Sentiment Text :: " + text + " Exception in Sentiment Score  :: ", ex);
                }
            }

            return JsonConvert.DeserializeObject<string>(3.ToString());
        }

        public static async Task<string> DetectLanguageAsync(string text)
        {
            Util.Log.WriteOperationStarted(typeof(Helpers), "Language Detection for " + text, "Started");

            try
            {
                using (LanguageServiceClient languageClient = new LanguageServiceClient())
                {
                    var languageCode = await languageClient.GetLanguageAsync(text);
                    string lang = CheckLanguageCode(languageCode);

                    return lang;
                }
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(Helpers), "Exception in Language Detection: ", ex);
                return JsonConvert.DeserializeObject<string>(" ");
            }
        }

        public static string DetectGender(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            try
            {
                HttpResponseMessage response;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(AppSettings.GenderAPI_URL);
                    var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("first_name", name) });

                    response = client.PostAsync("/api/gender/", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var genderResult = JsonConvert.DeserializeObject<BO.Gender.GenderJson>(response.Content.ReadAsStringAsync().Result);
                        if (genderResult.result.status)
                        {
                            return genderResult.result.gender;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(Helpers), "Gender Text :: " + name + " Exception in Gender API :: ", ex);
            }
            return string.Empty;
        }

        public static async Task<string> DetectGenderAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            try
            {
                string result = "";
                HttpResponseMessage response;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(AppSettings.GenderAPI_URL);
                    var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("first_name", name) });

                    response = await client.PostAsync("/api/gender/", content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }

                    var genderResult = JsonConvert.DeserializeObject<BO.Gender.GenderJson>(result);
                    if (genderResult.result.status)
                    {
                        return genderResult.result.gender;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(Helpers), "Gender Text :: " + name + " Exception in Gender API :: ", ex);
            }

            return string.Empty;
        }

    }
}
