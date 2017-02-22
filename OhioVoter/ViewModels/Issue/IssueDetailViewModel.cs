using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Title")]
        public string IssueTitle { get; set; }

        [Display(Name = "Requirement")]
        public string IssueRequirement { get; set; }

        [Display(Name = "Details")]
        public string IssueDetails { get; set; }

        [Display(Name = "Full Text Link")]
        public string IssueFullTextLink { get; set; }

        [Display(Name = "Option 1")]
        public string IssueOption1 { get; set; }

        [Display(Name = "Option 2")]
        public string IssueOption2 { get; set; }
    }
}