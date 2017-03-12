using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblBallotOffice")]
    public class BallotOffice
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("BallotHeader_Id")]
        public int BallotHeaderId { get; set; }

        [Required]
        [Column("ElectionOffice_Id")]
        public int ElectionOfficeId { get; set; }


        [ForeignKey("BallotHeaderId")]
        public virtual BallotHeader BallotHeader { get; set; }

        [ForeignKey("ElectionOfficeId")]
        public virtual ElectionOffice ElectionOffice { get; set; }
    }
}