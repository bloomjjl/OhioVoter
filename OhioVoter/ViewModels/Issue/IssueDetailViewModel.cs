using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Issue
{
    public class IssueDetailViewModel
    {
        public int DateId { get; set; }
        public int CountyId { get; set; }
        public int CommunityId { get; set; }

        public int IssueId { get; set; }
        public string IssueTitle { get; set; }
        public string IssueRequirement { get; set; }
        public string IssueDetails { get; set; }
        public string IssueFullTextLink { get; set; }
        public string IssueOption1 { get; set; }
        public string IssueOption2 { get; set; }
    }
}