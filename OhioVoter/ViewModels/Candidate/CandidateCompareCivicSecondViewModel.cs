using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareCivicSecondViewModel
    {
        public CandidateCompareCivicSecondViewModel() { }

        public CandidateCompareCivicSecondViewModel(List<string> voteSmartCandidateCivicHistory, List<string> voteSmartRunningMateCivicHistory, CandidateCompareSummarySecondViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateSecondDisplayId;
            CandidateId = summaryVM.CandidateCompareSummarySecond.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummarySecond.CandidateId;
            CandidateCivicMemberships = voteSmartCandidateCivicHistory;
            RunningMateCivicMemberships = voteSmartRunningMateCivicHistory;
        }


        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidateCivicMemberships { get; set; }
        public IEnumerable<string> RunningMateCivicMemberships { get; set; }
    }
}