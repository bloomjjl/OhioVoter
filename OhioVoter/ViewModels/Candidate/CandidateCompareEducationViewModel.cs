using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareEducationViewModel
    {
        public string ControllerName { get; set; }

        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }

        public string CertifiedCandidateId { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public string OfficeHolderId { get; set; }
        public string OfficeHolderName { get; set; }

        public int CandidateFirstDisplayId { get; set; }
        public bool CandidateFirstIsRunningMate { get; set; }
        public int CandidateSecondDisplayId { get; set; }
        public bool CandidateSecondIsRunningMate { get; set; }

        public CandidateCompareEducationFirstViewModel CandidateCompareEducationFirstViewModel { get; set; }
        public CandidateCompareEducationSecondViewModel CandidateCompareEducationSecondViewModel { get; set; }
    }
}