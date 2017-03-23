using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateAdditionalViewModel
    {
        public CandidateAdditionalViewModel() { }

        public CandidateAdditionalViewModel(List<string> voteSmartCandidateAdditionalInformation, List<string> voteSmartRunningMateAdditionalInformation, int candidateLookUpId, int candidateId, int runningMateId)
        {
            CandidateLookUpId = candidateLookUpId;
            CandidateId = candidateId;
            RunningMateId = runningMateId;
            CandidateAdditionalInformation = voteSmartCandidateAdditionalInformation;
            RunningMateAdditionalInformation = voteSmartRunningMateAdditionalInformation;
        }


        public int CandidateLookUpId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public IEnumerable<string> CandidateAdditionalInformation { get; set; }
        public IEnumerable<string> RunningMateAdditionalInformation { get; set; }
    }
}