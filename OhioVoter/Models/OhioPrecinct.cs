using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblOhioPrecinct")]
    public class OhioPrecinct
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public string CountyPrecinctCode { get; set; }

        public string PrecinctName { get; set; }

        [Required]
        [Column("OhioCounty_Id")]
        public int OhioCountyId { get; set; }

        [Required]
        [Column("HamiltonCountyPrecinct_Id")]
        public int HamiltonCountyPrecinctId { get; set; }

        [Required]
        [Column("OhioLocal_Id")]
        public int OhioLocalId { get; set; }



        [ForeignKey("OhioCountyId")]
        public virtual OhioCounty OhioCounty { get; set; }

        [ForeignKey("OhioLocalId")]
        public virtual OhioLocal OhioLocal { get; set; }

    }
}