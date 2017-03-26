using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Issue
{
    public class IssueLookUpViewModel
    {
        public IssueLookUpViewModel()
        {

        }


        public string ControllerName { get; set; }

        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }

        //public VotingDateDropDownList VotingDateDropDownList { get; set; }
        //[Display(Name = "Voting Date")]
        //public string SelectedVotingDateId { get; set; }
        //public IEnumerable<SelectListItem> VotingDates { get; set; }

        //public CountyDropDownList CountyDropDownList { get; set; }
        [Display(Name = "County")]
        public string SelectedCountyId { get; set; }
        public IEnumerable<SelectListItem> CountyNames { get; set; }

        //public CommunityDropDownList CommunityDropDownList { get; set; }
        [Display(Name = "Community")]
        public string SelectedCommunityId { get; set; }
        public IEnumerable<SelectListItem> CommunityNames { get; set; }
    }

    /*
    public class VotingDateDropDownList
    {
        [Display(Name = "Voting Date")]
        public string SelectedVotingDateId { get; set; }
        public IEnumerable<SelectListItem> VotingDates { get; set; }
    }

    public class CountyDropDownList
    {
        [Display(Name = "County")]
        public string SelectedCountyId { get; set; }
        public IEnumerable<SelectListItem> CountyNames { get; set; }
    }

    public class CommunityDropDownList
    {
        [Display(Name = "Community")]
        public string SelectedCommunityId { get; set; }
        public IEnumerable<SelectListItem> CommunityNames { get; set; }
    }
    */

}
