using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("ElectionIssuePrecinct")]
    public class ElectionIssuePrecinct
    {
        [Key]
        [Column(Order = 0)]
        public int ElectionIssueId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ElectionPrecinctId { get; set; }
    }
}