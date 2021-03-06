﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareEducationFirstViewModel
    {
        public CandidateCompareEducationFirstViewModel() { }

        public CandidateCompareEducationFirstViewModel(List<string> voteSmartCandidateEducationHistory, List<string> voteSmartRunningMateEducationHistory, CandidateCompareSummaryFirstViewModel summaryVM)
        {
            CandidateDisplayId = summaryVM.CandidateFirstDisplayId;
            CandidateId = summaryVM.CandidateCompareSummaryFirst.CandidateId;
            RunningMateId = summaryVM.RunningMateCompareSummaryFirst.CandidateId;
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