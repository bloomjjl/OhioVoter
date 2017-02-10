using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Location
{
    public class SideBarViewModel
    {
        public string ControllerName { get; set; }
        public VoterLocationViewModel VoterLocationViewModel { get; set; }
        public PollingLocationViewModel PollingLocationViewModel { get; set; }
        public CountyLocationViewModel CountyLocationViewModel { get; set; }
        public StateLocationViewModel StateLocationViewModel { get; set; }
    }



}