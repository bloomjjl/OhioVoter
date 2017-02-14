using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareDisplayViewModel
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
        public int CandidateSecondCompareCount { get; set; }
        public bool CandidateSecondIsRunningMate { get; set; }

        public CandidateCompareSummaryViewModel CandidateCompareSummaryViewModel { get; set; }
        public CandidateComparePoliticalViewModel CandidateComparePoliticalViewModel { get; set; }
        public CandidateCompareCaucusViewModel CandidateCompareCaucusViewModel { get; set; }
        public CandidateCompareProfessionalViewModel CandidateCompareProfessionalViewModel { get; set; }
        public CandidateCompareEducationViewModel CandidateCompareEducationViewModel { get; set; }
        public CandidateComparePersonalViewModel CandidateComparePersonalViewModel { get; set; }
        public CandidateCompareCivicViewModel CandidateCompareCivicViewModel { get; set; }
        public CandidateCompareAdditionalViewModel CandidateCompareAdditionalViewModel { get; set; }
    }
}