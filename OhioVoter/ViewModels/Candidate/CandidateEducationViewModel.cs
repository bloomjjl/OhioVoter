using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateEducationViewModel
    {
        public CandidateEducationViewModel() { }

        public CandidateEducationViewModel(List<string> voteSmartCandidateEducationalHistory, List<string> voteSmartRunningMateEducationalHistory, int candidateLookUpId, int candidateId, int runningMateId)
        {
            CandidateLookUpId = candidateLookUpId;
            CandidateId = candidateId;
            RunningMateId = runningMateId;
            CandidateEducationHistory = voteSmartCandidateEducationalHistory;
            RunningMateEducationHistory = voteSmartRunningMateEducationalHistory;
        }


        public int CandidateLookUpId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public IEnumerable<string> CandidateEducationHistory { get; set; }
        public IEnumerable<string> RunningMateEducationHistory { get; set; }
    }
}