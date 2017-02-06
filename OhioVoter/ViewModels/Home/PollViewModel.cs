using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Home
{
    public class PollViewModel
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ElectionDate { get; set; }

        public string OfficeName { get; set; }

        public IEnumerable<CandidateVote> CandidateVotes { get; set; }
    }

    public class CandidateVote
    {
        public String Candidate { get; set; }
        public String CoCandidate { get; set; }
        public string Party { get; set; }
        public string PartyColor { get; set; }
        public int VoteCount { get; set; }
        public decimal VotePercent { get; set; }
    }

}