using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OhioVoter.ViewModels.VoteSmart
{
    [XmlRoot(ElementName = "elections")]
    public class Elections
    {
        [XmlElement(ElementName = "generalInfo")]
        public GeneralInfo GeneralInfo { get; set; }
        [XmlElement(ElementName = "zipMessage")]
        public string ZipMessage { get; set; }
        [XmlElement(ElementName = "election")]
        public List<Election> Election { get; set; }
    }



    [XmlRoot(ElementName = "election")]
    public class Election
    {
        [XmlElement(ElementName = "electionId")]
        public string ElectionId { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "stateId")]
        public string StateId { get; set; }
        [XmlElement(ElementName = "officeTypeId")]
        public string OfficeTypeId { get; set; }
        [XmlElement(ElementName = "special")]
        public string Special { get; set; }
        [XmlElement(ElementName = "electionYear")]
        public string ElectionYear { get; set; }
    }

}