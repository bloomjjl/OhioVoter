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

        public CandidateSummaryViewModel(Models.ElectionCandidate selectedCandidateDTO, Models.ElectionCandidate candidateDTO)
        {
            VotingDate = selectedCandidateDTO.ElectionVotingDate.Date.ToShortDateString();
            VotingDateId = selectedCandidateDTO.ElectionVotingDate.Id;
            SelectedCandidateId = selectedCandidateDTO.Candidate.Id;
            SelectedCandidateOfficeId = selectedCandidateDTO.ElectionOfficeId;

            CandidateSummary = new CandidateSummary()
            {
                CandidateId = candidateDTO.Candidate.Id,
                VoteSmartCandidateId = candidateDTO.Candidate.VoteSmartCandidateId,
                VoteSmartPhotoUrl = candidateDTO.Candidate.VoteSmartPhotoUrl,
                GenderPhotUrl = candidateDTO.Candidate.GenderPhotoUrl,
                FirstName = candidateDTO.Candidate.FirstName,
                MiddleName = candidateDTO.Candidate.MiddleName,
                LastName = candidateDTO.Candidate.LastName,
                Suffix = candidateDTO.Candidate.Suffix,
                PartyName = candidateDTO.Party.PartyName,
                OfficeName = candidateDTO.ElectionOffice.Office.OfficeName,
                OfficeTerm = candidateDTO.ElectionOffice.OfficeTerm
            };
        }

        public CandidateSummaryViewModel(Models.ElectionCandidate SelectedCandidateDTO, Models.ElectionCandidate candidateDTO, Models.ElectionCandidate runingMateDTO)
        {
            VotingDate = SelectedCandidateDTO.ElectionVotingDate.Date.ToShortDateString();
            VotingDateId = SelectedCandidateDTO.ElectionVotingDate.Id;
            SelectedCandidateId = SelectedCandidateDTO.Candidate.Id;
            SelectedCandidateOfficeId = SelectedCandidateDTO.ElectionOfficeId;

            CandidateSummary = new CandidateSummary()
            {
                CandidateId = candidateDTO.Candidate.Id,
                VoteSmartCandidateId = candidateDTO.Candidate.VoteSmartCandidateId,
                VoteSmartPhotoUrl = candidateDTO.Candidate.VoteSmartPhotoUrl,
                GenderPhotUrl = candidateDTO.Candidate.GenderPhotoUrl,
                FirstName = candidateDTO.Candidate.FirstName,
                MiddleName = candidateDTO.Candidate.MiddleName,
                LastName = candidateDTO.Candidate.LastName,
                Suffix = candidateDTO.Candidate.Suffix,
                PartyName = candidateDTO.Party.PartyName,
                OfficeName = candidateDTO.ElectionOffice.Office.OfficeName,
                OfficeTerm = candidateDTO.ElectionOffice.OfficeTerm
            };
            RunningMateSummary = new RunningMateSummary()
            {
                CandidateId = runingMateDTO.Candidate.Id,
                VoteSmartPhotoUrl = runingMateDTO.Candidate.VoteSmartPhotoUrl,
                GenderPhotUrl = runingMateDTO.Candidate.GenderPhotoUrl,
                VoteSmartCandidateId = runingMateDTO.Candidate.VoteSmartCandidateId,
                FirstName = runingMateDTO.Candidate.FirstName,
                MiddleName = runingMateDTO.Candidate.MiddleName,
                LastName = runingMateDTO.Candidate.LastName,
                Suffix = runingMateDTO.Candidate.Suffix,
                PartyName = runingMateDTO.Party.PartyName,
                OfficeName = runingMateDTO.ElectionOffice.Office.OfficeName,
                OfficeTerm = runingMateDTO.ElectionOffice.OfficeTerm
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
            GenderPhotUrl = candidateDTO.GenderPhotoUrl;
            FirstName = candidateDTO.FirstName;
            MiddleName = candidateDTO.MiddleName;
            LastName = candidateDTO.LastName;
            Suffix = candidateDTO.Suffix;
            Gender = candidateDTO.Gender;
        }


        public int CandidateId { get; set; }
        public string VoteSmartCandidateId { get; set; }
        public string VoteSmartPhotoUrl { get; set; }
        public string GenderPhotUrl { get; set; }
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
            GenderPhotUrl = candidateDTO.GenderPhotoUrl;
            FirstName = candidateDTO.FirstName;
            MiddleName = candidateDTO.MiddleName;
            LastName = candidateDTO.LastName;
            Suffix = candidateDTO.Suffix;
        }


        public int CandidateId { get; set; }
        public int RunningMateId { get; set; }
        public string VoteSmartCandidateId { get; set; }
        public string GenderPhotUrl { get; set; }
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