using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblOhioSchoolDistrict")]
    public class OhioSchoolDistrict
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public int SchoolDistrictNumber { get; set; }

        public string HamiltonSchoolCode { get; set; }

        public string Description { get; set; }


    }
}