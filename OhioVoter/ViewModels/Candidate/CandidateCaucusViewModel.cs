using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCaucusViewModel
    {
        public CandidateCaucusViewModel() { }

        public CandidateCaucusViewModel(List<string> voteSmartCandidateCaucusHistory, List<string> voteSmartRunningMateCaucusHistory, int candidateLookUpId, int candidateId, int runningMateId)
        {
            CandidateLookUpId = candidateLookUpId;
            CandidateId = candidateId;
            RunningMateId = runningMateId;
            CandidateCaucusHistory = voteSmartCandidateCaucusHistory;
            RunningMateCaucusHistory = voteSmartRunningMateCaucusHistory;
        }


        public int CandidateLookUpId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public IEnumerable<string> CandidateCaucusHistory { get; set; }
        public IEnumerable<string> RunningMateCaucusHistory { get; set; }
    }
}