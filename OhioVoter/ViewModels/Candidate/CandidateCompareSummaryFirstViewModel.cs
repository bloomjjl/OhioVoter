using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareSummaryFirstViewModel
    {
        public int VotingDateId { get; set; }
        public int OfficeId { get; set; }
        public int CandidateFirstDisplayId { get; set; }
        public int CandidateSecondDisplayId { get; set; }
        public int TotalNumberOfCandidates { get; set; }
        public bool CandidateFirstDisplayIsRunningMate { get; set; }
        public CandidateCompareSummaryFirst CandidateCompareSummaryFirst { get; set; }
        public RunningMateCompareSummaryFirst RunningMateCompareSummaryFirst { get; set; }
    }



    public class CandidateCompareSummaryFirst
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


    public class RunningMateCompareSummaryFirst
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
    

}