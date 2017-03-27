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

        public BallotViewModel(string controllerName, int dateId, string date)
        {
            ControllerName = controllerName;
            VotingDateId = dateId;
            VotingDate = date;
        }

        public BallotViewModel(BallotViewModel ballotVM, BallotVoterViewModel ballotVoterVM, List<BallotOfficeViewModel> ballotOfficeVM, List<BallotIssueViewModel> ballotIssueVM)
        {
            ControllerName = ballotVM.ControllerName;
            VotingDateId = ballotVM.VotingDateId;
            VotingDate = ballotVM.VotingDate;
            BallotVoterViewModel = ballotVoterVM;
            BallotOfficeViewModel = ballotOfficeVM;
            BallotIssueViewModel = ballotIssueVM;
        }


        public string ControllerName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string VoterEmailAddress { get; set; }

        public int PrecinctId { get; set; }
        public string PrecinctName { get; set; }

        [Display(Name = "Election Date")]
        public string VotingDate { get; set; }
        public int VotingDateId { get; set; }
        //public string SelectedVotingDateId { get; set; }
        //public IEnumerable<SelectListItem> VotingDates { get; set; }

        public BallotVoterViewModel BallotVoterViewModel { get; set; }
        public List<BallotOfficeViewModel> BallotOfficeViewModel { get; set; }
        public List<BallotIssueViewModel> BallotIssueViewModel { get; set; }
    }
}