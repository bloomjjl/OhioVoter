using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class ElectionVotingDate
    {
        [Required]
        [Key]
        public int ElectionVotingDateId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool Active { get; set; }

    }
}