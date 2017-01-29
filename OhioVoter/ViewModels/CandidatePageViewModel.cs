using OhioVoter.ViewModels.VoteSmart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels
{
    public class CandidatePageViewModel
    {
        public SideBar SideBar { get; set; }
        public IEnumerable<Candidate> Candidates { get; set; }
        public IEnumerable<Office> Offices { get; set; }
    }

    /*
    // Change to VoteSmart object
    public class Candidate
    {
        public string Name { get; set; }
    }

    // Change to VoteSmart Object
    public class Office
    {
        public string Name { get; set; }
    }
    */

}