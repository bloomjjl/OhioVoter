using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("OhioCounty")]
    public class OhioCounty
    {
        [Key]
        public int OhioCountyId { get; set; }
        public string CountyName { get; set; }
    }
}