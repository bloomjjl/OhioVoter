using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateSummary
    {
        public int CandidateLookUpId { get; set; }

        public int ElectionVotingDateId { get; set; }
        public DateTime VotingDate { get; set; }

        public string CertifiedCandidateId { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public string OfficeHolderId { get; set; }
        public string OfficeHolderName { get; set; }

        public int CandidateId { get; set; }
        public string VoteSmartCandidateId { get; set; }
        public string OpenSecretsCandidateId { get; set; }
        public string VoteSmartCandidatePhotoUrl { get; set; }
        public string CandidateFirstName { get; set; }
        public string CandidateMiddleName { get; set; }
        public string CandidateLastName { get; set; }
        public string CandidateSuffix { get; set; }
        public string CandidateName
        {
            get
            {
                return string.Format("{0} {1}", CandidateFirstName, CandidateLastName);
            }
        }

        public int CandidateOfficeId { get; set; }
        public string VoteSmartCandidateOfficeId { get; set; }
        public string CandidateOfficeName { get; set; }
        public string CandidateOfficeTerm { get; set; }

        public string VoteSmartCandidateNickName { get; set; }
        public string VoteSmartCandidateMiddleName { get; set; }
        public string VoteSmartCandidatePreferredName { get; set; }
        public string VoteSmartCandidateBirthDate { get; set; }
        public string VoteSmartCandidateBirthPlace { get; set; }
        public string VoteSmartCandidatePronunciation { get; set; }
        public string VoteSmartCandidateGender { get; set; }
        public IEnumerable<string> VoteSmartCandidateFamily { get; set; }
        public string VoteSmartCandidateHomeCity { get; set; }
        public string VoteSmartCandidateHomeState { get; set; }
        public IEnumerable<string> VoteSmartCandidateEducation { get; set; }
        public IEnumerable<string> VoteSmartCandidateProfession { get; set; }
        public IEnumerable<string> VoteSmartCandidatePolitical { get; set; }
        public string VoteSmartCandidateReligion { get; set; }
        public IEnumerable<string> VoteSmartCandidateCongMembership { get; set; }
        public IEnumerable<string> VoteSmartCandidateOrgMembership { get; set; }
        public IEnumerable<string> VoteSmartCandidateSpecialMsg { get; set; }

        public int RunningMateId { get; set; }
        public string VoteSmartRunningMateId { get; set; }
        public string OpenSecretsRunningMateId { get; set; }
        public string VoteSmartRunningMatePhotoUrl { get; set; }
        public string RunningMateFirstName { get; set; }
        public string RunningMateMiddleName { get; set; }
        public string RunningMateLastName { get; set; }
        public string RunningMateSuffix { get; set; }
        public string RunningMateName
        {
            get
            {
                return string.Format("{0} {1}", RunningMateFirstName, RunningMateLastName);
            }
        }

        public int RunningMateOfficeId { get; set; }
        public string VoteSmartRunningMateOfficeId { get; set; }
        public string RunningMateOfficeName { get; set; }
        public string RunningMateOfficeTerm { get; set; }

        public string VoteSmartRunningMateNickName { get; set; }
        public string VoteSmartRunningMateMiddleName { get; set; }
        public string VoteSmartRunningMatePreferredName { get; set; }
        public string VoteSmartRunningMateBirthDate { get; set; }
        public string VoteSmartRunningMateBirthPlace { get; set; }
        public string VoteSmartRunningMatePronunciation { get; set; }
        public string VoteSmartRunningMateGender { get; set; }
        public IEnumerable<string> VoteSmartRunningMateFamily { get; set; }
        public string VoteSmartRunningMateHomeCity { get; set; }
        public string VoteSmartRunningMateHomeState { get; set; }
        public IEnumerable<string> VoteSmartRunningMateEducation { get; set; }
        public IEnumerable<string> VoteSmartRunningMateProfession { get; set; }
        public IEnumerable<string> VoteSmartRunningMatePolitical { get; set; }
        public string VoteSmartRunningMateReligion { get; set; }
        public IEnumerable<string> VoteSmartRunningMateCongMembership { get; set; }
        public IEnumerable<string> VoteSmartRunningMateOrgMembership { get; set; }
        public IEnumerable<string> VoteSmartRunningMateSpecialMsg { get; set; }

    }




}