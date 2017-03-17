using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblApi")]
    public class Api
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public string ApiUrl { get; set; }

        public string ApiKey { get; set; }
    }
}