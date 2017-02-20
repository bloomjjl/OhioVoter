using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("ElectionPrecinct")]
    public class ElectionPrecinct
    {
        [Key]
        public int ElectionPrecinctId { get; set; }
        public string PrecinctName { get; set; }
        public int HamiltonCountyPrecinctId { get; set; }
        public string OhioPrecinctCode { get; set; }
        public int OhioCountyId { get; set; }
        public int LocalId { get; set; }
    }
}