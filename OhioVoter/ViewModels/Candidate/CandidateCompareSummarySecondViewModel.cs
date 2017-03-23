using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareSummarySecondViewModel
    {
        public CandidateCompareSummarySecondViewModel() { }

        public CandidateCompareSummarySecondViewModel(CandidateCompareDisplayViewModel compareDisplayVM, int totalNumberOfCandidates)
        {
            CandidateFirstDisplayId = compareDisplayVM.CandidateFirstDisplayId;
            CandidateSecondDisplayId = compareDisplayVM.CandidateSecondDisplayId;
            CandidateCompareCount = totalNumberOfCandidates;
            VotingDateId = compareDisplayVM.VotingDateId;
            VotingDate = compareDisplayVM.VotingDate;
            OfficeId = compareDisplayVM.OfficeId;
        }

        public CandidateCompareSummarySecondViewModel(CandidateSummaryViewModel summaryVM, int firstCandidateDisplayId, int secondCandidateDisplayId, int totalNumberOfCandidates)
        {
            VotingDateId = summaryVM.VotingDateId;
            CandidateFirstDisplayId = firstCandidateDisplayId;
            CandidateSecondDisplayId = secondCandidateDisplayId;
            TotalNumberOfCandidates = totalNumberOfCandidates;
            OfficeId = summaryVM.SelectedCandidateOfficeId;
            CandidateCompareSummarySecond = new CandidateCompareSummarySecond()
            {
                CandidateId = summaryVM.CandidateSummary.CandidateId,
                VoteSmartCandidateId = summaryVM.CandidateSummary.VoteSmartCandidateId,
                VoteSmartPhotoUrl = summaryVM.CandidateSummary.VoteSmartPhotoUrl,
                FirstName = summaryVM.CandidateSummary.FirstName,
                MiddleName = summaryVM.CandidateSummary.MiddleName,
                LastName = summaryVM.CandidateSummary.LastName,
                Suffix = summaryVM.CandidateSummary.Suffix,
                PartyName = summaryVM.CandidateSummary.PartyName,
                OfficeName = summaryVM.CandidateSummary.OfficeName,
                OfficeTerm = summaryVM.CandidateSummary.OfficeTerm,
                OfficeHolderName = summaryVM.CandidateSummary.OfficeHolderName,
                Gender = summaryVM.CandidateSummary.Gender
            };
            RunningMateCompareSummarySecond = new RunningMateCompareSummarySecond()
            {
                CandidateId = summaryVM.RunningMateSummary.CandidateId,
                VoteSmartCandidateId = summaryVM.RunningMateSummary.VoteSmartCandidateId,
                VoteSmartPhotoUrl = summaryVM.RunningMateSummary.VoteSmartPhotoUrl,
                FirstName = summaryVM.RunningMateSummary.FirstName,
                MiddleName = summaryVM.RunningMateSummary.MiddleName,
                LastName = summaryVM.RunningMateSummary.LastName,
                Suffix = summaryVM.RunningMateSummary.Suffix,
                PartyName = summaryVM.RunningMateSummary.PartyName,
                OfficeName = summaryVM.RunningMateSummary.OfficeName,
                OfficeTerm = summaryVM.RunningMateSummary.OfficeTerm,
                OfficeHolderName = summaryVM.RunningMateSummary.OfficeHolderName,
                Gender = summaryVM.RunningMateSummary.Gender
            };
        }


        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }
        public int OfficeId { get; set; }
        public int CandidateFirstDisplayId { get; set; }
        public int CandidateSecondDisplayId { get; set; }
        public int TotalNumberOfCandidates { get; set; }
        public int CandidateCompareCount { get; set; }
        public bool CandidateSecondDisplayIsRunningMate { get; set; }
        public CandidateCompareSummarySecond CandidateCompareSummarySecond { get; set; }
        public RunningMateCompareSummarySecond RunningMateCompareSummarySecond { get; set; }
        public List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates { get; set; }
    }


    public class CandidateCompareSummarySecond
    {
        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public string VoteSmartCandidateId { get; set; }
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
        public string Gender { get; set; }
    }

    public class RunningMateCompareSummarySecond
    {
        public int CandidateId { get; set; }
        public string VoteSmartCandidateId { get; set; }
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
        public string Gender { get; set; }
    }

}