using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OhioVoter.ViewModels.VoteSmart
{
    [XmlRoot(ElementName = "bio")]
    public class Bio
    {
        [XmlElement(ElementName = "generalInfo")]
        public GeneralInfoCandidateBio GeneralInfo { get; set; }
        [XmlElement(ElementName = "candidate")]
        public CandidateBio Candidate { get; set; }
    }

    [XmlRoot(ElementName = "generalInfo")]
    public class GeneralInfoCandidateBio
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "linkBack")]
        public string LinkBack { get; set; }
    }

    [XmlRoot(ElementName = "candidate")]
    public class CandidateBio
    {
        [XmlElement(ElementName = "candidateId")]
        public string CandidateId { get; set; }
        [XmlElement(ElementName = "crpId")]
        public string CrpId { get; set; }
        [XmlElement(ElementName = "photo")]
        public string Photo { get; set; }
        [XmlElement(ElementName = "firstName")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "nickName")]
        public string NickName { get; set; }
        [XmlElement(ElementName = "middleName")]
        public string MiddleName { get; set; }
        [XmlElement(ElementName = "preferredName")]
        public string PreferredName { get; set; }
        [XmlElement(ElementName = "lastName")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "suffix")]
        public string Suffix { get; set; }
        [XmlElement(ElementName = "birthDate")]
        public string BirthDate { get; set; }
        [XmlElement(ElementName = "birthPlace")]
        public string BirthPlace { get; set; }
        [XmlElement(ElementName = "pronunciation")]
        public string Pronunciation { get; set; }
        [XmlElement(ElementName = "gender")]
        public string Gender { get; set; }
        [XmlElement(ElementName = "family")]
        public string Family { get; set; }
        [XmlElement(ElementName = "homeCity")]
        public string HomeCity { get; set; }
        [XmlElement(ElementName = "homeState")]
        public string HomeState { get; set; }
        [XmlElement(ElementName = "education")]
        public string Education { get; set; }
        [XmlElement(ElementName = "profession")]
        public string Profession { get; set; }
        [XmlElement(ElementName = "political")]
        public string Political { get; set; }
        [XmlElement(ElementName = "religion")]
        public string Religion { get; set; }
        [XmlElement(ElementName = "congMembership")]
        public string CongMembership { get; set; }
        [XmlElement(ElementName = "orgMembership")]
        public string OrgMembership { get; set; }
        [XmlElement(ElementName = "specialMsg")]
        public string SpecialMsg { get; set; }
    }

}