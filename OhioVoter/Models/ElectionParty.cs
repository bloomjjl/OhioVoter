using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class ElectionParty
    {
        [Required]
        [Key]
        public string PartyId { get; set; }

        [Required]
        public string PartyName { get; set; }
    }
}