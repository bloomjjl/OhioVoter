using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class ElectionCandidate
    {
        [Required]
        [Key]
        public int ElectionCandidateId { get; set; }

        [Required]
        public string VoteSmartCandidateId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Nick Name")]
        public string NickName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Preferred Name")]
        public string PreferredName { get; set; }

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

        [Display(Name = "Birth Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Birth City")]
        public string BirthCity { get; set; }

        [Display(Name = "Birth State Abbreviation")]
        public string BirthState { get; set; }

        public string Gender { get; set; }
    }
}