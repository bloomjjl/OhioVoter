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
        public Feed CnnRssFeed { get; set; }
        public Feed FoxNewsRssFeed { get; set; }
    }

}