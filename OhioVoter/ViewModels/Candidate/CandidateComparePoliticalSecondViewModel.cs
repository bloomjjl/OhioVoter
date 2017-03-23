using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateComparePoliticalSecondViewModel
    {
        public CandidateComparePoliticalSecondViewModel() { }

        public CandidateComparePoliticalSecondViewModel(List<string> voteSmartCandidatePoliticalHistory, List<string> voteSmartRunningMatePoliticalHistory, CandidateCompareSummarySecondViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateSecondDisplayId;
            CandidateId = summaryVM.CandidateCompareSummarySecond.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummarySecond.CandidateId;
            CandidatePoliticalHistory = voteSmartCandidatePoliticalHistory;
            RunningMatePoliticalHistory = voteSmartRunningMatePoliticalHistory;
        }


        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidatePoliticalHistory { get; set; }
        public IEnumerable<string> RunningMatePoliticalHistory { get; set; }
    }
}