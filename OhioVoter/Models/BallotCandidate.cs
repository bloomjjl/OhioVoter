using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblBallotCandidate")]
    public class BallotCandidate
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("BallotOffice_Id")]
        public int BallotOfficeId { get; set; }

        [Required]
        [Column("ElectionCandidate_Id")]
        public int ElectionCandidateId { get; set; }


        [ForeignKey("BallotOfficeId")]
        public virtual BallotOffice BallotOffice { get; set; }

        [ForeignKey("ElectionCandidateId")]
        public virtual ElectionCandidate ElectionCandidate { get; set; }
    }
}