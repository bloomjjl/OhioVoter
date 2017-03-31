using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblParty")]
    public class Party
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public string PartyCode { get; set; }

        public string PartyAbbreviation { get; set; }

        [Required]
        public string PartyName { get; set; }

        [Required]
        public string PartyColor { get; set; }
    }
}