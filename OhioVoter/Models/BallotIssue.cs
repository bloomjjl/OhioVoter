using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblBallotIssue")]
    public class BallotIssue
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("BallotHeader_Id")]
        public int BallotHeaderId { get; set; }

        [Required]
        [Column("ElectionIssue_Id")]
        public int ElectionIssueId { get; set; }

        [Required]
        public int SelectedOption { get; set; }


        [ForeignKey("BallotHeaderId")]
        public virtual BallotHeader BallotHeader { get; set; }

        [ForeignKey("ElectionIssueId")]
        public virtual ElectionIssue ElectionIssue { get; set; }
    }
}