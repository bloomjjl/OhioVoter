using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblVoterList_Hamilton")]
    public class HamiltonOhioVoter
    {
        public HamiltonOhioVoter()
        {

        }

        [Required]
        [Key]
        public int Id { get; set; }

        public int HamiltonVoterId { get; set; }

        public int HamiltonPrecinctNumber { get; set; }

        public int HamiltonPrecinctSplit { get; set; }

        [Required]
        [Column("OhioPrecinct_Id")]
        public int OhioPrecinctId { get; set; }

        public DateTime RegisteredDate { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string SuffixName { get; set; }

        public string PhoneNumber { get; set; }

        public string Status { get; set; }

        public string PartyCode { get; set; }

        public string PartyAbbreviation { get; set; }

        public string BirthYear { get; set; }

        public string AddressPreDirectional_Short { get; set; }

        public string AddressPreDirectional_Long { get; set; }

        [Required]
        public int AddressNumber { get; set; }

        public string AddressFraction { get; set; }

        [Required]
        public string AddressStreet { get; set; }

        public string AddressSuffix_Short { get; set; }

        public string AddressSuffix_Long { get; set; }

        public string AddressOther { get; set; }

        public string AddressCityName { get; set; }

        public string StateAbbreviation { get; set; }

        [Required]
        public int AddressZip { get; set; }

        [Required]
        [Column("OhioCounty_Id")]
        public int OhioCountyId { get; set; }

        [Required]
        [Column("CourtOfAppealsOffice_Id")]
        public int CourtOfAppealsOfficeId { get; set; }

        [Required]
        public string CourtOfAppealsOfficeCode { get; set; }

        [Required]
        [Column("StateBoardOfEducation_Id")]
        public int StateBoardOfEducationId { get; set; }

        [Required]
        public string StateBoardOfEducationCode { get; set; }

        [Required]
        public string CongressOfficeCode { get; set; }

        [Required]
        public string SenateOfficeCode { get; set; }

        [Required]
        public string HouseOfficeCode { get; set; }

        [Required]
        public string JudicialOfficeCode { get; set; }

        [Required]
        public string SchoolOfficeCode { get; set; }

        public string CountySchoolOfficeCode { get; set; }

        public string VocationalSchoolOfficeCode { get; set; }



        [ForeignKey("OhioPrecinctId")]
        public virtual OhioPrecinct OhioPrecinct { get; set; }

        [ForeignKey("OhioCountyId")]
        public virtual OhioCounty OhioCounty { get; set; }

        [ForeignKey("StateBoardOfEducationId")]
        public virtual StateBoardOfEducation StateBoardOfEducation { get; set; }
    }
}