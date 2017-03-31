using OhioVoter.ViewModels.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace OhioVoter.Services
{
    public class RssManagement
    {
        private static int _maxItemCount = 3;



        /// <summary>
        /// get the rss feed from Fox news
        /// </summary>
        /// <returns></returns>
        public Feed GetFoxNewsRssPoliticalFeed()
        {
            string feedUrl = "http://feeds.foxnews.com/foxnews/politics";
            RssReader reader = new RssReader();
            return reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);
        }



        /// <summary>
        /// get the rss feed from CNBC news
        /// </summary>
        /// <returns></returns>
        public Feed GetCnbcRSSPoliticalFeed()
        {
            string feedUrl = "http://www.cnbc.com/id/10000113/device/rss/rss.html";
            RssReader reader = new RssReader();
            return reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);
        }



        /// <summary>
        /// get the rss feed from CNN news
        /// </summary>
        /// <returns></returns>
        public Feed GetCnnRssPoliticalFeed()
        {
            string feedUrl = "http://rss.cnn.com/rss/cnn_allpolitics.rss";
            RssReader reader = new RssReader();
            return reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);
        }



        /// <summary>
        /// get the rss feed from Ohio Secretary Of State website
        /// </summary>
        /// <returns></returns>
        /// ERROR: this rss feed URL is not working properly
        ///        local variable named: lastBuildDate is throwing an error
        ///        when RssReader runs SyndicationFeed.Load(reader) 
        public Feed GetOhioSecretaryOfStateRssFeed()
        {
            string feedUrl = "https://www.sos.state.oh.us/sos/sosfeeds.aspx?hungry=yes";
            RssReader reader = new RssReader();
            return reader.GetInformationFromRSSFeed(feedUrl, _maxItemCount);
        }

    }
}