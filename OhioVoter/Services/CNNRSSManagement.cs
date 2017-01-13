using OhioVoter.ViewModels.RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace OhioVoter.Services
{
    public class CNNRSSManagement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Feed GetCNNRSSPoliticalFeed()
        {
            // RSS file formate elements
            // https://cyber.harvard.edu/rss/rss.html

            // Atom file format
            // http://atomenabled.org/
            // https://tools.ietf.org/html/rfc4287


            XmlReader reader = XmlReader.Create("http://rss.cnn.com/rss/cnn_allpolitics.rss");
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            Feed displayFeed = GetInformationFromRSSFeedToDisplay(feed);
            
            return displayFeed;
        }



        private Feed GetInformationFromRSSFeedToDisplay(SyndicationFeed feed)
        {
            int itemCount = GetNumberOfItemsToDisplay(feed.Items.Count());

            if (itemCount == -1)
                return new Feed();

            Feed displayFeed = new Feed()
            {
                Channel = GetChannelFromRSSFeed(feed),
                Items = GetItemsFromRSSFeed(feed, itemCount)
            };

            return displayFeed;
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



        private int GetNumberOfItemsToDisplay(int? maxCount)
        {
            if (maxCount == null)
            {
                return -1;
            }
            else if (maxCount > 5)
            {
                return 5;
            }

            return (int)maxCount;
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
            return new Element()
            {
                Title = item.Title.Text,
                PubDate = item.PublishDate.LocalDateTime,
                // remove html tags from Summary string
                Summary = Regex.Replace(item.Summary.Text, @"<[^>]*>", String.Empty),
                Link = item.Id.ToString()
            };

        }



    }
}