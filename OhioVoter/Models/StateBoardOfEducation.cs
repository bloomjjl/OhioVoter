using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblStateBoardOfEducation")]
    public class StateBoardOfEducation
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public int District { get; set; }

        public int IRN { get; set; }

        public string StateBoardOfEducationCode { get; set; }

        public int SchoolDistrictRegion { get; set; }

        [Column("SchoolDistrict_Id")]
        public int SchoolDistrictId { get; set; }

        [Column("County_Id")]
        public int CountyId { get; set; }


        [ForeignKey("SchoolDistrictId")]
        public virtual OhioSchoolDistrict OhioSchoolDistrict { get; set; }

        [ForeignKey("CountyId")]
        public virtual OhioCounty OhioCounty { get; set; }

    }
}