using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidatePersonalViewModel
    {
        public CandidatePersonalViewModel() { }

        public CandidatePersonalViewModel(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        {
            CandidateLookUpId = candidateLookUpId;
            CandidateId = candidateId;
            RunningMateId = runningMateId;
            CandidateFamily = voteSmartCandidates[0].Family;
            RunningMateFamily = voteSmartCandidates[1].Family;
            CandidateGender = voteSmartCandidates[0].Gender;
            RunningMateGender = voteSmartCandidates[1].Gender;
            CandidateBirthDate = voteSmartCandidates[0].BirthDate;
            RunningMateBirthDate = voteSmartCandidates[1].BirthDate;
            CandidateBirthPlace = voteSmartCandidates[0].BirthPlace;
            RunningMateBirthPlace = voteSmartCandidates[1].BirthPlace;
            CandidateHomeCity = voteSmartCandidates[0].HomeCity;
            RunningMateHomeCity = voteSmartCandidates[1].HomeCity;
            CandidateHomeState = voteSmartCandidates[0].HomeState;
            RunningMateHomeState = voteSmartCandidates[1].HomeState;
            CandidateReligion = voteSmartCandidates[0].Religion;
            RunningMateReligion = voteSmartCandidates[1].Religion;
        }


        public int CandidateLookUpId { get; set; }
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