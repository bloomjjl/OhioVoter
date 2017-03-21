using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateListViewModel
    {
        public CandidateListViewModel() { }

        public CandidateListViewModel(ElectionCandidate candidateDTO)
        {
            ElectionCandidateId = candidateDTO.Id;
            CandidateId = candidateDTO.CandidateId;
            PhotoUrl = candidateDTO.Candidate.VoteSmartPhotoUrl;
            CandidateName = candidateDTO.Candidate.CandidateFirstLastName;
            PartyId = candidateDTO.PartyId;
            PartyName = candidateDTO.Party.PartyName;
            ElectionOfficeId = candidateDTO.ElectionOfficeId;
            ElectionOfficeName = candidateDTO.ElectionOffice.Office.OfficeName;
            ElectionOfficeTerm = candidateDTO.ElectionOffice.OfficeTerm;
        }


        public string PhotoUrl { get; set; }
        public int ElectionCandidateId { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public int ElectionOfficeId { get; set; }
        public string ElectionOfficeName { get; set; }
        public string ElectionOfficeTerm { get; set; }
    }
}