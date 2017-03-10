using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Ballot
{
    public class BallotViewModel
    {
        public BallotViewModel() { }

        public BallotViewModel(string controllerName, string dateId, IEnumerable<SelectListItem> dropDownListOfDates)
        {
            ControllerName = controllerName;
            SelectedVotingDateId = dateId;
            VotingDates = dropDownListOfDates;
        }

        public BallotViewModel(BallotViewModel ballotVM, BallotVoterViewModel ballotVoterVM, List<BallotOfficeViewModel> ballotOfficeVM, List<BallotIssueViewModel> ballotIssueVM)
        {
            ControllerName = ballotVM.ControllerName;
            SelectedVotingDateId = ballotVM.SelectedVotingDateId;
            VotingDates = ballotVM.VotingDates;
            BallotVoterViewModel = ballotVoterVM;
            BallotOfficeViewModel = ballotOfficeVM;
            BallotIssueViewModel = ballotIssueVM;
        }


        public string ControllerName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string VoterEmailAddress { get; set; }

        [Display(Name = "Election Date")]
        public string SelectedVotingDateId { get; set; }
        public IEnumerable<SelectListItem> VotingDates { get; set; }

        public BallotVoterViewModel BallotVoterViewModel { get; set; }
        public List<BallotOfficeViewModel> BallotOfficeViewModel { get; set; }
        public List<BallotIssueViewModel> BallotIssueViewModel { get; set; }
    }
}