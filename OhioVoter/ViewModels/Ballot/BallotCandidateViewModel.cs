using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Ballot
{
    public class BallotCandidateViewModel
    {
        public BallotCandidateViewModel() { }

        public BallotCandidateViewModel(Models.ElectionCandidate candidateDTO)
        {
            PartyId = candidateDTO.PartyId;
            PartyName = candidateDTO.Party.PartyName;
            CertifiedCandidateId = candidateDTO.CertifiedCandidateId;

            CandidateId = candidateDTO.CandidateId;
            VoteSmartCandidateId = candidateDTO.Candidate.VoteSmartCandidateId;
            CandidatePhoto = candidateDTO.Candidate.VoteSmartPhotoUrl;
            CandidateFirstName = candidateDTO.Candidate.FirstName;
            CandidateMiddleName = candidateDTO.Candidate.MiddleName;
            CandidateLastName = candidateDTO.Candidate.LastName;
            CandidateSuffix = candidateDTO.Candidate.Suffix;
            CandidateGender = candidateDTO.Candidate.Gender;

            RunningMateId = candidateDTO.RunningMateId;
        }

        public BallotCandidateViewModel(BallotCandidateViewModel candidateVM, Models.Candidate runningmateDTO)
        {
            PartyId = candidateVM.PartyId;
            PartyName = candidateVM.PartyName;
            CertifiedCandidateId = candidateVM.CertifiedCandidateId;

            CandidateId = candidateVM.CandidateId;
            VoteSmartCandidateId = candidateVM.VoteSmartCandidateId;
            CandidatePhoto = candidateVM.CandidatePhoto;
            CandidateFirstName = candidateVM.CandidateFirstName;
            CandidateMiddleName = candidateVM.CandidateMiddleName;
            CandidateLastName = candidateVM.CandidateLastName;
            CandidateSuffix = candidateVM.CandidateSuffix;
            CandidateGender = candidateVM.CandidateGender;

            RunningMateId = runningmateDTO.Id;
            VoteSmartRunningMateId = runningmateDTO.VoteSmartCandidateId;
            VoteSmartRunningMatePhoto = runningmateDTO.VoteSmartPhotoUrl;
            RunningMatePhoto = runningmateDTO.VoteSmartPhotoUrl;
            RunningMateFirstName = runningmateDTO.FirstName;
            RunningMateMiddleName = runningmateDTO.MiddleName;
            RunningMateLastName = runningmateDTO.LastName;
            RunningMateSuffix = runningmateDTO.Suffix;
            RunningMateGender = runningmateDTO.Gender;
        }


        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public string CertifiedCandidateId { get; set; }
        public bool IsSelected { get; set; }

        public int CandidateId { get; set; }
        public string VoteSmartCandidateId { get; set; }
        public string VoteSmartCandidatePhoto { get; set; }
        public string CandidatePhoto { get; set; }
        public string CandidateFirstName { get; set; }
        public string CandidateMiddleName { get; set; }
        public string CandidateLastName { get; set; }
        public string CandidateSuffix { get; set; }
        public string CandidateGender { get; set; }

        public string CandidateName
        {
            get
            {
                string FirstAndMiddleName = CandidateFirstName;
                string LastAndSuffixName = CandidateLastName;

                if (CandidateMiddleName != null && CandidateMiddleName != "")
                {
                    FirstAndMiddleName = string.Concat(CandidateFirstName, " ", CandidateMiddleName);
                }
                if (CandidateSuffix != null && CandidateSuffix != "")
                {
                    LastAndSuffixName = string.Concat(CandidateLastName, " ", CandidateSuffix);
                }

                return string.Format("{0} {1}", FirstAndMiddleName, LastAndSuffixName);
            }
        }

        public int RunningMateId { get; set; }
        public string VoteSmartRunningMateId { get; set; }
        public string VoteSmartRunningMatePhoto { get; set; }
        public string RunningMatePhoto { get; set; }
        public string RunningMateFirstName { get; set; }
        public string RunningMateMiddleName { get; set; }
        public string RunningMateLastName { get; set; }
        public string RunningMateSuffix { get; set; }
        public string RunningMateGender { get; set; }

        public string RunningMateName
        {
            get
            {
                string FirstAndMiddleName = RunningMateFirstName;
                string LastAndSuffixName = RunningMateLastName;

                if (RunningMateMiddleName != null && RunningMateMiddleName != "")
                {
                    FirstAndMiddleName = string.Concat(RunningMateFirstName, " ", RunningMateMiddleName);
                }
                if (RunningMateSuffix != null && RunningMateSuffix != "")
                {
                    LastAndSuffixName = string.Concat(RunningMateLastName, " ", RunningMateSuffix);
                }

                return string.Format("{0} {1}", FirstAndMiddleName, LastAndSuffixName);
            }
        }
    }
}