using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class OfficeHolder
    {
        [Required]
        [Key]
        public string OfficeHolderId { get; set; }

        [Required]
        public string OfficeHolderName { get; set; }
    }
}