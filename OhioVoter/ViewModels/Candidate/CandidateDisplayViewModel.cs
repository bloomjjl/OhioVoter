using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateDisplayViewModel
    {
        public string ControllerName { get; set; }

        public int CandidateLookUpId { get; set; }

        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }

        public string CertifiedCandidateId { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public string OfficeHolderId { get; set; }
        public string OfficeHolderName { get; set; }

        public Candidate Candidate { get; set; }
        public RunningMate RunningMate { get; set; }

        public CandidateSummaryViewModel CandidateSummaryViewModel { get; set; }
        public PoliticalViewModel PoliticalViewModel { get; set; }
        public CaucusViewModel CaucusViewModel { get; set; }
        public ProfessionalViewModel ProfessionalViewModel { get; set; }
        public EducationViewModel EducationViewModel { get; set; }
        public PersonalViewModel PersonalViewModel { get; set; }
        public CivicViewModel CivicViewModel { get; set; }
        public AdditionalViewModel AdditionalViewModel { get; set; }
    }


    public class Candidate
    {
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string OfficeTerm { get; set; }
        public string VoteSmartOfficeId { get; set; }

        public int CandidateId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Name
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string VoteSmartCandidateId { get; set; }
        public string OpenSecretsCandidateId { get; set; }
        public string VoteSmartCandidatePhotoUrl { get; set; }
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
    }



    public class RunningMate
    {
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string OfficeTerm { get; set; }
        public string VoteSmartOfficeId { get; set; }

        public int CandidateId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Name
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string VoteSmartCandidateId { get; set; }
        public string OpenSecretsCandidateId { get; set; }
        public string VoteSmartCandidatePhotoUrl { get; set; }
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

    }

}