using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareAdditionalFirstViewModel
    {
        public CandidateCompareAdditionalFirstViewModel() { }

        public CandidateCompareAdditionalFirstViewModel(List<string> voteSmartCandidateAdditionalInformation, List<string> voteSmartRunningMateAdditionalInformation, CandidateCompareSummaryFirstViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateFirstDisplayId;
            CandidateId = summaryVM.CandidateCompareSummaryFirst.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummaryFirst.CandidateId;
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