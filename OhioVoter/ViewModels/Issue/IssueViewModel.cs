using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Issue
{
    public class IssueViewModel
    {
        public string ControllerName { get; set; }
        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }
        public IssueLookUpViewModel IssueLookUpViewModel { get; set; }
        public IssueDisplayViewModel IssueDisplayViewModel { get; set; }
        public IssueDetailViewModel IssueDetailViewModel { get; set; }
    }
}