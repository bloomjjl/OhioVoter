using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Location
{
    public class CountyLocationViewModel
    {
        public CountyLocationViewModel() { }

        public CountyLocationViewModel(Models.OhioBoardOfElection countyDTO)
        {
            StreetAddress = countyDTO.StreetAddress1;
            StreetAddress2 = countyDTO.StreetAddress2;
            City = countyDTO.City;
            StateAbbreviation = countyDTO.State;
            ZipCode = countyDTO.ZipCode.ToString();
            County = countyDTO.OhioCounty.Name;
            Website = countyDTO.Website;
            Phone = countyDTO.Phone;
        }


        public string Status { get; set; }

        public string Message { get; set; }

        public string LocationName { get; set; }

        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        public string StreetAddress2 { get; set; }

        public string Neighborhood { get; set; }

        [Required()]
        public string City { get; set; }

        public string County { get; set; }

        public string StateName { get; set; }

        public string StateAbbreviation { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        public string ZipCodeSuffix { get; set; }

        public string Country { get; set; }

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