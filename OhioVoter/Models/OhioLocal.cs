﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblOhioLocal")]
    public class OhioLocal
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("OhioCounty_Id")]
        public int OhioCountyId { get; set; }

        public string LocalName { get; set; }

        public string LocalType { get; set; }



        [ForeignKey("OhioCountyId")]
        public virtual OhioCounty OhioCounty { get; set; }

    }
}