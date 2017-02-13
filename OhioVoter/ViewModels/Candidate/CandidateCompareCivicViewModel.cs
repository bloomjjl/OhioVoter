using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareCivicViewModel
    {
        public int CandidateLookUpId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public IEnumerable<string> CandidateCivicMemberships { get; set; }
        public IEnumerable<string> RunningMateCivicMemberships { get; set; }
    }
}