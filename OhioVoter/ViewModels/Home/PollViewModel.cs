using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Home
{
    public class PollViewModel
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string ElectionDate { get; set; }

        //public string OfficeName { get; set; }
        [Display(Name = "Lookup By Office:")]
        public string SelectedElectionOfficeId { get; set; }
        public IEnumerable<SelectListItem> ElectionOfficeNames { get; set; }

        public IEnumerable<CandidateVoteViewModel> CandidateVotes { get; set; }
    }

    public class CandidateVoteViewModel
    {
        public String Candidate { get; set; }
        public String CoCandidate { get; set; }
        public string Party { get; set; }
        public string PartyColor { get; set; }
        public int VoteCount { get; set; }
        public decimal VotePercent { get; set; }
        public string ImageUrl { get; set; }
    }

}