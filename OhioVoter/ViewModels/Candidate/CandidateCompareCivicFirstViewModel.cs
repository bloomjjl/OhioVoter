using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareCivicFirstViewModel
    {
        public CandidateCompareCivicFirstViewModel() { }

        public CandidateCompareCivicFirstViewModel(List<string> voteSmartCandidateCivicHistory, List<string> voteSmartRunningMateCivicHistory, CandidateCompareSummaryFirstViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateFirstDisplayId;
            CandidateId = summaryVM.CandidateCompareSummaryFirst.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummaryFirst.CandidateId;
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