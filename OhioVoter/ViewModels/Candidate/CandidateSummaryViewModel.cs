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
        public CandidateSummaryViewModel() { }

        public CandidateSummaryViewModel(Models.ElectionCandidate selectedCandidateDTO, Models.ElectionCandidate CandidateDTO)
        {
            VotingDate = selectedCandidateDTO.ElectionVotingDate.Date.ToShortDateString();
            VotingDateId = selectedCandidateDTO.ElectionVotingDate.Id;
            SelectedCandidateId = selectedCandidateDTO.Candidate.Id;
            SelectedCandidateOfficeId = selectedCandidateDTO.ElectionOfficeId;
            CandidateSummary = new CandidateSummary()
            {
                CandidateId = CandidateDTO.Candidate.Id,
                VoteSmartCandidateId = CandidateDTO.Candidate.VoteSmartCandidateId,
                VoteSmartPhotoUrl = CandidateDTO.Candidate.VoteSmartPhotoUrl,
                FirstName = CandidateDTO.Candidate.FirstName,
                MiddleName = CandidateDTO.Candidate.MiddleName,
                LastName = CandidateDTO.Candidate.LastName,
                Suffix = CandidateDTO.Candidate.Suffix,
                PartyName = CandidateDTO.Party.PartyName,
                OfficeName = CandidateDTO.ElectionOffice.Office.OfficeName,
                OfficeTerm = CandidateDTO.ElectionOffice.OfficeTerm
            };
        }

        public CandidateSummaryViewModel(Models.ElectionCandidate SelectedCandidateDTO, Models.ElectionCandidate CandidateDTO, Models.ElectionCandidate RuningMateDTO)
        {
            VotingDate = SelectedCandidateDTO.ElectionVotingDate.Date.ToShortDateString();
            VotingDateId = SelectedCandidateDTO.ElectionVotingDate.Id;
            SelectedCandidateId = SelectedCandidateDTO.Candidate.Id;
            SelectedCandidateOfficeId = SelectedCandidateDTO.ElectionOfficeId;
            CandidateSummary = new CandidateSummary()
            {
                CandidateId = CandidateDTO.Candidate.Id,
                VoteSmartCandidateId = CandidateDTO.Candidate.VoteSmartCandidateId,
                VoteSmartPhotoUrl = CandidateDTO.Candidate.VoteSmartPhotoUrl,
                FirstName = CandidateDTO.Candidate.FirstName,
                MiddleName = CandidateDTO.Candidate.MiddleName,
                LastName = CandidateDTO.Candidate.LastName,
                Suffix = CandidateDTO.Candidate.Suffix,
                PartyName = CandidateDTO.Party.PartyName,
                OfficeName = CandidateDTO.ElectionOffice.Office.OfficeName,
                OfficeTerm = CandidateDTO.ElectionOffice.OfficeTerm
            };
            RunningMateSummary = new RunningMateSummary()
            {
                CandidateId = RuningMateDTO.Candidate.Id,
                VoteSmartPhotoUrl = RuningMateDTO.Candidate.VoteSmartPhotoUrl,
                VoteSmartCandidateId = RuningMateDTO.Candidate.VoteSmartCandidateId,
                FirstName = RuningMateDTO.Candidate.FirstName,
                MiddleName = RuningMateDTO.Candidate.MiddleName,
                LastName = RuningMateDTO.Candidate.LastName,
                Suffix = RuningMateDTO.Candidate.Suffix,
                PartyName = RuningMateDTO.Party.PartyName,
                OfficeName = RuningMateDTO.ElectionOffice.Office.OfficeName,
                OfficeTerm = RuningMateDTO.ElectionOffice.OfficeTerm
            };
        }


        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }
        public int SelectedCandidateId { get; set; }
        public int SelectedCandidateOfficeId { get; set; }
        public CandidateSummary CandidateSummary { get; set; }
        public RunningMateSummary RunningMateSummary { get; set; }
    }



    public class CandidateSummary
    {
        public CandidateSummary() { }

        public CandidateSummary(Models.Candidate candidateDTO)
        {
            CandidateId = candidateDTO.Id;
            VoteSmartCandidateId = candidateDTO.VoteSmartCandidateId;
            VoteSmartPhotoUrl = candidateDTO.VoteSmartPhotoUrl;
            FirstName = candidateDTO.FirstName;
            MiddleName = candidateDTO.MiddleName;
            LastName = candidateDTO.LastName;
            Suffix = candidateDTO.Suffix;
            Gender = candidateDTO.Gender;
        }


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



    public class RunningMateSummary
    {
        public RunningMateSummary() { }

        public RunningMateSummary(Models.Candidate candidateDTO)
        {
            CandidateId = candidateDTO.Id;
            VoteSmartCandidateId = candidateDTO.VoteSmartCandidateId;
            VoteSmartPhotoUrl = candidateDTO.VoteSmartPhotoUrl;
            FirstName = candidateDTO.FirstName;
            MiddleName = candidateDTO.MiddleName;
            LastName = candidateDTO.LastName;
            Suffix = candidateDTO.Suffix;
        }


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

}