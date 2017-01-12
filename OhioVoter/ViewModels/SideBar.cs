using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels
{
    public class SideBar
    {
        public Location VoterLocation { get; set; }
        public Location PollingLocation { get; set; }
        public Location CountyLocation { get; set; }
        public Location StateLocation { get; set; }
    }

    public class Location
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public string LocationName { get; set; }

        [Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        public string StateName { get; set; }

        [Required]
        [Display(Name = "State")]
        public string StateAbbreviation { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string FullAddress
        {
            get
            {
                return string.Format("{0} {1}, {2} {3}", this.StreetAddress, this.City, this.StateAbbreviation, this.ZipCode);
            }
        }

        public string GoogleLocationMapAPI { get; set; }
    }

}