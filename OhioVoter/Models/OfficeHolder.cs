﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("OfficeHolder")]
    public class OfficeHolder
    {
        [Required]
        [Key]
        public string OfficeHolderId { get; set; }

        [Required]
        public string OfficeHolderName { get; set; }
    }
}