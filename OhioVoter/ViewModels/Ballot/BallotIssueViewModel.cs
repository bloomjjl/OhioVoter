using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Ballot
{
    public class BallotIssueViewModel
    {
        public BallotIssueViewModel() { }

        public BallotIssueViewModel(Models.ElectionIssuePrecinct issueDTO)
        {
            ElectionIssueId = issueDTO.ElectionIssueId;
            VotingDateId = issueDTO.ElectionIssue.ElectionVotingDateId;
            VotingDate = issueDTO.ElectionIssue.ElectionVotingDate.Date.ToShortDateString();
            CountyId = issueDTO.ElectionIssue.OhioCountyId;
            CountyName = issueDTO.ElectionIssue.OhioCounty.Name;
            PrecinctId = issueDTO.OhioPrecinctId;
            Title = issueDTO.ElectionIssue.IssueTitle;
            Requirement = issueDTO.ElectionIssue.IssueRequirement;
            Details = issueDTO.ElectionIssue.IssueDetails;
            Option1Value = issueDTO.ElectionIssue.IssueOption1;
            Option2Value = issueDTO.ElectionIssue.IssueOption2;
            SelectedValue = issueDTO.ElectionIssueId.ToString();
            FullTextUrl = issueDTO.ElectionIssue.IssueFullTextLink;
        }

        public int ElectionIssueId { get; set; }
        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }
        public int CountyId { get; set; }
        public string CountyName { get; set; }
        public int PrecinctId { get; set; }
        public string PrecinctName { get; set; }
        public string Title { get; set; }
        public string Requirement { get; set; }
        public string Details { get; set; }
        public string Option1Value { get; set; }
        public string Option2Value { get; set; }
        public int OptionChecked { get; set; } // 0 = none, 1 = option1, 2 = option2
        public string Option1Checked { get; set; } // checked -or- ""
        public string Option2Checked { get; set; } // checked -or- ""
        public string SelectedValue { get; set; }
        public string FullTextUrl { get; set; }
    }
}