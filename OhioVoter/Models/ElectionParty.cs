using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("ElectionParty")]
    public class ElectionParty
    {
        [Required]
        [Key]
        public string PartyId { get; set; }

        [Required]
        public string PartyName { get; set; }
    }
}