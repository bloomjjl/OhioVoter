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
        public CalendarViewModel CalendarViewModel { get; set; }
        public PollViewModel PollViewModel { get; set; }
        public RssFeeds RssFeedsViewModel { get; set; }
    }

    public class RssFeeds
    {
        public RssFeedViewModel FoxNewsRssFeed { get; set; }
        public RssFeedViewModel CnnRssFeed { get; set; }
        public RssFeedViewModel CnbcRssFeed { get; set; }
        public RssFeedViewModel OhioSecretaryOfStateRssFeed { get; set; }
    }

}