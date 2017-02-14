﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareCivicFirstViewModel
    {
        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidateCivicMemberships { get; set; }
        public IEnumerable<string> RunningMateCivicMemberships { get; set; }
    }
}