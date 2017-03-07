using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Ballot
{
    public class BallotLocationViewModel
    {
        public BallotLocationViewModel() { }

        public BallotLocationViewModel(ViewModels.Location.VoterLocationViewModel locationVM )
        {
            ControllerName = locationVM.ControllerName;
            Status = locationVM.Status;
            Message = locationVM.Message;
            StreetNumber = locationVM.StreetNumber;
            StreetName = locationVM.StreetName;
            StreetAddress = locationVM.StreetAddress;
            City = locationVM.City;
            StateAbbreviation = locationVM.StateAbbreviation;
            ZipCode = locationVM.ZipCode;
            County = locationVM.County;
        }


        public string ControllerName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateAbbreviation { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }
    }
}
