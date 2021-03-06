﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OhioVoter.ViewModels.VoteSmart
{
    [XmlRoot(ElementName = "candidateList")]
    public class CandidateList
    {
        [XmlElement(ElementName = "generalInfo")]
        public GeneralInfoCandidate GeneralInfo { get; set; }
        [XmlElement(ElementName = "candidate")]
        public List<Candidate> Candidate { get; set; }
    }

    [XmlRoot(ElementName = "generalInfo")]
    public class GeneralInfoCandidate
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "linkBack")]
        public string LinkBack { get; set; }
    }

    [XmlRoot(ElementName="candidate")]
    public class Candidate
    {
		[XmlElement(ElementName="candidateId")]
		public string ElectionCandidateId { get; set; }
		[XmlElement(ElementName="firstName")]
		public string FirstName { get; set; }
		[XmlElement(ElementName="nickName")]
		public string NickName { get; set; }
		[XmlElement(ElementName="middleName")]
		public string MiddleName { get; set; }
		[XmlElement(ElementName="preferredName")]
		public string PreferredName { get; set; }
		[XmlElement(ElementName="lastName")]
		public string LastName { get; set; }
		[XmlElement(ElementName="suffix")]
		public string Suffix { get; set; }
		[XmlElement(ElementName="title")]
		public string Title { get; set; }
		[XmlElement(ElementName="ballotName")]
		public string BallotName { get; set; }
		[XmlElement(ElementName="electionParties")]
		public string ElectionParties { get; set; }
		[XmlElement(ElementName="electionStatus")]
		public string ElectionStatus { get; set; }
		[XmlElement(ElementName="electionStage")]
		public string ElectionStage { get; set; }
		[XmlElement(ElementName="electionDistrictId")]
		public string ElectionDistrictId { get; set; }
		[XmlElement(ElementName="electionDistrictName")]
		public string ElectionDistrictName { get; set; }
		[XmlElement(ElementName="electionOffice")]
		public string ElectionOffice { get; set; }
		[XmlElement(ElementName="electionOfficeId")]
		public string ElectionOfficeId { get; set; }
		[XmlElement(ElementName="electionStateId")]
		public string ElectionStateId { get; set; }
		[XmlElement(ElementName="electionOfficeTypeId")]
		public string ElectionOfficeTypeId { get; set; }
		[XmlElement(ElementName="electionYear")]
		public string ElectionYear { get; set; }
		[XmlElement(ElementName="electionSpecial")]
		public string ElectionSpecial { get; set; }
		[XmlElement(ElementName="electionDate")]
		public string ElectionDate { get; set; }
		[XmlElement(ElementName="officeParties")]
		public string OfficeParties { get; set; }
		[XmlElement(ElementName="officeStatus")]
		public string OfficeStatus { get; set; }
		[XmlElement(ElementName="officeDistrictId")]
		public string OfficeDistrictId { get; set; }
		[XmlElement(ElementName="officeDistrictName")]
		public string OfficeDistrictName { get; set; }
		[XmlElement(ElementName="officeStateId")]
		public string OfficeStateId { get; set; }
		[XmlElement(ElementName="officeId")]
		public string OfficeId { get; set; }
		[XmlElement(ElementName="officeName")]
		public string OfficeName { get; set; }
		[XmlElement(ElementName="officeTypeId")]
		public string OfficeTypeId { get; set; }
		[XmlElement(ElementName="runningMateId")]
		public string RunningMateId { get; set; }
		[XmlElement(ElementName="runningMateName")]
		public string RunningMateName { get; set; }
	}

}