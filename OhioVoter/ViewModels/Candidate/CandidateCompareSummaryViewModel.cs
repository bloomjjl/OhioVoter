using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateCompareSummaryViewModel
    {
        public string ControllerName { get; set; }

        public int VotingDateId { get; set; }
        public string VotingDate { get; set; }

        public string CertifiedCandidateId { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public string OfficeHolderId { get; set; }
        public string OfficeHolderName { get; set; }

        public int CandidateFirstDisplayId { get; set; }
        public bool CandidateFirstIsRunningMate { get; set; }
        public int CandidateSecondDisplayId { get; set; }
        public bool CandidateSecondIsRunningMate { get; set; }

        public CandidateCompareSummaryFirstViewModel CandidateCompareSummaryFirstViewModel { get; set; }
        public CandidateCompareSummarySecondViewModel CandidateCompareSummarySecondViewModel { get; set; }
        public CandidateCompareSummaryLookUpViewModel CandidateCompareSummaryLookUpViewModel { get; set; }

    }


    public class CandidateCompareFirst
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



    public class CandidateCompareSecond
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