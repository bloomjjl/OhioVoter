using OhioVoter.ViewModels.RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace OhioVoter.Services
{
    public class FoxNewsRSSManagement
    {
        public Feed GetFoxNewsRSSPoliticalFeed()
        {
            string feedUrl = "http://feeds.foxnews.com/foxnews/politics";
            int maxItemCount = 5;
            RSSReader reader = new RSSReader();
            Feed feed = reader.GetInformationFromRSSFeed(feedUrl, maxItemCount);

            return feed;
        }


        


    }
}