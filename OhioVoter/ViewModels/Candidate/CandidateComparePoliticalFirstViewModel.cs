using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateComparePoliticalFirstViewModel
    {
        public CandidateComparePoliticalFirstViewModel() { }

        public CandidateComparePoliticalFirstViewModel(List<string> voteSmartCandidatePoliticalHistory, List<string> voteSmartRunningMatePoliticalHistory, CandidateCompareSummaryFirstViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateFirstDisplayId;
            CandidateId = summaryVM.CandidateCompareSummaryFirst.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummaryFirst.CandidateId;
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