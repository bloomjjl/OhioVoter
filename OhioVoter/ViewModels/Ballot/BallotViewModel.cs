using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Ballot
{
    public class BallotViewModel
    {
        public BallotViewModel() { }


        public string ControllerName { get; set; }
        public int ElectionVotingDateId { get; set; }
        public DateTime ElectionVotingDate { get; set; }
        public BallotVoterViewModel BallotVoterViewModel { get; set; }
        public List<BallotOfficeViewModel> BallotOfficeViewModel { get; set; }
        public List<BallotIssueViewModel> BallotIssueViewModel { get; set; }
    }
}