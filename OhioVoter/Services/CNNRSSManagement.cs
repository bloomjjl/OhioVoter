﻿using OhioVoter.ViewModels.RSS;
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
        public Feed GetCNNRSSPoliticalFeed()
        {
            string feedUrl = "http://rss.cnn.com/rss/cnn_allpolitics.rss";
            int maxItemCount = 5;
            RSSReader reader = new RSSReader();
            Feed feed = reader.GetInformationFromRSSFeed(feedUrl, maxItemCount);

            return feed;
        }




    }
}