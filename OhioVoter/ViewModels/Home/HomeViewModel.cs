using OhioVoter.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Home
{
    public class HomeViewModel
    {
        public string ControllerName { get; set; }
        public CalendarViewModel Calendar { get; set; }
        public PollViewModel Poll { get; set; }
        public RssFeeds RssFeeds { get; set; }
    }

    public class RssFeeds
    {
        public RssFeedViewModel FoxNewsRssFeed { get; set; }
        public RssFeedViewModel CnnRssFeed { get; set; }
        public RssFeedViewModel CnbcRssFeed { get; set; }
        public RssFeedViewModel OhioSecretaryOfStateRssFeed { get; set; }
    }

}