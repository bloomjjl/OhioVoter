using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareAdditionalSecondViewModel
    {
        public CandidateCompareAdditionalSecondViewModel() { }

        public CandidateCompareAdditionalSecondViewModel(List<string> voteSmartCandidateAdditionalInformation, List<string> voteSmartRunningMateAdditionalInformation, CandidateCompareSummarySecondViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateSecondDisplayId;
            CandidateId = summaryVM.CandidateCompareSummarySecond.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummarySecond.CandidateId;
            CandidateAdditionalInformation = voteSmartCandidateAdditionalInformation;
            RunningMateAdditionalInformation = voteSmartRunningMateAdditionalInformation;
        }


        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidateAdditionalInformation { get; set; }
        public IEnumerable<string> RunningMateAdditionalInformation { get; set; }
    }
}