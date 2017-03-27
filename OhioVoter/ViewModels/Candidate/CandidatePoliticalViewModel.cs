using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidatePoliticalViewModel
    {
        public CandidatePoliticalViewModel() { }

        public CandidatePoliticalViewModel(List<string> voteSmartCandidatePoliticalHistory, List<string> voteSmartRunningMatePoliticalHistory, int candidateLookUpId, int candidateId, int runningMateId)
        {
            CandidateLookUpId = candidateLookUpId;
            CandidateId = candidateId;
            RunningMateId = runningMateId;
            CandidatePoliticalHistory = voteSmartCandidatePoliticalHistory;
            RunningMatePoliticalHistory = voteSmartRunningMatePoliticalHistory;
        }


        public int CandidateLookUpId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public IEnumerable<string> CandidatePoliticalHistory { get; set; }
        public IEnumerable<string> RunningMatePoliticalHistory { get; set; }
    }
}