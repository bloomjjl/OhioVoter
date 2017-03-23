using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareCaucusFirstViewModel
    {
        public CandidateCompareCaucusFirstViewModel() { }

        public CandidateCompareCaucusFirstViewModel(List<string> voteSmartCandidateCaucusMembership, List<string> voteSmartRunningMateCaucusMembership, CandidateCompareSummaryFirstViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateFirstDisplayId;
            CandidateId = summaryVM.CandidateCompareSummaryFirst.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummaryFirst.CandidateId;
            CandidateCaucusHistory = voteSmartCandidateCaucusMembership;
            RunningMateCaucusHistory = voteSmartRunningMateCaucusMembership;
        }


        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidateCaucusHistory { get; set; }
        public IEnumerable<string> RunningMateCaucusHistory { get; set; }
    }
}