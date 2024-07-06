using Google.Apis.Util.Store;
using SocialCRM.Web.Repositories;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.GoogleAnalytics
{
    public class AnalyticsDataStore : IDataStore
    {

        private IGoogleAnalyticRepository repo = new GoogleAnalyticRepository(); 
        public async Task ClearAsync()
        {          

        }

        public async Task DeleteAsync<T>(string key)
        {
            await repo.DeleteAsync<T>(key);
        }

        public Task<T> GetAsync<T>(string key)
        {
            var value =  repo.GetAsync<T>(key);
            return value;
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            await repo.StoreAsync<T>(key, value);
        }

        private static string GenerateStoredKey(string key, Type t)
        {
            return string.Format("{0}-{1}", t.FullName, key);
        }
        private string GetUserName(string key)
        {
            if (key.Contains('-'))
            {
                var accountName = key.Split('-');
                return accountName.First();
            }
            else
                return key;
        }
    }
}