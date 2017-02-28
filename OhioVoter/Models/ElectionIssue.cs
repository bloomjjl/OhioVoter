using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblElectionIssue")]
    public class ElectionIssue
    {
        public ElectionIssue()
        {

        }

        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("ElectionVotingDate_Id")]
        public int ElectionVotingDateId { get; set; }

        [Required]
        [Column("OhioCounty_Id")]
        public int OhioCountyId { get; set; }

        [Required]
        public string IssueTitle { get; set; }

        [Required]
        public string IssueRequirement { get; set; }

        [Required]
        public string IssueDetails { get; set; }

        public string IssueOption1 { get; set; }

        public string IssueOption2 { get; set; }

        public string IssueFullTextLink { get; set; }


        [ForeignKey("ElectionVotingDateId")]
        public virtual ElectionVotingDate ElectionVotingDate { get; set; }

        [ForeignKey("OhioCountyId")]
        public virtual OhioCounty OhioCounty { get; set; }

    }
}