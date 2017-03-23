using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareProfessionalSecondViewModel
    {
        public CandidateCompareProfessionalSecondViewModel() { }

        public CandidateCompareProfessionalSecondViewModel(List<string> voteSmartCandidateProfessionalHistory, List<string> voteSmartRunningMateProfessionalHistory, CandidateCompareSummarySecondViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateSecondDisplayId;
            CandidateId = summaryVM.CandidateCompareSummarySecond.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummarySecond.CandidateId;
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