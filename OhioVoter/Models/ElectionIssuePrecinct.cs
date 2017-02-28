using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblElectionIssuePrecinct")]
    public class ElectionIssuePrecinct
    {
        [Required]
        [Key]
        [Column("ElectionIssue_Id", Order = 0)]
        public int ElectionIssueId { get; set; }

        [Required]
        [Key]
        [Column("OhioPrecinct_Id", Order = 1)]
        public int OhioPrecinctId { get; set; }



        [ForeignKey("ElectionIssueId")]
        public virtual ElectionIssue ElectionIssue { get; set; }

        [ForeignKey("OhioPrecinctId")]
        public virtual OhioPrecinct OhioPrecinct { get; set; }

    }
}