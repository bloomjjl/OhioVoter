using Newtonsoft.Json.Linq;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.VoteSmart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Xml;

namespace OhioVoter.Services
{
    public class VoteSmartApiManagement
    {
        private static string _votesmartApiKey = "1c5073e4c73532c7d2daf51651023d8f";



        /// <summary>
        /// get list of candidates that are an exact match to the supplied candidate name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public List<Candidate> GetVoteSmartMatchingCandidateListFromSuppliedLastName(string suppliedLastName)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForAllOfficialsWithMatchingLastName(suppliedLastName);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                List<Candidate> candidates = ProcessResponseForCandidateList(xmlDoc);

                return candidates;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return new List<Candidate>();
            }
        }



        /// <summary>
        /// get list of candidates that are similar to the supplied candidate name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public List<Candidate> GetVoteSmartSimilarCandidateListFromSuppliedLastName(string suppliedLastName)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForAllOfficialsWithSimilarLastName(suppliedLastName);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                List<Candidate> candidates = ProcessResponseForCandidateList(xmlDoc);

                return candidates;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return new List<Candidate>();
            }
        }




        public XmlDocument MakeRequest(string requestUrl)
        {// Tests Generated
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(requestUrl);
                return (xmlDoc);
            }
            catch (Exception e)
            {
                return null;
            }
        }



        public List<Candidate> ProcessResponseForCandidateList(XmlDocument xmlDoc)
        {// Tests Generated
            if (xmlDoc == null)
                return new List<Candidate>();

            XmlNodeList candidates = xmlDoc.GetElementsByTagName("candidate");

            List<Candidate> candidatelist = new List<Candidate>();

            for (int i = 0; i < candidates.Count; i++)
            {
                candidatelist.Add(new Candidate()
                {
                    CandidateId = candidates[i].SelectSingleNode("candidateId").InnerText,
                    FirstName = candidates[i].SelectSingleNode("firstName").InnerText,
                    LastName = candidates[i].SelectSingleNode("lastName").InnerText
                });
            };

            return candidatelist;
        }


        /*
        private List<Candidate> GetListOfCandidates(XmlDocument urlResponse)
        {
            List<Candidate> candidates = new List<Candidate>();
            string xml = urlResponse.InnerXml.ToString();

            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            { 
                // LinePosition < 
                foreach (var candidate in xml)
                {
                    Candidate c = new Candidate();

                    reader.ReadToFollowing("candidate");

                    // int line = reader.LineNumber();
                    // int postion = reader.LinePosition();
                    
                    // Create another reader that contains just the current candidate node.
                    XmlReader inner = reader.ReadSubtree();
                    
                    inner.ReadToDescendant("candidateId");
                    c.CandidateId = reader.ReadElementContentAsString();
                    
                    inner.ReadToDescendant("firstName");
                    c.FirstName = reader.ReadElementContentAsString();

                    inner.ReadToDescendant("lastName");
                    c.LastName = reader.ReadElementContentAsString();

                    // call Close on the inner reader and
                    // continue processing using the original reader.
                    inner.Close();

                    candidates.Add(c);
                };
            }          
              
            return candidates;
        }
        */


        /*
        private Candidate GetCandidateInformation(XmlReader reader)
        {
            Candidate candidate = new Candidate();

            try
            {
                reader.ReadToFollowing("candidate");

                reader.ReadStartElement("candidateId");
                candidate.CandidateId = reader.ReadElementContentAsString();

                reader.ReadStartElement("firstName");
                candidate.FirstName = reader.ReadElementContentAsString();

                reader.ReadStartElement("lastName");
                candidate.LastName = reader.ReadElementContentAsString();

                return candidate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();

                return candidate;
            }
        }
        */



        // ***************************************************************
        // ***************************************************************
        // ***************************************************************

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
        public string GetUrlRequestForCountiesForSuppliedState(string stateId = "OH")
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Local.getCounties?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "stateId=", stateId);
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
        public string GetUrlRequestForCitiesForSuppliedState(string stateId = "OH")
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Local.getCities?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "stateId=", stateId);
        }



        /// <summary>
        ///     VoteSmart API Class: Local
        ///     Fetches officials for a locality
        /// </summary>
        /// <param name="localId"></param>
        /// <returns>
        ///     List of candidates with their name,location,office,party
        /// </returns>
        public string GetUrlRequestForCandidatesForSpecifiedLocation(string localId)
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Local.getOfficials?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "localId=", localId);
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps all office types votesmart keeps track of
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of office types
        /// </returns>
        public string GetUrlRequestForAllOfficeTypes()
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Office.getTypes?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key);
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps the branches of government and their IDs
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of branches of government
        /// </returns>
        public string GetUrlRequestForAllBranchesOfGovernment()
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Office.getBranches?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key);
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps the levels of government and their IDs
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of levels of government
        /// </returns>
        public string GetUrlRequestForAllLevelsOfGovernment()
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Office.getLevels?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key);
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps offices we keep track of according to type.
        /// </summary>
        /// <param name="officeTypeId"></param>
        /// <returns>
        ///     List of offices
        /// </returns>
        public string GetUrlRequestForAllOfficesForSpecifiedType(string officeTypeId)
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByType?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "officeTypeId=", officeTypeId);
        }



        /// <summary>
        ///     VoteSmart API Class: Office
        ///     This method dumps offices votesmart keeps track of according to level.
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns>
        ///     List of offices
        /// </returns>
        public string GetUrlRequestForAllOfficesForSpecifiedLevel(string levelId)
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByLevel?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "levelId=", levelId);
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
        public string GetUrlRequestForAllOfficesForSpecifiedTypeAndLevel(string officeTypeId, string officeLevelId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByTypeLevel?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "officeTypeId=", officeTypeId,
                                 andChar, "officeLevelId=", officeLevelId);
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
        public string GetUrlRequestForAllOfficesForSpecifiedBranchAndLevel(string branchId, string levelId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Office.getOfficesByBranchLevel?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "branchId=", branchId,
                                 andChar, "levelId=", levelId);
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
        public string GetUrlRequestForAllOfficialsForSpecifiedState(string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getStatewide?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "stateId=", stateId);
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
        public string GetUrlRequestForAllOfficialsForSpecifiedOfficeAndState(string officeId, string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByOfficeState?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "officeId=", officeId,
                                 andChar, "stateId=", stateId);
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
        public string GetUrlRequestForAllOfficialsForSpecifiedOfficeTypeAndState(string officeTypeId, string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByOfficeTypeState?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "officeTypeId=", officeTypeId,
                                 andChar, "stateId=", stateId);
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to a lastName match.
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetUrlRequestForAllOfficialsWithMatchingLastName(string lastName)
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByLastname?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "lastName=", lastName);
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to a fuzzy lastName match.
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetUrlRequestForAllOfficialsWithSimilarLastName(string lastName)
        {// Tests Generated
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByLevenshtein?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "lastName=", lastName);
        }



        /// <summary>
        ///     VoteSmart API Class: Officials
        ///     This method grabs a list of officials according to the district they are running for.
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns>
        ///     List of officials
        /// </returns>
        public string GetUrlRequestForAllOfficialsByDistrict(string districtId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByDistrict?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "districtId=", districtId);
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
        public string GetUrlRequestForAllOfficialsByDistrict(string zip5, string zip4)
        {
            string api = "http://api.votesmart.org/";
            string path = "Officials.getByZip?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "zip5=", zip5,
                                 andChar, "zip4=", zip4);
        }



        /// <summary>
        ///     VoteSmart API Class: State
        ///     This method grabs a simple state ID and name list for mapping ID to state names.
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        ///     List of states
        /// </returns>
        public string GetUrlRequestForAllStates()
        {
            string api = "http://api.votesmart.org/";
            string path = "State.getStateIDs?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key);
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
        public string GetUrlRequestForInformationForSpecifiedState(string stateId = "OH")
        {
            string api = "http://api.votesmart.org/";
            string path = "State.getState?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "stateId=", stateId);
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
        public string GetUrlRequestForCampaignOfficeInformationForSpecifiedCandidate(string candidateId)
        {
            string api = "http://api.votesmart.org/";
            string path = "Address.getCampaign?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "candidateId=", candidateId);
        }



        /// <summary>
        ///     VoteSmart API Class: CandidateBio
        ///     This method grabs the main bio for each candidate.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>
        ///     Candidate object campaign office information
        /// </returns>
        public string GetUrlRequestForBiographyInformationForSpecifiedCandidate(string candidateId)
        {
            string api = "http://api.votesmart.org/";
            string path = "CandidateBio.getBio?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "candidateId=", candidateId);
        }









    }
}