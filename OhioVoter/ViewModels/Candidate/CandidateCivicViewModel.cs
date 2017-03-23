using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCivicViewModel
    {
        public CandidateCivicViewModel() { }

        public CandidateCivicViewModel(List<string> voteSmartCandidateCivicMembership, List<string> voteSmartRunningMateCivicMembership, int candidateLookUpId, int candidateId, int runningMateId)
        {
            CandidateLookUpId = candidateLookUpId;
            CandidateId = candidateId;
            RunningMateId = runningMateId;
            CandidateCivicMemberships = voteSmartCandidateCivicMembership;
            RunningMateCivicMemberships = voteSmartRunningMateCivicMembership;
        }


        public int CandidateLookUpId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public IEnumerable<string> CandidateCivicMemberships { get; set; }
        public IEnumerable<string> RunningMateCivicMemberships { get; set; }
    }
}