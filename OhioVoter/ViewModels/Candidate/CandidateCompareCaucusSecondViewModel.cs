using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareCaucusSecondViewModel
    {
        public CandidateCompareCaucusSecondViewModel() { }

        public CandidateCompareCaucusSecondViewModel(List<string> voteSmartCandidateCaucusMembership, List<string> voteSmartRunningMateCaucusMembership, CandidateCompareSummarySecondViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateSecondDisplayId;
            CandidateId = summaryVM.CandidateCompareSummarySecond.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummarySecond.CandidateId;
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