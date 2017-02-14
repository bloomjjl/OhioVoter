using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareAdditionalFirstViewModel
    {
        public int CandidateDisplayId { get; set; }
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }

        public IEnumerable<string> CandidateAdditionalInformation { get; set; }
        public IEnumerable<string> RunningMateAdditionalInformation { get; set; }
    }
}