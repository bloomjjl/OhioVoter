using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Location
{
    public class PollingLocationViewModel
    {
        public PollingLocationViewModel() { }

        public PollingLocationViewModel(Models.OhioPrecinct precinctDTO)
        {
            PrecinctId = precinctDTO.Id;
            LocationName = precinctDTO.PollingLocationName;
            StreetAddress = precinctDTO.PollingAddress1;
            StreetAddress2 = precinctDTO.PollingAddress2;
            City = precinctDTO.PollingCity;
            StateAbbreviation = precinctDTO.PollingState;
            ZipCode = precinctDTO.PollingZipCode;
            CountyId = precinctDTO.OhioCountyId;
        }


        public int PrecinctId { get; set; }
        public string Status { get; set; }

        public string Message { get; set; }

        public string LocationName { get; set; }

        [Required(ErrorMessage = "Please enter a street address in Ohio.")]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        public string StreetAddress2 { get; set; }

        public string Neighborhood { get; set; }

        [Required()]
        public string City { get; set; }

        public int CountyId { get; set; }

        public string County { get; set; }

        public string StateName { get; set; }

        public string StateAbbreviation { get; set; }

        [Required(ErrorMessage = "Please enter a zip code in Ohio.")]
        [Display(Name = "Zip Code")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Zip Code Must Be 5 Digits")]
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

        public string VoterStreetAddress { get; set; }

        public string VoterCity { get; set; }

        public string VoterStateAbbreviation { get; set; }

        public string VoterZipCode { get; set; }

        public string VoterFullAddress
        {
            get
            {
                return string.Format("{0} {1}, {2} {3}", this.VoterStreetAddress, this.VoterCity, this.VoterStateAbbreviation, this.VoterZipCode);
            }
        }

        public string GoogleLocationMapAPI { get; set; }
    }


}