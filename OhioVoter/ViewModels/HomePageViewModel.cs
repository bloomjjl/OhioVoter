using OhioVoter.ViewModels.Election;
using OhioVoter.ViewModels.RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels
{
    public class HomePageViewModel
    {
        public SideBar SideBar { get; set; }
        public Calendar Calendar { get; set; }
        public Poll Poll { get; set; }
        public RssFeed RssFeeds { get; set; }
    }

    public class RssFeed
    {
        public Feed FoxNewsRssFeed { get; set; }
        public Feed CnnRssFeed { get; set; }
        public Feed CnbcRssFeed { get; set; }
        public Feed OhioSecretaryOfStateRssFeed { get; set; }
    }

}