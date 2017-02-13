using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateLookUpViewModel
    {
        public string ControllerName { get; set; }
        public string VotingDate { get; set; }
        //public CandidateDropDownList CandidateDropDownList { get; set; }
        [Display(Name = "")]
        public string SelectedCandidateId { get; set; }
        public IEnumerable<SelectListItem> CandidateNames { get; set; }

    }



    public class ElectionDateDropDownList
    {
        [Display(Name = "Election Date")]
        public string SelectedDateId { get; set; }
        public IEnumerable<SelectListItem> Date { get; set; }
    }

    public class OfficeDropDownList
    {
        [Display(Name = "Office")]
        public string SelectedOfficeId { get; set; }
        public IEnumerable<SelectListItem> OfficeNames { get; set; }
    }

    public class CandidateDropDownList
    {
        [Display(Name = "")]
        public string SelectedCandidateId { get; set; }
        public IEnumerable<SelectListItem> CandidateNames { get; set; }
    }

}