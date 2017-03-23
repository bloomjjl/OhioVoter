using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareProfessionalFirstViewModel
    {
        public CandidateCompareProfessionalFirstViewModel() { }

        public CandidateCompareProfessionalFirstViewModel(List<string> voteSmartCandidateProfessionalHistory, List<string> voteSmartRunningMateProfessionalHistory, CandidateCompareSummaryFirstViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateFirstDisplayId;
            CandidateId = summaryVM.CandidateCompareSummaryFirst.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummaryFirst.CandidateId;
            CandidateProfessionalHistory = voteSmartCandidateProfessionalHistory;
            RunningMateProfessionalHistory = voteSmartRunningMateProfessionalHistory;
        }


        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidateProfessionalHistory { get; set; }
        public IEnumerable<string> RunningMateProfessionalHistory { get; set; }
    }
}