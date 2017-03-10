using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblOhioBoardOfElection")]
    public class OhioBoardOfElection
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public string StreetAddress1 { get; set; }

        public string StreetAddress2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public int ZipCode { get; set; }

        public int CountyId { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }


        [ForeignKey("CountyId")]
        public virtual OhioCounty OhioCounty { get; set; }

    }
}