using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateOfficeListViewModel
    {
        public CandidateOfficeListViewModel() { }

        public int ElectionOfficeId { get; set; }
        public string ElectionOfficeName { get; set; }

    }
}