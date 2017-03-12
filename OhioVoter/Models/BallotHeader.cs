using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblBallotHeader")]
    public class BallotHeader
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("ElectionVotingDate_Id")]
        public int ElectionVotingDateId { get; set; }

        [Required]
        [Column("User_Id")]
        public int UserId { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public DateTime DateEmailed { get; set; }


        [ForeignKey("ElectionVotingDateId")]
        public virtual ElectionVotingDate ElectionVotingDate { get; set; }

    }
}