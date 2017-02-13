using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareViewModel
    {
        public string ControllerName { get; set; }
        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }
        public int OfficeId { get; set; }

        public int CandidateFirstDisplayId { get; set; }
        public bool CandidateFirstIsRunningMate { get; set; }
        public int CandidateFirstId { get; set; }
        public int RunningMateFirstId { get; set; }

        public int CandidateSecondDisplayId { get; set; }
        public bool CandidateSecondIsRunningMate { get; set; }
        public int CandidateSecondId { get; set; }
        public int RunningMateSecondId { get; set; }

        public CandidateCompareDisplayViewModel CandidateCompareDisplayViewModel { get; set; }
    }
}