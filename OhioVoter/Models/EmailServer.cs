using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblEmailServer")]
    public class EmailServer
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Website { get; set; }

        [Required]
        public string SmtpServerName { get; set; }

        [Required]
        public string SmtpUserName { get; set; }

        [Required]
        public string SmtpPassword { get; set; }
    }
}