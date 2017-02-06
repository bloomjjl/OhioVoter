using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OhioVoter.ViewModels.VoteSmart
{
    [XmlRoot(ElementName = "generalInfo")]
    public class GeneralInfo
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "linkBack")]
        public string LinkBack { get; set; }
    }
}