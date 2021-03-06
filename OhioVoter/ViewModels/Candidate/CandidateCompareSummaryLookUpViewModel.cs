﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareSummaryLookUpViewModel
    {
        public CandidateCompareSummaryLookUpViewModel() { }

        public CandidateCompareSummaryLookUpViewModel(int candidateFirstDisplayId, int votingDateId, int officeId, IEnumerable<SelectListItem> candidateNames, string selectedCandidateId)
        {
            CandidateFirstDisplayId = candidateFirstDisplayId;
            OfficeId = officeId;
            VotingDateId = votingDateId;
            SelectedCandidateId = selectedCandidateId;
            CandidateNames = candidateNames;
        }


        public int CandidateFirstDisplayId { get; set; }
        public int OfficeId { get; set; }
        public int VotingDateId { get; set; }

        [Display(Name = "")]
        public string SelectedCandidateId { get; set; }
        public IEnumerable<SelectListItem> CandidateNames { get; set; }
    }

    /*
    public class CandidateCompareDropDownList
    {
        [Display(Name = "")]
        public string SelectedCandidateId { get; set; }
        public IEnumerable<SelectListItem> CandidateNames { get; set; }
    }
    */

}