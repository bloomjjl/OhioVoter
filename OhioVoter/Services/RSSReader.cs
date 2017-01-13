using OhioVoter.ViewModels.RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace OhioVoter.Services
{
    // RSS file formate elements
    // https://cyber.harvard.edu/rss/rss.html


    /// <summary>
    /// An RSS Feed reader server control, it will basically aggregate rss feeds and display the content
    /// </summary>
    /// 
    public class RSSReader
    {

        public Feed GetInformationFromRSSFeed(string feedUrl, int maxItemCount)
        {
            XmlReader reader = XmlReader.Create(feedUrl);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            Feed displayFeed = GetInformationFromRSSFeedToDisplay(feed, maxItemCount);

            return displayFeed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        private Feed GetInformationFromRSSFeedToDisplay(SyndicationFeed feed, int maxItemCount)
        {
            int itemCount = GetNumberOfItemsToDisplay(feed.Items.Count(), maxItemCount);

            if (itemCount == -1)
                return new Feed();

            Feed displayFeed = new Feed()
            {
                Channel = GetChannelFromRSSFeed(feed),
                Items = GetItemsFromRSSFeed(feed, itemCount)
            };

            return displayFeed;
        }



        private int GetNumberOfItemsToDisplay(int? feedItemCount, int maxItemCount)
        {
            if (feedItemCount == null)
            {
                return -1;
            }
            else if (feedItemCount > maxItemCount)
            {
                return maxItemCount;
            }

            return (int)feedItemCount;
        }



        private Channel GetChannelFromRSSFeed(SyndicationFeed feed)
        {
            return new Channel()
            {
                Element = GetElementInformationForChannel(feed)
            };
        }



        private Element GetElementInformationForChannel(SyndicationFeed feed)
        {
            return new Element()
            {
                Image = feed.ImageUrl.OriginalString.ToString(),
                Title = feed.Title.Text
            };
        }



        private IEnumerable<Item> GetItemsFromRSSFeed(SyndicationFeed feed, int itemCount)
        {
            List<Item> sortedItems = GetListOfAllItemsInRssFeed(feed).OrderByDescending(x => x.Element.PubDate).ToList();
            List<Item> selectedItems = new List<Item>();

            for (int i = 0; i < itemCount; i++)
            {
                selectedItems.Add(sortedItems[i]);
            }

            return selectedItems;
        }



        private List<Item> GetListOfAllItemsInRssFeed(SyndicationFeed feed)
        {
            List<Item> items = new List<Item>();

            foreach (var item in feed.Items)
            {
                items.Add(GetItemFromRssFeed(item));
            };

            return items;

        }



        private Item GetItemFromRssFeed(SyndicationItem item)
        {
            return new Item()
            {
                Element = GetItemInformationForCurrentItemInRssFeed(item)
            };
        }



        private Element GetItemInformationForCurrentItemInRssFeed(SyndicationItem item)
        {
            Element element = new Element()
            {
                Title = item.Title.Text,
                PubDate = item.PublishDate.LocalDateTime,
                Summary = item.Summary.Text,
                Link = item.Id.ToString()
            };

            element = RemoveHTMLTagsFromSummaryElement(element);

            return element;
        }



        private Element RemoveHTMLTagsFromSummaryElement(Element element)
        {
            element.Summary = Regex.Replace(element.Summary.ToString(), @"<[^>]*>", string.Empty);
            return element;
        }




    }
}

