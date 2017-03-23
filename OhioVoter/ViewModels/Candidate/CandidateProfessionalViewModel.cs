using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateProfessionalViewModel
    {
        public CandidateProfessionalViewModel() { }

        public CandidateProfessionalViewModel(List<string> voteSmartCandidateProfessionalHistory, List<string> voteSmartRunningMateProfessionalHistory, int candidateLookUpId, int candidateId, int runningMateId)
        {
            CandidateLookUpId = candidateLookUpId;
            CandidateId = candidateId;
            RunningMateId = runningMateId;
            CandidateProfessionalHistory = voteSmartCandidateProfessionalHistory;
            RunningMateProfessionalHistory = voteSmartRunningMateProfessionalHistory;
        }


        public int CandidateLookUpId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public IEnumerable<string> CandidateProfessionalHistory { get; set; }
        public IEnumerable<string> RunningMateProfessionalHistory { get; set; }
    }
}