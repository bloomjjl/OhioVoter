using OhioVoter;
using OhioVoter.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateViewModel
    {
        public Location.SideBarViewModel SideBarViewModel { get; set; }
        public ElectionDateDropDownList ElectionDateDropDownList { get; set; }
        public OfficeDropDownList OfficeDropDownList { get; set; }
        public CandidateDropDownList CandidateDropDownList{ get; set; }

        public Models.ElectionVotingDate ElectionDate { get; set; }

        public CandidateSummary Candidate { get; set; }
        //public IEnumerable<CandidateSummary> Candidates { get; set; }
    }



    public class ElectionDateDropDownList
    {
        [Display(Name = "Election Date")]
        public int SelectedDateId { get; set; }
        public IEnumerable<SelectListItem> Date { get; set; }
    }

    public class OfficeDropDownList
    {
        [Display(Name = "Office")]
        public int SelectedOfficeId { get; set; }
        public IEnumerable<SelectListItem> OfficeNames { get; set; }
    }

    public class CandidateDropDownList
    {
        [Display(Name = "Candidate")]
        public int SelectedCandidateId { get; set; }
        public IEnumerable<SelectListItem> CandidateNames { get; set; }
    }


}