using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("ElectionIssue")]
    public class ElectionIssue
    {
        [Key]
        public int ElectionIssueId { get; set; }
        public int ElectionVotingDateId { get; set; }
        public int OhioCountyId { get; set; }
        public string IssueTitle { get; set; }
        public string IssueRequirement { get; set; }
        public string IssueDetails { get; set; }
        public string IssueOption1 { get; set; }
        public string IssueOption2 { get; set; }
        public string IssueFullTextLink { get; set; }
    }
}