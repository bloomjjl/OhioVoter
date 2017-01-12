using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Google
{
    public class Election
    {
        public string id { get; set; }
        public string name { get; set; }
        public string electionDay { get; set; }
        public string ocdDivisionId { get; set; }
    }

    public class NormalizedInput
    {
        public string line1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class Address
    {
        public string locationName { get; set; }
        public string line1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class Source
    {
        public string name { get; set; }
        public bool official { get; set; }
    }

    public class PollingLocation
    {
        public Address address { get; set; }
        public string notes { get; set; }
        public string pollingHours { get; set; }
        public List<Source> sources { get; set; }
    }

    public class District
    {
        public string name { get; set; }
        public string scope { get; set; }
        public string id { get; set; }
        public string kgForeignKey { get; set; }
    }

    public class Channel
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Candidate
    {
        public string name { get; set; }
        public string party { get; set; }
        public string candidateUrl { get; set; }
        public List<Channel> channels { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }

    public class Source2
    {
        public string name { get; set; }
        public bool official { get; set; }
    }

    public class Contest
    {
        public string type { get; set; }
        public string office { get; set; }
        public List<string> level { get; set; }
        public List<string> roles { get; set; }
        public District district { get; set; }
        public List<Candidate> candidates { get; set; }
        public List<Source2> sources { get; set; }
    }

    public class CorrespondenceAddress
    {
        public string line1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class ElectionAdministrationBody
    {
        public string name { get; set; }
        public string electionInfoUrl { get; set; }
        public CorrespondenceAddress correspondenceAddress { get; set; }
    }

    public class PhysicalAddress
    {
        public string line1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class ElectionOfficial
    {
        public string officePhoneNumber { get; set; }
        public string emailAddress { get; set; }
    }

    public class ElectionAdministrationBody2
    {
        public string electionInfoUrl { get; set; }
        public PhysicalAddress physicalAddress { get; set; }
        public List<ElectionOfficial> electionOfficials { get; set; }
    }

    public class Source3
    {
        public string name { get; set; }
        public bool official { get; set; }
    }

    public class LocalJurisdiction
    {
        public string name { get; set; }
        public ElectionAdministrationBody2 electionAdministrationBody { get; set; }
        public List<Source3> sources { get; set; }
    }

    public class Source4
    {
        public string name { get; set; }
        public bool official { get; set; }
    }

    public class State
    {
        public string name { get; set; }
        public ElectionAdministrationBody electionAdministrationBody { get; set; }
        public LocalJurisdiction local_jurisdiction { get; set; }
        public List<Source4> sources { get; set; }
    }

    public class RootObject
    {
        public string kind { get; set; }
        public Election election { get; set; }
        public NormalizedInput normalizedInput { get; set; }
        public List<PollingLocation> pollingLocations { get; set; }
        public List<Contest> contests { get; set; }
        public List<State> state { get; set; }
    }

}

