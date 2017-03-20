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
        public CandidateViewModel() { }

        public CandidateViewModel(string controllerName, int candidateId)
        {
            ControllerName = controllerName;
            CandidateId = candidateId;
        }

        public CandidateViewModel(string controllerName, int candidateId, CandidateLookUpViewModel candidateLookUpVM)
        {
            ControllerName = controllerName;
            CandidateId = candidateId;
            CandidateLookUpViewModel = candidateLookUpVM;
        }

        public string ControllerName { get; set; }
        public int CandidateId { get; set; }
        public CandidateLookUpViewModel CandidateLookUpViewModel { get; set; }
        public CandidateDisplayViewModel CandidateDisplayViewModel { get; set; }
    }

}