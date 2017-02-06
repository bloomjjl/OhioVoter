using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class ElectionOffice
    {
        [Required]
        [Key]
        public int ElectionOfficeId { get; set; }

        [Required]
        public int VoteSmartOfficeId { get; set; }

        [Required]
        public string OfficeName { get; set; }

        public string Term { get; set; }
    }
}