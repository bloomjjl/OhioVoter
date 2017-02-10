using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateSummaryViewModel
    {
        public CandidateSummary CandidateSummary { get; set; }
        public RunningMateSummary RunningMateSummary { get; set; }
    }



    public class CandidateSummary
    {
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