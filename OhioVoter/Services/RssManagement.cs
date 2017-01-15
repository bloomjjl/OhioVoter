using OhioVoter.ViewModels.RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.Services
{
    public class RssManagement
    {
        private static int _maxItemCount = 3;



        public Feed GetFoxNewsRssPoliticalFeed()
        {
            string feedUrl = "http://feeds.foxnews.com/foxnews/politics";
            RssReader reader = new RssReader();
            Feed feed = reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);

            return feed;
        }



        public Feed GetCnbcRSSPoliticalFeed()
        {
            string feedUrl = "http://www.cnbc.com/id/10000113/device/rss/rss.html";
            RssReader reader = new RssReader();
            Feed feed = reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);

            return feed;
        }



        public Feed GetCnnRssPoliticalFeed()
        {
            string feedUrl = "http://rss.cnn.com/rss/cnn_allpolitics.rss";
            RssReader reader = new RssReader();
            Feed feed = reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);

            return feed;
        }



        public Feed GetOhioSecretaryOfStateRssFeed()
        {
            string feedUrl = "https://www.sos.state.oh.us/sos/sosfeeds.aspx?hungry=yes";
            RssReader reader = new RssReader();
            Feed feed = reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);

            return feed;
        }

    }
}