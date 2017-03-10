using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblCandidate")]
    public class Candidate
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string VoteSmartCandidateId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Suffix { get; set; }

        public string CandidateName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string Gender { get; set; }

        public string Photo { get; set; }

        public string VoteSmartPhotoUrl { get; set; }
    }
}
