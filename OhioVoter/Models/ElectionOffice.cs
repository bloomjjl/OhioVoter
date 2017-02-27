using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblElectionOffice")]
    public class ElectionOffice
    {
        public ElectionOffice()
        {

        }

        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("ElectionVotingDate_Id")]
        public int ElectionVotingDateId { get; set; }

        [Required]
        [Column("Office_Id")]
        public int OfficeId { get; set; }

        public string Term { get; set; }



        [ForeignKey("ElectionVotingDateId")]
        public virtual ElectionVotingDate ElectionVotingDate { get; set; }

        [ForeignKey("OfficeId")]
        public virtual Office Office { get; set; }

    }
}