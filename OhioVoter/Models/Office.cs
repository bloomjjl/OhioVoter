using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblOffice")]
    public class Office
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string VoteSmartOfficeId { get; set; }

        [Required]
        public string OfficeName { get; set; }

        public string OfficeCode { get; set; }

        public string OfficeWebsite { get; set; }
    }
}