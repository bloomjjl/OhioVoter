using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Ballot
{
    public class BallotOfficeViewModel
    {
        public BallotOfficeViewModel() { }

        public BallotOfficeViewModel(Models.ElectionOffice officeDTO)
        {
            ElectionVotingDateId = officeDTO.ElectionVotingDateId;
            ElectionVotingDate = officeDTO.ElectionVotingDate.Date.ToShortDateString();
            ElectionOfficeId = officeDTO.Id;
            OfficeId = officeDTO.OfficeId;
            OfficeName = officeDTO.Office.OfficeName;
            OfficeTerm = officeDTO.OfficeTerm;
            NumberOfSeats = officeDTO.NumberOfSeats;
        }


        public int ElectionVotingDateId { get; set; }
        public string ElectionVotingDate { get; set; }
        public int ElectionOfficeId { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string OfficeTerm { get; set; }
        public int NumberOfSeats { get; set; }
        public List<BallotCandidateViewModel> BallotListedCandidatesViewModel { get; set; }
        public List<BallotCandidateViewModel> BallotwriteInCandidatesViewModel { get; set; }
    }

}