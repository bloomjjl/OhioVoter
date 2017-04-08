using Newtonsoft.Json.Linq;
using OhioVoter.ViewModels.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class RssReader
    {
        /// <summary>
        /// get the channel and specified amount of items from the supplied rss feed
        /// </summary>
        /// <param name="feedUrl"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        public Feed GetInformationFromRSSFeed(string feedUrl, int maxItemCount)
        {
            try
            {
                XmlReader reader = XmlReader.Create(feedUrl);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();

                return GetInformationFromRSSFeedToDisplay(feed, maxItemCount);
            }
            catch (Exception e)
            {
                return new Feed();
            }
        }



        /// <summary>
        /// Make sure feed object is valid. Then get the channel and specified amount of items from feed
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        private Feed GetInformationFromRSSFeedToDisplay(SyndicationFeed feed, int maxItemCount)
        {
            int itemCount = GetNumberOfItemsToDisplay(feed.Items.Count(), maxItemCount);

            if (itemCount == -1)
                return new Feed();

            return new Feed()
            {
                Channel = GetChannelFromRSSFeed(feed),
                Items = GetItemsFromRSSFeed(feed, itemCount)
            };
        }



        /// <summary>
        /// make sure there are items to display and set the max limit to get from feed
        /// </summary>
        /// <param name="feedItemCount"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
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



        /// <summary>
        /// store the channel object from feed
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        private Channel GetChannelFromRSSFeed(SyndicationFeed feed)
        {
            return new Channel()
            {
                Element = GetElementInformationForChannel(feed)
            };
        }



        /// <summary>
        /// separate and store the channel elements
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        private Element GetElementInformationForChannel(SyndicationFeed feed)
        {
            return new Element()
            {
                Image = GetImageUrlFromChannelElement(feed),
                Link_0 = GetLinkFromChannelElement(feed, 0),
                Link_1 = GetLinkFromChannelElement(feed, 1),
                Link_2 = GetLinkFromChannelElement(feed, 2),
                Title = GetTitleFromChannelElement(feed)
            };
        }



        /// <summary>
        /// make sure URL is valid
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        private String GetImageUrlFromChannelElement(SyndicationFeed feed)
        {
            try
            {
                string imageUrl = feed.ImageUrl.AbsolutePath.ToString();
                return feed.ImageUrl.OriginalString.ToString();
            }
            catch
            {
                // catch if value is null
                return string.Empty;
            }
        }



        /// <summary>
        /// make sure the link is valid
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private String GetLinkFromChannelElement(SyndicationFeed feed, int index)
        {
            try
            {
                return feed.Links[index].Uri.AbsoluteUri.ToString();
            }
            catch
            {
                // catch if value is null
                return string.Empty;
            }
        }



        /// <summary>
        /// make sure the title is valid
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        private String GetTitleFromChannelElement(SyndicationFeed feed)
        {
            try
            {
                return feed.Title.Text;
            }
            catch
            {
                // catch if value is null
                return string.Empty;
            }
        }



        /// <summary>
        /// sort the items for the feed by PubDate and store based on the itemCount 
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
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



        /// <summary>
        /// store all the items for the feed
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        private List<Item> GetListOfAllItemsInRssFeed(SyndicationFeed feed)
        {
            List<Item> items = new List<Item>();

            foreach (var item in feed.Items)
            {
                items.Add(GetItemFromRssFeed(item));
            };

            return items;

        }



        /// <summary>
        /// store the item object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Item GetItemFromRssFeed(SyndicationItem item)
        {
            return new Item()
            {
                Element = GetItemInformationForCurrentItemInRssFeed(item)
            };
        }



        /// <summary>
        /// separate and store the item elements
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Element GetItemInformationForCurrentItemInRssFeed(SyndicationItem item)
        {
            Element element = new Element()
            {
                Title = GetTitleFromItemElement(item),
                PubDate = GetPublishDateFromItemElement(item),
                Summary = GetSummaryFromItemElement(item),
                Link_0 = GetLinkFromItemElement(item, 0),
                Id = GetIdFromItemElement(item)
            };

            element = RemoveHTMLTagsFromSummaryElement(element);

            return element;
        }



        /// <summary>
        /// make sure title is valid
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private String GetTitleFromItemElement(SyndicationItem item)
        {
            try
            {
                return item.Title.Text;
            }
            catch
            {
                // catch if value is null
                return string.Empty;
            }
        }



        /// <summary>
        /// make sure date is valid
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DateTime GetPublishDateFromItemElement(SyndicationItem item)
        {
            try
            {
                return item.PublishDate.LocalDateTime;
            }
            catch
            {
                // catch if value is null
                return Convert.ToDateTime("1/01/1900");
            }
        }



        /// <summary>
        /// make sure summary is valid
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private String GetSummaryFromItemElement(SyndicationItem item)
        {
            try
            {
                return item.Summary.Text;
            }
            catch
            {
                // catch if value is null
                return string.Empty;
            }
        }



        /// <summary>
        /// make sure a link is valid
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private String GetLinkFromItemElement(SyndicationItem item, int index)
        {
            try
            {
                return item.Links[index].Uri.AbsoluteUri.ToString();
            }
            catch
            {
                // catch if value is null
                return string.Empty;
            }
        }



        /// <summary>
        /// Make sure an Id is valid
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private String GetIdFromItemElement(SyndicationItem item)
        {
            try
            {
                return item.Id.ToString();
            }
            catch
            {
                // catch if value is null
                return string.Empty;
            }
        }



        /// <summary>
        /// Remove the HTML tags so they don't display on webpage
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private Element RemoveHTMLTagsFromSummaryElement(Element element)
        {
            element.Summary = Regex.Replace(element.Summary.ToString(), @"<[^>]*?>", string.Empty);
            return element;
        }




    }
}

