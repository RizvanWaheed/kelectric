using CRM.APIS.Feedly.FeedlySharp;
using CRM.APIS.Feedly.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Models.Feedly.Sources
{
    public class DataCollection
    {
        public static List<FeedlyItem> SearchFeeds(string searchTerm, FeedlyClient client, FeedlySearchFeed feed, bool isRss = false)
        {
            List<FeedlyItem> items = new List<FeedlyItem>();
            List<FeedlyItem> filteredItems = new List<FeedlyItem>();
            string continuation = null;
            List<FeedlyEntry> itemsOriginal = new List<FeedlyEntry>();
            do
            {
                FeedlyStreamEntriesResponse entries = client.GetStreamEntries(feed.Id, ContentType.Feed, 100, FeedSorting.Newest, null,null, continuation).Result;
                if (entries.Items != null)
                {
                    itemsOriginal.AddRange(entries.Items);
                    continuation = entries.Continuation;
                }
                else
                {
                    continuation = null;
                }
            } while (continuation != null && continuation != "");
            if (itemsOriginal.Count == 0)
            {
                do
                {
                    FeedlyStreamEntriesResponse entries = client.GetStreamEntries(feed.Id, ContentType.Feed, 100, FeedSorting.Oldest, null, null, continuation).Result;
                    if (entries.Items != null)
                    {
                        itemsOriginal.AddRange(entries.Items);
                        continuation = entries.Continuation;
                    }
                    else
                    {
                        continuation = null;
                    }
                } while (continuation != null && continuation != "");
            }
            if (itemsOriginal.Count == 0 && !isRss) // api issue
            {
               FeedlyStreamEntriesResponse entries = client.GetStreamEntries(feed.Id, ContentType.Feed, 100, FeedSorting.Newest, null, null, null).Result;

                if (entries.Items != null)
                {
                    itemsOriginal.AddRange(entries.Items);
                }
            }
            foreach (FeedlyEntry itemOriginal in itemsOriginal)
            {
                try
                {
                    if ((itemOriginal.Summary != null && itemOriginal.Summary.Content.ToLower().Contains(searchTerm.ToLower())) ||
                        (itemOriginal.Content != null && itemOriginal.Content.Content.ToLower().Contains(searchTerm.ToLower())) ||
                        (itemOriginal.Title != null && itemOriginal.Title.ToLower().Contains(searchTerm.ToLower())))
                        items.Add(MapFeedlyItems(feed, itemOriginal));
                    else if(isRss)
                        items.Add(MapFeedlyItems(feed, itemOriginal));
                }
                catch (Exception ex)
                {
                }
            }
            filteredItems.AddRange(items);
            return filteredItems;
        }
        private static FeedlyItem MapFeedlyItems(FeedlySearchFeed feed,FeedlyEntry item)
        {
            FeedlyItem ret = new FeedlyItem();
            ret.Id = item.Id;
            ret.FingerPrint = item.Fingerprint;
            ret.OriginId = item.OriginId;
            ret.Author = item.Author;
            ret.Title = item.Origin.Name;
            ret.BaseUri = item.Origin != null ? item.Origin.Uri.AbsoluteUri : "";
            ret.PublishDate = item.PublishedDate;
            ret.CrawledDate = item.CrawledDate;
            ret.UpdateDate = item.UpdateDate;
            ret.IsUnread = item.IsUnread;
            ret.Keywords = (item.Keywords != null ? string.Join(", ", item.Keywords) : "");
            ret.Summary_Text = item.Summary != null ? StripHTML(item.Summary.Content) : "";
            ret.Content_Text = item.Content != null ? StripHTML(item.Content.Content) : "";
            if (item.AlternateLinks != null)
                ret.Links.AddRange(MapFeedlyLinks(item.AlternateLinks));
            if (item.CanonicalLinks != null)
                ret.Links.AddRange(MapFeedlyLinks(item.CanonicalLinks));
            ret.Feed.FeedId = feed.Id;
            ret.Feed.Title = feed.Name;
            ret.Feed.CoverUrl = feed.CoverImage != null ? feed.CoverImage.AbsoluteUri : "";
            ret.Feed.Coverage = null;// feed.co;
            ret.Feed.DeliciousTags = null;
            ret.Feed.Description = feed.Description;
            ret.Feed.FacebookLikes = feed.FacebookLikes;
            ret.Feed.FacebookUserName = feed.FacebookName;
            ret.Feed.TwitterFollowers = feed.TwitterFollowers;
            ret.Feed.TwitterScreenName = feed.TwitterName;
            ret.Feed.Velocity = (decimal?)feed.Velocity;
            ret.Feed.VisualUrl = null;
            ret.Feed.Website = feed.Uri != null ? feed.Uri.AbsoluteUri : "";
            ret.Feed.Language = feed.Language;
            if (ret.Summary_Text == "")
                ret.Summary_Text = ret.Content_Text;
            return ret;
        }
        private static List<FeedlyItemLink> MapFeedlyLinks(List<FeedlyLink> links)
        {
            List<FeedlyItemLink> retList = new List<FeedlyItemLink>();
            foreach (var link in links)
            {
               FeedlyItemLink l = new FeedlyItemLink();
                l.MediaType = link.ContentType;
                l.Uri = link.Uri != null ? link.Uri.AbsoluteUri : "";
                retList.Add(l);
            }
            return retList;
        }
        private static string StripHTML(string HTMLText)
        {
            if (HTMLText == null || HTMLText == "")
                return "";
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase); return reg.Replace(HTMLText, "");
        }
    }
}