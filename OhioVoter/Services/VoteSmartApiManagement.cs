using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace OhioVoter.Services
{
    public class VoteSmartApiManagement
    {
        private static string _votesmartApiKey = "1c5073e4c73532c7d2daf51651023d8f";



        public String GetVoteSmartCandidateInformation()
        {
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/

                string stateId = "OH";
                string localId = "3292";
                string hamiltonCountyLocalId = "3292";
                string cincinnatiCityLocalId = "930";
                string officeTypeId = "P";
                string levelId = "F";
                string officeLevelId = levelId;
                string branchId = "B";
                string officeId = "1";
                string lastName = "Obama";
                string districtId = "30922";
                string electionDistrictId = "25657"; // district 1
                string officeDistrictId = "30643"; // office district 1
                string electionYear = "";
                string stageId = "";
                string candidateId = "27009"; // David Mann


                // LOCAL

                string urlRequest = GetCountiesForSuppliedState(stateId);
                WebClient client = new WebClient();
                string json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetCitiesForSuppliedState(stateId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetCandidatesForSpecifiedLocation(hamiltonCountyLocalId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetCandidatesForSpecifiedLocation(cincinnatiCityLocalId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // OFFICES

                urlRequest = GetAllOfficeTypes();
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetAllBranchesOfGovernment();
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetAllLevelsOfGovernment();
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetAllOfficesForSpecifiedType(officeTypeId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetAllOfficesForSpecifiedLevel(levelId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetAllOfficesForSpecifiedTypeAndLevel(officeTypeId, officeLevelId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // ERROR??
                urlRequest = GetAllOfficesForSpecifiedBranchAndLevel(branchId, levelId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // OFFICIALS

                urlRequest = GetAllOfficialsForSpecifiedState(stateId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // ERROR??
                urlRequest = GetAllOfficialsForSpecifiedOfficeAndState(officeId, stateId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // ERROR??
                urlRequest = GetAllOfficialsForSpecifiedOfficeTypeAndState(officeTypeId, stateId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetAllOfficialsWithMatchingLastName(lastName);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // similar last name
                // used by VoteSmart to look up candidates
                urlRequest = GetAllOfficialsWithLastName(lastName);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                districtId = "31565"; // officeDistrictId
                urlRequest = GetAllOfficialsByDistrict(districtId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // STATES

                urlRequest = GetAllStates();
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                urlRequest = GetInformationForSpecifiedState(stateId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // ADDRESS

                // ERROR??
                urlRequest = GetCampaignOfficeInformationForSpecifiedCandidate(candidateId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                // CANDIDATEBIO

                urlRequest = GetBiographyInformationForSpecifiedCandidate(candidateId);
                client = new WebClient();
                json = client.DownloadString(urlRequest);
                //RootObject votesmartResult = JToken.Parse(json).ToObject<RootObject>();

                return "";
            }
            catch
            {// return empty object if bad address supplied
                return "";
            }
        }


        /// <summary>
        ///     VoteSmart API Class: Local
        ///     Fetches counties in a state
        ///     set default state to Ohio
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>
        ///     list of counties
        ///     (localId,name,url)
        /// </returns>
        public string GetCountiesForSuppliedState(string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Local.getCounties?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "stateId=" + stateId;
        }



        /// <summary>
        ///     VoteSmart API Class: Local
        ///     Fetches cities in a state
        ///     set default state to Ohio
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>
        ///     List of cities
        ///     (localId,name,url)
        /// </returns>
        public string GetCitiesForSuppliedState(string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Local.getCities?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "stateId=" + stateId;
        }



        /// <summary>
        ///     VoteSmart API Class: Local
        ///     Fetches officials for a locality
        /// </summary>
        /// <param name="localId"></param>
        /// <returns>
        ///     List of candidates with their name,location,office,party
        /// </returns>
        public string GetCandidatesForSpecifiedLocation(string localId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Local.getOfficials?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "localId=" + localId;
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps all office types votesmart keeps track of
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of office types
        /// </returns>
        public string GetAllOfficeTypes()
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getTypes?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key;
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps the branches of government and their IDs
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of branches of government
        /// </returns>
        public string GetAllBranchesOfGovernment()
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getBranches?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key;
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps the levels of government and their IDs
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of levels of government
        /// </returns>
        public string GetAllLevelsOfGovernment()
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getLevels?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key;
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps offices we keep track of according to type.
        /// </summary>
        /// <param name="officeTypeId"></param>
        /// <returns>
        ///     List of offices
        /// </returns>
        public string GetAllOfficesForSpecifiedType(string officeTypeId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByType?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "officeTypeId=" + officeTypeId;
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps offices votesmart keeps track of according to level.
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns>
        ///     List of offices
        /// </returns>
        public string GetAllOfficesForSpecifiedLevel(string levelId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByLevel?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "levelId=" + levelId;
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps offices votesmart keeps track of according to type and level.
        /// </summary>
        /// <param name="officeTypeId"></param>
        /// <param name="officeLevelId"></param>
        /// <returns>
        ///     List of offices
        /// </returns>
        public string GetAllOfficesForSpecifiedTypeAndLevel(string officeTypeId, string officeLevelId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByTypeLevel?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "officeTypeId=" + officeTypeId +
                   andChar + "officeLevelId=" + officeLevelId;
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps offices votesmart keeps track of according to branch and level.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="levelId"></param>
        /// <returns>
        ///     List of offices
        /// </returns>        
        /// 
        /// <errorMessage>Unknown error.</errorMessage>
        public string GetAllOfficesForSpecifiedBranchAndLevel(string branchId, string levelId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByBranchLevel?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "branchId=" + branchId +
                   andChar + "levelId=" + levelId;
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to state representation
        ///     set default to Ohio
        /// </summary>
        /// <param name="stateId">(default: 'NA')</param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetAllOfficialsForSpecifiedState(string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getStatewide?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "stateId=" + stateId;
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to office and state representation.
        ///     set default to Ohio
        /// </summary>
        /// <param name="officeId"></param>
        /// <param name="stateId">(default: 'NA')</param>
        /// <returns>
        ///     List of officials
        /// </returns>
        /// 
        /// <errorMessage>No officials found matching this criteria.</errorMessage>
        public string GetAllOfficialsForSpecifiedOfficeAndState(string officeId, string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByOfficeState?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "officeId=" + officeId +
                   andChar + "stateId=" + stateId;
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to office type and state representation.
        ///     set default to Ohio
        /// </summary>
        /// <param name="officeTypeId"></param>
        /// <param name="stateId">(default: 'NA')</param>
        /// <returns>
        ///     List of officials
        /// </returns>
        /// 
        /// <errorMessage>No candidates found matching this criteria.</errorMessage>
        public string GetAllOfficialsForSpecifiedOfficeTypeAndState(string officeTypeId, string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByOfficeTypeState?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "officeTypeId=" + officeTypeId +
                   andChar + "stateId=" + stateId;
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to a lastName match.
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetAllOfficialsWithMatchingLastName(string lastName)
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByLastname?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "lastName=" + lastName;
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to a fuzzy lastName match.
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetAllOfficialsWithLastName(string lastName)
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByLevenshtein?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "lastName=" + lastName;
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to the district they are running for.
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetAllOfficialsByDistrict(string districtId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByDistrict?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "districtId=" + districtId;
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to the zip code they represent.
        /// </summary>
        /// <param name="zip5"></param>
        /// <param name="zip4">(default: NULL)</param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetAllOfficialsByDistrict(string zip5, string zip4)
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByZip?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "zip5=" + zip5 +
                   andChar + "zip4=" + zip4;
        }



        /// <summary>
        ///     VoteSmart API Class: State
        ///     This method grabs a simple state ID and name list for mapping ID to state names.
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of states
        /// </returns>
        public string GetAllStates()
        {
            string api = "http://api.votesmart.org/";
            string path = "State.getStateIDs?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key;
        }



        /// <summary>
        ///     VoteSmart API Class: State
        ///     This method grabs a various data votesmart keeps on a state
        ///     set default state to Ohio
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>
        ///     state object
        /// </returns>
        public string GetInformationForSpecifiedState(string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "State.getState?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "stateId=" + stateId;
        }



        /// <summary>
        ///     VoteSmart API Class: Address
        ///     This method grabs campaign office(s) and basic candidate information for the specified candidate.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>
        ///     List of campaign office information
        /// </returns>
        /// 
        /// <errorMessage>Campaign address no longer available or candidate does not exist.</errorMessage>
        public string GetCampaignOfficeInformationForSpecifiedCandidate(string candidateId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Address.getCampaign?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "candidateId=" + candidateId;
        }



        /// <summary>
        ///     VoteSmart API Class: CandidateBio
        ///     This method grabs the main bio for each candidate.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>
        ///     Candidate object campaign office information
        /// </returns>
        public string GetBiographyInformationForSpecifiedCandidate(string candidateId)
        {
            string api = "http://api.votesmart.org/";
            string path = "CandidateBio.getBio?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return api + path +
                   andChar + "key=" + key +
                   andChar + "candidateId=" + candidateId;
        }









    }
}