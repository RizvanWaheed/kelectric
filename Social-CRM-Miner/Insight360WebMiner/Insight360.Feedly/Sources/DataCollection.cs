using Insight360.Feedly.FeedlySharp;
using Insight360.Feedly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insight360.Feedly.Sources
{
    public class DataCollection
    {
        public static List<Feedly.ViewModel.FeedlyItem> SearchFeeds(string searchTerm, FeedlyClient client, FeedlySearchFeed feed,DateTime? lastFeatchDate,bool isRss = false)
        {
            List<Feedly.ViewModel.FeedlyItem> items = new List<Feedly.ViewModel.FeedlyItem>();
            List<Feedly.ViewModel.FeedlyItem> filteredItems = new List<Feedly.ViewModel.FeedlyItem>();
            string continuation = null;
            List<FeedlyEntry> itemsOriginal = new List<FeedlyEntry>();
            try
            {
                do
                {
                    FeedlyStreamEntriesResponse entries = client.GetStreamEntries(feed.Id, ContentType.Feed, 100, FeedSorting.Newest, null, lastFeatchDate, continuation).Result;
                    if (entries.Items != null)
                    {
                        itemsOriginal.AddRange(entries.Items);
                        continuation = entries.Continuation;
                    }
                    else
                    {
                        continuation = null;
                    }
                } while (continuation != null && continuation != "" && itemsOriginal.Count <= 100);
                /*if (itemsOriginal.Count == 0)
                {
                    do
                    {
                        FeedlyStreamEntriesResponse entries = client.GetStreamEntries(feed.Id, ContentType.Feed, 100, FeedSorting.Oldest, null, lastFeatchDate, continuation).Result;
                        if (entries.Items != null)
                        {
                            itemsOriginal.AddRange(entries.Items);
                            continuation = entries.Continuation;
                        }
                        else
                        {
                            continuation = null;
                        }
                    } while (continuation != null && continuation != "" && itemsOriginal.Count <= 1500);
                }*/
                //if (itemsOriginal.Count == 0 && !isRss) // api issue
                //{
                //    FeedlyStreamEntriesResponse entries = client.GetStreamEntries(feed.Id, ContentType.Feed, 100, FeedSorting.Newest, null,lastFeatchDate, null).Result;

                //    if (entries.Items != null)
                //    {
                //        itemsOriginal.AddRange(entries.Items);
                //    }
                //}
                foreach (FeedlyEntry itemOriginal in itemsOriginal)
                {
                    try
                    {
                        if ((itemOriginal.Summary != null && itemOriginal.Summary.Content.ToLower().Contains(searchTerm.ToLower())) ||
                            (itemOriginal.Content != null && itemOriginal.Content.Content.ToLower().Contains(searchTerm.ToLower())) ||
                            (itemOriginal.Title != null && itemOriginal.Title.ToLower().Contains(searchTerm.ToLower())))
                            items.Add(MapFeedlyItems(feed, itemOriginal));
                        else if (isRss)
                            items.Add(MapFeedlyItems(feed, itemOriginal));
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                filteredItems.AddRange(items);

                return filteredItems;
            }
            catch (Exception ex)
            {
                return filteredItems;
            }
        }
        private static ViewModel.FeedlyItem MapFeedlyItems(FeedlySearchFeed feed, FeedlyEntry item)
        {
            ViewModel.FeedlyItem ret = new ViewModel.FeedlyItem();
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
        private static List<Feedly.ViewModel.FeedlyItemLink> MapFeedlyLinks(List<FeedlyLink> links)
        {
            List<Feedly.ViewModel.FeedlyItemLink> retList = new List<Feedly.ViewModel.FeedlyItemLink>();
            foreach (var link in links)
            {
                Feedly.ViewModel.FeedlyItemLink l = new Feedly.ViewModel.FeedlyItemLink();
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
