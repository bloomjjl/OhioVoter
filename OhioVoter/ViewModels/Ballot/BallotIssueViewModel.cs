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
            Id = issueDTO.ElectionIssueId;
            VotingDateId = issueDTO.ElectionIssue.ElectionVotingDateId;
            VotingDate = issueDTO.ElectionIssue.ElectionVotingDate.Date.ToShortDateString();
            CountyId = issueDTO.ElectionIssue.OhioCountyId;
            CountyName = issueDTO.ElectionIssue.OhioCounty.Name;
            PrecinctId = issueDTO.OhioPrecinctId;
            Title = issueDTO.ElectionIssue.IssueTitle;
            Requirement = issueDTO.ElectionIssue.IssueRequirement;
            Details = issueDTO.ElectionIssue.IssueDetails;
            Option1 = issueDTO.ElectionIssue.IssueOption1;
            Option2 = issueDTO.ElectionIssue.IssueOption2;
            SelectedOption = issueDTO.ElectionIssueId.ToString();
            FullTextUrl = issueDTO.ElectionIssue.IssueFullTextLink;
        }

        public int Id { get; set; }
        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }
        public int CountyId { get; set; }
        public string CountyName { get; set; }
        public int PrecinctId { get; set; }
        public string PrecinctName { get; set; }
        public string Title { get; set; }
        public string Requirement { get; set; }
        public string Details { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string SelectedOption { get; set; }
        public string FullTextUrl { get; set; }
    }
}