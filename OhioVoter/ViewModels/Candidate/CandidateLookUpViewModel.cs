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
        public CandidateLookUpViewModel() { }

        public CandidateLookUpViewModel(string controllerName)
        {
            ControllerName = controllerName;
        }

        public CandidateLookUpViewModel(string controllerName, string date, List<CandidateListViewModel>  candidateListVM, IEnumerable<SelectListItem>  electionOfficeSelectList)
        {
            ControllerName = controllerName;
            VotingDate = date;
            CandidateListViewModel = candidateListVM;
            ElectionOfficeNames = electionOfficeSelectList;
        }


        public string ControllerName { get; set; }
        public string VotingDate { get; set; }

        [Display(Name = "")]
        public string SelectedElectionOfficeId { get; set; }
        public IEnumerable<SelectListItem> ElectionOfficeNames { get; set; }

        //[Display(Name = "")]
        //public string SelectedCandidateId { get; set; }
        //public IEnumerable<SelectListItem> CandidateNames { get; set; }
        public string CandidateName { get; set; }
        public List<CandidateListViewModel> CandidateListViewModel { get; set; }
    }


    /*
    public class ElectionDateDropDownList
    {
        [Display(Name = "Election Date")]
        public string SelectedDateId { get; set; }
        public IEnumerable<SelectListItem> Date { get; set; }
    }
    */

    /*
    public class ElectionOfficeDropDownList
    {
        [Display(Name = "Office")]
        public string SelectedElectionOfficeId { get; set; }
        public IEnumerable<SelectListItem> ElectionOfficeNames { get; set; }
    }
    */

    public class CandidateDropDownList
    {
        [Display(Name = "")]
        public string SelectedCandidateId { get; set; }
        public IEnumerable<SelectListItem> CandidateNames { get; set; }
    }

}