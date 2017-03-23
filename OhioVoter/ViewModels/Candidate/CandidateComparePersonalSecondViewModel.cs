using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateComparePersonalSecondViewModel
    {
        public CandidateComparePersonalSecondViewModel() { }

        public CandidateComparePersonalSecondViewModel(ViewModels.VoteSmart.CandidateBio voteSmartCandidateBiography, ViewModels.VoteSmart.CandidateBio voteSmartRunningMateBiography, CandidateCompareSummarySecondViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateSecondDisplayId;
            CandidateId = summaryVM.CandidateCompareSummarySecond.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummarySecond.CandidateId;

            CandidateFamily = voteSmartCandidateBiography.Family;
            RunningMateFamily = voteSmartRunningMateBiography.Family;
            CandidateGender = voteSmartCandidateBiography.Gender;
            RunningMateGender = voteSmartRunningMateBiography.Gender;
            CandidateBirthDate = voteSmartCandidateBiography.BirthDate;
            RunningMateBirthDate = voteSmartRunningMateBiography.BirthDate;
            CandidateBirthPlace = voteSmartCandidateBiography.BirthPlace;
            RunningMateBirthPlace = voteSmartRunningMateBiography.BirthPlace;
            CandidateHomeCity = voteSmartCandidateBiography.HomeCity;
            RunningMateHomeCity = voteSmartRunningMateBiography.HomeCity;
            CandidateHomeState = voteSmartCandidateBiography.HomeState;
            RunningMateHomeState = voteSmartRunningMateBiography.HomeState;
            CandidateReligion = voteSmartCandidateBiography.Religion;
            RunningMateReligion = voteSmartRunningMateBiography.Religion;
        }


        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public string CandidateFamily { get; set; }
        public string RunningMateFamily { get; set; }
        public string CandidateGender { get; set; }
        public string RunningMateGender { get; set; }
        public string CandidateBirthDate { get; set; }
        public string RunningMateBirthDate { get; set; }
        public string CandidateBirthPlace { get; set; }
        public string RunningMateBirthPlace { get; set; }
        public string CandidateHomeCity { get; set; }
        public string RunningMateHomeCity { get; set; }
        public string CandidateHomeState { get; set; }
        public string RunningMateHomeState { get; set; }
        public string CandidateReligion { get; set; }
        public string RunningMateReligion { get; set; }
    }
}