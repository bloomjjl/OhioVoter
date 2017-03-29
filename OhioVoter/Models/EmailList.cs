using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblEmailList")]
    public class EmailList
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsVerified { get; set; }

        public bool IsActive { get; set; }
    }
}