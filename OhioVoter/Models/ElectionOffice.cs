using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("ElectionOffice")]
    public class ElectionOffice
    {
        [Required]
        [Key]
        public int ElectionOfficeId { get; set; }

        [Required]
        public string VoteSmartOfficeId { get; set; }

        [Required]
        public string OfficeName { get; set; }

        public string Term { get; set; }
    }
}