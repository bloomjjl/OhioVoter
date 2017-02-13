﻿using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateSummaryViewModel
    {
        public int SelectedCandidateId { get; set; }
        public int SelectedCandidateOfficeId { get; set; }
        public int SelectedCandidateVotingDateId { get; set; }
        public CandidateSummary CandidateSummary { get; set; }
        public RunningMateSummary RunningMateSummary { get; set; }
    }



    public class CandidateSummary
    {
        public int CandidateId { get; set; }
        public string VoteSmartPhotoUrl { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        public string PartyName { get; set; }
        public string OfficeName { get; set; }
        public string OfficeTerm { get; set; }
        public string OfficeHolderName { get; set; }
    }



    public class RunningMateSummary
    {
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public string VoteSmartPhotoUrl { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        public string PartyName { get; set; }
        public string OfficeName { get; set; }
        public string OfficeTerm { get; set; }
        public string OfficeHolderName { get; set; }
    }

}