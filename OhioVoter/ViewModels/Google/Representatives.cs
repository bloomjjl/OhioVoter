using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OhioVoter.ViewModels.Google
{
    /*
    public class RootObject
    {
        public string kind { get; set; }
        public NormalizedInput normalizedInput { get; set; }
        public Divisions divisions { get; set; }
        public List<Office> offices { get; set; }
        public List<Official> officials { get; set; }
    }

    [XmlRoot(ElementName = "NormalizedInput")]
    public class RepresentativesNormalizedInput
    {
        public string line1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class Divisions
    {
        public OcdDivisionCountryUs __invalid_name__ocd-division/country:us { get; set; }
        public OcdDivisionCountryUsStateOh __invalid_name__ocd-division/country:us/state:oh { get; set; }
        public OcdDivisionCountryUsStateOhCd1 __invalid_name__ocd-division/country:us/state:oh/cd:1 { get; set; }
        public OcdDivisionCountryUsStateOhCountyHamilton __invalid_name__ocd-division/country:us/state:oh/county:hamilton { get; set; }
        public OcdDivisionCountryUsStateOhCountyHamiltonSchoolDistrictFinneytownLocal __invalid_name__ocd-division/country:us/state:oh/county:hamilton/school_district:finneytown_local { get; set; }
        public OcdDivisionCountryUsStateOhPlaceCincinnati __invalid_name__ocd-division/country:us/state:oh/place:cincinnati { get; set; }
        public OcdDivisionCountryUsStateOhSldl32 __invalid_name__ocd-division/country:us/state:oh/sldl:32 { get; set; }
        public OcdDivisionCountryUsStateOhSldu9 __invalid_name__ocd-division/country:us/state:oh/sldu:9 { get; set; }
    }

    public class OcdDivisionCountryUs
    {
        public string name { get; set; }
        public List<int> officeIndices { get; set; }
    }

    public class OcdDivisionCountryUsStateOh
    {
        public string name { get; set; }
        public List<int> officeIndices { get; set; }
    }

    public class OcdDivisionCountryUsStateOhCd1
    {
        public string name { get; set; }
        public List<int> officeIndices { get; set; }
    }

    public class OcdDivisionCountryUsStateOhCountyHamilton
    {
        public string name { get; set; }
        public List<int> officeIndices { get; set; }
    }

    public class OcdDivisionCountryUsStateOhCountyHamiltonSchoolDistrictFinneytownLocal
    {
        public string name { get; set; }
    }

    public class OcdDivisionCountryUsStateOhPlaceCincinnati
    {
        public string name { get; set; }
        public List<int> officeIndices { get; set; }
    }

    public class OcdDivisionCountryUsStateOhSldl32
    {
        public string name { get; set; }
        public List<int> officeIndices { get; set; }
    }

    public class OcdDivisionCountryUsStateOhSldu9
    {
        public string name { get; set; }
        public List<int> officeIndices { get; set; }
    }


    public class Office
    {
        public string name { get; set; }
        public string divisionId { get; set; }
        public List<string> levels { get; set; }
        public List<string> roles { get; set; }
        public List<int> officialIndices { get; set; }
    }

public class Address
{
    public string line1 { get; set; }
    public string line2 { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zip { get; set; }
    public string line3 { get; set; }
}

public class Channel
{
    public string type { get; set; }
    public string id { get; set; }
}

public class Official
{
    public string name { get; set; }
    public List<Address> address { get; set; }
    public string party { get; set; }
    public List<string> phones { get; set; }
    public List<string> urls { get; set; }
    public string photoUrl { get; set; }
    public List<Channel> channels { get; set; }
    public List<string> emails { get; set; }
}

    */
}