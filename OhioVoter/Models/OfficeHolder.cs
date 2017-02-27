using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblOfficeHolder")]
    public class OfficeHolder
    {
        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }
    }
}