using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareEducationSecondViewModel
    {
        public CandidateCompareEducationSecondViewModel() { }

        public CandidateCompareEducationSecondViewModel(List<string> voteSmartCandidateEducationHistory, List<string> voteSmartRunningMateEducationHistory, CandidateCompareSummarySecondViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateSecondDisplayId;
            CandidateId = summaryVM.CandidateCompareSummarySecond.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummarySecond.CandidateId;
            CandidateEducationHistory = voteSmartCandidateEducationHistory;
            RunningMateEducationHistory = voteSmartRunningMateEducationHistory;
        }


        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidateEducationHistory { get; set; }
        public IEnumerable<string> RunningMateEducationHistory { get; set; }
    }
}