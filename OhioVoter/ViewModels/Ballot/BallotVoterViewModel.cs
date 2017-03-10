using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Ballot
{
    public class BallotVoterViewModel
    {
        public BallotVoterViewModel() { }

        public BallotVoterViewModel(VoterAddressViewModel locationVM)
        {
            AddressNumber = locationVM.StreetNumber;
            AddressStreetName = locationVM.StreetName;
            AddressNumberAndStreetName = locationVM.StreetAddress;
            AddressCityName = locationVM.City;
            AddressZip = locationVM.ZipCode;
            County = locationVM.County;
        }

        public BallotVoterViewModel(Models.HamiltonOhioVoter hamiltonOhioVoterDTO)
        {
            VoterLocationId = hamiltonOhioVoterDTO.Id;
            OhioPrecinctId = hamiltonOhioVoterDTO.OhioPrecinctId;
            PrecinctName = hamiltonOhioVoterDTO.OhioPrecinct.PrecinctName;
            AddressCityName = hamiltonOhioVoterDTO.AddressCityName;
            CountyId = hamiltonOhioVoterDTO.OhioCountyId;
            County = hamiltonOhioVoterDTO.OhioCounty.Name;
            CourtOfAppeasOfficeCode = hamiltonOhioVoterDTO.CourtOfAppealsOfficeCode;
            CongressOfficeCode = hamiltonOhioVoterDTO.CongressOfficeCode;
            SenateOfficeCode = hamiltonOhioVoterDTO.SenateOfficeCode;
            HouseOfficeCode = hamiltonOhioVoterDTO.HouseOfficeCode;
            JudicialOfficeCode = hamiltonOhioVoterDTO.JudicialOfficeCode;
            SchoolOfficeCode = hamiltonOhioVoterDTO.SchoolOfficeCode;
            CountySchoolOfficeCode = hamiltonOhioVoterDTO.CountySchoolOfficeCode;
            VocationalSchoolOfficeCode = hamiltonOhioVoterDTO.VocationalSchoolOfficeCode;
        }

        public int ElectionVotingDateId { get; set; }
        public DateTime ElectionVotingDate { get; set; }

        public int VoterLocationId { get; set; }
        public int OhioPrecinctId { get; set; }
        public string PrecinctName { get; set; }

        public string AddressNumber { get; set; }
        public string AddressStreetName { get; set; }
        public string AddressNumberAndStreetName { get; set; }
        public string AddressSuffix { get; set; }
        public string AddressStreetNameAndSuffix { get; set; }
        public string AddressCityName { get; set; }
        public string AddressZip { get; set; }
        public int CountyId { get; set; }
        public string County { get; set; }

        public string CongressOfficeCode { get; set; }
        public string SenateOfficeCode { get; set; }
        public string HouseOfficeCode { get; set; }
        public string JudicialOfficeCode { get; set; }
        public string SchoolOfficeCode { get; set; }
        public string CountySchoolOfficeCode { get; set; }
        public string VocationalSchoolOfficeCode { get; set; }

        public string CourtOfAppeasOfficeCode { get; set; }
    }
}