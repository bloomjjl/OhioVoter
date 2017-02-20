using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("OhioLocal")]
    public class OhioLocal
    {
        [Key]
        public int OhioLocalId { get; set; }
        public int OhioCountyId { get; set; }
        public string LocalName { get; set; }
        public string LocalType { get; set; }
    }
}