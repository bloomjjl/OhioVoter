﻿using Newtonsoft.Json.Linq;
using OhioVoter.Models;
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
        private string _votesmartApiKey;
        private static string _votesmartApi = "http://api.votesmart.org/";



        public VoteSmartApiManagement()
        {
            _votesmartApiKey = GetApiKeyFromDatabase();
        }



        private string GetApiKeyFromDatabase()
        { 
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                Api dtoApi = context.Apis.FirstOrDefault(x => x.ApiUrl == _votesmartApi);

                if (dtoApi == null)
                {
                    return string.Empty;
                }
                else
                {
                    return dtoApi.ApiKey;
                }
            }
        }



        public ViewModels.VoteSmart.Elections GetVoteSmartElectionInformationFromSuppliedVoteSmartElectionId(string electionId)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForElectionInformationForElectionId(electionId);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                ViewModels.VoteSmart.Elections election = ProcessResponseForElectionList(xmlDoc);

                return election;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return new ViewModels.VoteSmart.Elections();
            }
        }





        /// <summary>
        /// get list of candidates that are an exact match to the supplied candidate name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public ViewModels.VoteSmart.CandidateBio GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(string voteSmartCandidateId)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForBiographyInformationForSpecifiedCandidate(voteSmartCandidateId);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                ViewModels.VoteSmart.CandidateBio candidate = ProcessResponseForCandidateBio(xmlDoc);

                return candidate;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return new ViewModels.VoteSmart.CandidateBio();
            }
        }







        public string GetVoteSmartCandidateImageUrlFromSuppliedVoteSmartCandidateId(string voteSmartCandidateId)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForBiographyInformationForSpecifiedCandidate(voteSmartCandidateId);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                ViewModels.VoteSmart.CandidateBio candidate = ProcessResponseForCandidateBio(xmlDoc);

                return candidate.Photo;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return string.Empty;
            }
        }



        /// <summary>
        /// get list of candidates that are an exact match to the supplied candidate name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public List<ViewModels.VoteSmart.Candidate> GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear(string lastName, int year, string stageId)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForCandidateInformationForMatchingLastNameInASpecifiedElectionYear(lastName, year, stageId);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                List<ViewModels.VoteSmart.Candidate> candidates = ProcessResponseForCandidateList(xmlDoc);

                return candidates;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return new List<ViewModels.VoteSmart.Candidate>();
            }
        }






        /// <summary>
        /// get list of candidates that are similar to the supplied candidate name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public List<ViewModels.VoteSmart.Candidate> GetVoteSmartSimilarCandidateListFromSuppliedLastName(string suppliedLastName)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForAllOfficialsWithSimilarLastName(suppliedLastName);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                List<ViewModels.VoteSmart.Candidate> candidates = ProcessResponseForCandidateList(xmlDoc);

                return candidates;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return new List<ViewModels.VoteSmart.Candidate>();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public ViewModels.VoteSmart.Elections GetVoteSmartElectionInformationFromSuppliedZipCodeAndYear(string zipCode, string zipCodeSuffix, int year)
        {
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/
                // http://xmltocsharp.azurewebsites.net/

                string urlRequest = GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear(zipCode, zipCodeSuffix, year);
                XmlDocument xmlDoc = MakeRequest(urlRequest);
                ViewModels.VoteSmart.Elections elections = ProcessResponseForElectionList(xmlDoc);

                return elections;
            }
            catch (Exception e)
            {// return empty object if bad information supplied
                return new ViewModels.VoteSmart.Elections();
            }
        }



        // ***************************************************************
        // ***************************************************************
        // ***************************************************************



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



        public List<ViewModels.VoteSmart.Candidate> ProcessResponseForCandidateList(XmlDocument xmlDoc)
        {// Tests Generated
            if (xmlDoc == null)
                return new List<ViewModels.VoteSmart.Candidate>();

            XmlNodeList candidates = xmlDoc.GetElementsByTagName("candidate");
            List<ViewModels.VoteSmart.Candidate> candidatelist = new List<ViewModels.VoteSmart.Candidate>();

            for (int i = 0; i < candidates.Count; i++)
            {
                candidatelist.Add(new ViewModels.VoteSmart.Candidate()
                {
                    ElectionCandidateId = candidates[i].SelectSingleNode("candidateId").InnerText,
                    FirstName = candidates[i].SelectSingleNode("firstName").InnerText,
                    LastName = candidates[i].SelectSingleNode("lastName").InnerText
                });
            };

            return candidatelist;
        }



        public CandidateBio ProcessResponseForCandidateBio(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return new CandidateBio();

            try
            {
                return new CandidateBio()
                {
                    CandidateId = ValidateDataForElementIsNotNull(xmlDoc, "candidateId"),
                    CrpId = ValidateDataForElementIsNotNull(xmlDoc, "crpId"),
                    Photo = ValidateDataForElementIsNotNull(xmlDoc, "photo"),
                    FirstName = ValidateDataForElementIsNotNull(xmlDoc, "firstName"),
                    NickName = ValidateDataForElementIsNotNull(xmlDoc, "nickName"),
                    MiddleName = ValidateDataForElementIsNotNull(xmlDoc, "middleName"),
                    PreferredName = ValidateDataForElementIsNotNull(xmlDoc, "preferredName"),
                    LastName = ValidateDataForElementIsNotNull(xmlDoc, "lastName"),
                    Suffix = ValidateDataForElementIsNotNull(xmlDoc, "suffix "),
                    BirthDate = ValidateDataForElementIsNotNull(xmlDoc, "birthDate"),
                    BirthPlace = ValidateDataForElementIsNotNull(xmlDoc, "birthPlace"),
                    Pronunciation = ValidateDataForElementIsNotNull(xmlDoc, "pronunciation "),
                    Gender = ValidateDataForElementIsNotNull(xmlDoc, "gender"),
                    Family = ValidateDataForElementIsNotNull(xmlDoc, "family"),
                    HomeCity = ValidateDataForElementIsNotNull(xmlDoc, "homeCity"),
                    HomeState = ValidateDataForElementIsNotNull(xmlDoc, "homeState"),
                    Education = ValidateDataForElementIsNotNull(xmlDoc, "education"),
                    Profession = ValidateDataForElementIsNotNull(xmlDoc, "profession"),
                    Political = ValidateDataForElementIsNotNull(xmlDoc, "political"),
                    Religion = ValidateDataForElementIsNotNull(xmlDoc, "religion"),
                    CongMembership = ValidateDataForElementIsNotNull(xmlDoc, "congMembership"),
                    OrgMembership = ValidateDataForElementIsNotNull(xmlDoc, "orgMembership"),
                    SpecialMsg = ValidateDataForElementIsNotNull(xmlDoc, "specialMsg"),
                };
            }
            catch
            {
                return new CandidateBio();
            }
        }



        public string ValidateDataForElementIsNotNull(XmlDocument xmlDoc, string tagName)
        {
            try
            {
                return xmlDoc.GetElementsByTagName(tagName)[0].ChildNodes[0].InnerText;
            }
            catch
            {
                return "";
            }
        }



        public ViewModels.VoteSmart.Elections ProcessResponseForElectionList(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return new Elections();

            XmlNodeList electionList = xmlDoc.GetElementsByTagName("elections");
            ViewModels.VoteSmart.Elections elections = new ViewModels.VoteSmart.Elections();
            elections.Election = new List<ViewModels.VoteSmart.Election>();

            for (int i = 0; i < electionList[0].ChildNodes.Count; i++)
            {
                
                if (electionList[0].ChildNodes[i].Name == "election")
                {
                    elections.Election.Add(new ViewModels.VoteSmart.Election()
                    {
                        ElectionId = electionList[0].ChildNodes[i].SelectSingleNode("electionId").InnerText,
                        Name = electionList[0].ChildNodes[i].SelectSingleNode("name").InnerText,
                        StateId = electionList[0].ChildNodes[i].SelectSingleNode("stateId").InnerText,
                        OfficeTypeId = electionList[0].ChildNodes[i].SelectSingleNode("officeTypeId").InnerText,
                        Special = electionList[0].ChildNodes[i].SelectSingleNode("special").InnerText,
                        ElectionYear = electionList[0].ChildNodes[i].SelectSingleNode("electionYear").InnerText
                    });
                }
                
            };

            return elections;
        }



        // ***************************************************************
        // ***************************************************************
        // ***************************************************************



        /// <summary>
        ///     VoteSmart API Class: Address
        ///     This method grabs campaign office(s) and basic candidate information for the specified candidate.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>
        ///     <errorMessage>Campaign address no longer available or candidate does not exist.</errorMessage>
        /// </returns>
        public string GetUrlRequestForCampaignOfficeInformationForSpecifiedCandidate(string candidateId)
        {
            string api = _votesmartApi;
            string path = "Address.getCampaign?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "candidateId=", candidateId);
        }



        /// <summary>
        ///     VoteSmart API Class: Candidates
        ///     This method grabs a list of candidates according to a lastname match.
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="year"></param>
        /// <param name="stageId"></param>
        /// <returns>
        ///     candidateList.candidate*.candidateId
        ///     candidateList.candidate*.firstName
        ///     candidateList.candidate*.nickName
        ///     candidateList.candidate*.middleName
        ///     candidateList.candidate*.preferredName
        ///     candidateList.candidate*.lastName
        ///     candidateList.candidate*.suffix
        ///     candidateList.candidate*.title
        ///     candidateList.candidate*.ballotName
        ///     candidateList.candidate*.electionParties
        ///     candidateList.candidate*.electionStatus
        ///     candidateList.candidate*.electionStage
        ///     candidateList.candidate*.electionDistrictId
        ///     candidateList.candidate*.electionDistrictName
        ///     candidateList.candidate*.electionOffice
        ///     candidateList.candidate*.electionOfficeId
        ///     candidateList.candidate*.electionStateId
        ///     candidateList.candidate*.electionOfficeTypeId
        ///     candidateList.candidate*.electionYear
        ///     candidateList.candidate*.electionSpecial
        ///     candidateList.candidate*.electionDate
        ///     candidateList.candidate*.officeParties
        ///     candidateList.candidate*.officeStatus
        ///     candidateList.candidate*.officeDistrictId
        ///     candidateList.candidate*.officeDistrictName
        ///     candidateList.candidate*.officeStateId
        ///     candidateList.candidate*.officeId
        ///     candidateList.candidate*.officeName
        ///     candidateList.candidate*.officeTypeId
        ///     candidateList.candidate*.runningMateId
        ///     candidateList.candidate*.runningMateName
        /// </returns>
        public string GetUrlRequestForCandidateInformationForMatchingLastNameInASpecifiedElectionYear(string lastName, int year, string stageId)
        {
            string api = _votesmartApi;
            string path = "Candidates.getByLastname?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "lastName=", lastName,
                                 andChar, "electionYear=", year,
                                 andChar, "stageId=", stageId);
        }




        /// <summary>
        ///     VoteSmart API Class: CandidateBio
        ///     This method grabs the main bio for each candidate.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>
        ///     bio.candidate.crpId (OpenSecrets ID)
        ///     bio.candidate.firstName
        ///     bio.candidate.nickName
        ///     bio.candidate.middleName
        ///     bio.candidate.lastName
        ///     bio.candidate.suffix
        ///     bio.candidate.birthDate
        ///     bio.candidate.birthPlace
        ///     bio.candidate.pronunciation
        ///     bio.candidate.gender
        ///     bio.candidate.family
        ///     bio.candidate.photo
        ///     bio.candidate.homeCity
        ///     bio.candidate.homeState
        ///     bio.candidate.education**
        ///     bio.candidate.profession**
        ///     bio.candidate.political**
        ///     bio.candidate.religion
        ///     bio.candidate.congMembership**
        ///     bio.candidate.orgMembership**
        ///     bio.candidate.specialMsg
        ///     bio.office.parties
        ///     bio.office.title
        ///     bio.office.shortTitle
        ///     bio.office.name
        ///     bio.office.type
        ///     bio.office.status
        ///     bio.office.firstElect
        ///     bio.office.lastElect
        ///     bio.office.nextElect
        ///     bio.office.termStart
        ///     bio.office.termEnd
        ///     bio.office.district
        ///     bio.office.districtId
        ///     bio.office.stateId
        ///     bio.office.committee*.committeeId
        ///     bio.office.committee*.committeeName
        ///     bio.election*.office
        ///     bio.election*.officeId
        ///     bio.election*.officeType
        ///     bio.election*.parties
        ///     bio.election*.district
        ///     bio.election*.districtId
        ///     bio.election*.status
        ///     bio.election*.ballotName
        /// </returns>
        public string GetUrlRequestForBiographyInformationForSpecifiedCandidate(string candidateId)
        {
            string api = _votesmartApi;
            string path = "CandidateBio.getBio?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "candidateId=", candidateId);
        }



        /// <summary>
        ///     VoteSmart API Class: CandidateBio
        ///     This method expands on getBio() by expanding the education, profession, political, orgMembership, and congMembership elements.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns>
        ///     Includes all elements for getBio(), and expands upon the following:
        ///     bio.candidate.education*.degree
        ///     bio.candidate.education*.field
        ///     bio.candidate.education*.school
        ///     bio.candidate.education*.span
        ///     bio.candidate.education*.gpa
        ///     bio.candidate.education*.fullText
        ///     bio.candidate.profession*.title
        ///     bio.candidate.profession*.organization
        ///     bio.candidate.profession*.span
        ///     bio.candidate.profession*.special
        ///     bio.candidate.profession*.district
        ///     bio.candidate.profession*.fullText
        ///     bio.candidate.political*.title
        ///     bio.candidate.political*.organization
        ///     bio.candidate.political*.span
        ///     bio.candidate.political*.special
        ///     bio.candidate.political*.district
        ///     bio.candidate.political*.fullText
        ///     bio.candidate.congMembership*.title
        ///     bio.candidate.congMembership*.organization
        ///     bio.candidate.congMembership*.span
        ///     bio.candidate.congMembership*.special
        ///     bio.candidate.congMembership*.district
        ///     bio.candidate.congMembership*.fullText
        ///     bio.candidate.orgMembership*.title
        ///     bio.candidate.orgMembership*.organization
        ///     bio.candidate.orgMembership*.span
        ///     bio.candidate.orgMembership*.special
        ///     bio.candidate.orgMembership*.district
        ///     bio.candidate.orgMembership*.fullText
        /// </returns>
        public string GetUrlRequestForDetailedBiographyInformationForSpecifiedCandidate(string candidateId)
        {
            string api = _votesmartApi;
            string path = "CandidateBio.getDetailedBio?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "candidateId=", candidateId);
        }




        /// <summary>
        ///     VoteSmart API Class: District
        ///     This method grabs district IDs according to the zip code.
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="zipCodeSuffix"></param>
        /// <returns>
        ///     districtList.zipMessage
        ///     districtList.district*.districtId
        ///     districtList.district*.name
        ///     districtList.district*.officeId
        ///     districtList.district*.stateId
        /// </returns>
        public string GetUrlRequestForDistrictInformationForSuppliedZipCode(string zipCode, string zipCodeSuffix)
        {// Tests Generated
            string api = _votesmartApi;
            string path = "District.getByZip?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "zip5=", zipCode,
                                 andChar, "zip4=", zipCodeSuffix);
        }



        /// <summary>
        ///     VoteSmart API Class: Election
        ///     This method grabs district basic election data according to electionId
        /// </summary>
        /// <param name="electionId"></param>
        /// <returns>
        ///     elections.election*.electionId
        ///     elections.election*.name
        ///     elections.election*.stateId
        ///     elections.election*.officeTypeId
        ///     elections.election*.special
        ///     elections.election*.electionYear
        ///     elections.election*.stage*.stageId
        ///     elections.election*.stage*.name
        ///     elections.election*.stage*.stateId
        ///     elections.election*.stage*.electionDate
        ///     elections.election*.stage*.filingDeadline
        ///     elections.election*.stage*.npatMailed
        /// </returns>
        public string GetUrlRequestForElectionInformationForElectionId(string electionId)
        {
            string api = _votesmartApi;
            string path = "Election.getElection?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "electionId=", electionId);
        }





        /// <summary>
        ///     VoteSmart API Class: Election
        ///     This method grabs district basic election data according to zip code.
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="zipCodeSuffix"></param>
        /// <param name="year"></param>
        /// <returns>
        ///     elections.election*.electionId
        ///     elections.election*.name
        ///     elections.election*.stateId
        ///     elections.election*.officeTypeId
        ///     elections.election*.special
        ///     elections.election*.electionYear        
        /// </returns>
        public string GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear(string zipCode, string zipCodeSuffix, int year)
        {// Tests Generated
            string api = _votesmartApi;
            string path = "Election.getElectionByZip?";
            string key = _votesmartApiKey;
            string andChar = "&";

            if (year <= 0)
                year = DateTime.Today.Year;

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "zip5=", zipCode,
                                 andChar, "zip4=", zipCodeSuffix,
                                 andChar, "year=", year);
        }



        /// <summary>
        ///     VoteSmart API Class: Local
        ///     Fetches counties in a state
        ///     set default state to Ohio
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>
        ///     counties.county*.localId
        ///     counties.county*.name
        ///     counties.county*.url
        /// </returns>
        public string GetUrlRequestForCountiesForSuppliedState(string stateId = "OH")
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     cities.city*.localId
        ///     cities.city*.name
        ///     cities.city*.url
        /// </returns>
        public string GetUrlRequestForCitiesForSuppliedState(string stateId = "OH")
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     candidatelist.candidate*.candidateId
        ///     candidatelist.candidate*.firstName
        ///     candidatelist.candidate*.nickName
        ///     candidatelist.candidate*.middleName
        ///     candidatelist.candidate*.lastName
        ///     candidatelist.candidate*.suffix
        ///     candidatelist.candidate*.title
        ///     candidatelist.candidate*.electionParties
        ///     candidatelist.candidate*.electionDistrictId
        ///     candidatelist.candidate*.electionStateId
        ///     candidatelist.candidate*.officeParties
        ///     candidatelist.candidate*.officeDistrictId
        ///     candidatelist.candidate*.officeDistrictName
        ///     candidatelist.candidate*.officeStateId
        ///     candidatelist.candidate*.officeId
        ///     candidatelist.candidate*.officeName
        ///     candidatelist.candidate*.officeTypeId
        /// </returns>
        public string GetUrlRequestForCandidatesForSpecifiedLocation(string localId)
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     officeTypes.type*.officeTypeId
        ///     officeTypes.type*.officeLevelId
        ///     officeTypes.type*.officeBranchId
        ///     officeTypes.type*.name
        /// </returns>
        public string GetUrlRequestForAllOfficeTypes()
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     branches.branch*.officeBranchId
        ///     branches.branch*.name
        /// </returns>
        public string GetUrlRequestForAllBranchesOfGovernment()
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     levels.level*.officeLevelId
        ///     levels.level*.name
        /// </returns>
        public string GetUrlRequestForAllLevelsOfGovernment()
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     offices.office*.officeId
        ///     offices.office*.officeTypeId
        ///     offices.office*.officeLevelId
        ///     offices.office*.officeBranchId
        ///     offices.office*.name
        ///     offices.office*.title
        ///     offices.office*.shortTitle
        /// </returns>
        public string GetUrlRequestForAllOfficesForSpecifiedType(string officeTypeId)
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     offices.office*.officeId
        ///     offices.office*.officeTypeId
        ///     offices.office*.officeLevelId
        ///     offices.office*.officeBranchId
        ///     offices.office*.name
        ///     offices.office*.title
        ///     offices.office*.shortTitle
        /// </returns>
        public string GetUrlRequestForAllOfficesForSpecifiedLevel(string levelId)
        {// Tests Generated
            string api = _votesmartApi;
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
        /// </returns>
        public string GetUrlRequestForAllOfficesForSpecifiedTypeAndLevel(string officeTypeId, string officeLevelId)
        {
            string api = _votesmartApi;
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
        ///     <errorMessage>Unknown error.</errorMessage>
        /// </returns>        
        public string GetUrlRequestForAllOfficesForSpecifiedBranchAndLevel(string branchId, string levelId)
        {
            string api = _votesmartApi;
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
        /// </returns>
        public string GetUrlRequestForAllOfficialsForSpecifiedState(string stateId = "OH")
        {
            string api = _votesmartApi;
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
        ///     <errorMessage>No officials found matching this criteria.</errorMessage>
        /// </returns>
        public string GetUrlRequestForAllOfficialsForSpecifiedOfficeAndState(string officeId, string stateId = "OH")
        {
            string api = _votesmartApi;
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
        ///     <errorMessage>No candidates found matching this criteria.</errorMessage>
        /// </returns>
        public string GetUrlRequestForAllOfficialsForSpecifiedOfficeTypeAndState(string officeTypeId, string stateId = "OH")
        {
            string api = _votesmartApi;
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
        ///     candidateList.candidate*.candidateId
        ///     candidateList.candidate*.firstName
        ///     candidateList.candidate*.nickName
        ///     candidateList.candidate*.middleName
        ///     candidateList.candidate*.lastName
        ///     candidateList.candidate*.suffix
        ///     candidateList.candidate*.title
        ///     candidateList.candidate*.electionParties
        ///     candidateList.candidate*.officeParties
        ///     candidateList.candidate*.officeStatus
        ///     candidateList.candidate*.officeDistrictId
        ///     candidateList.candidate*.officeDistrictName
        ///     candidateList.candidate*.officeTypeId
        ///     candidateList.candidate*.officeId
        ///     candidateList.candidate*.officeName
        ///     candidateList.candidate*.officeStateId
        /// </returns>
        public string GetUrlRequestForAllOfficialsWithMatchingLastName(string lastName)
        {// Tests Generated
            string api = _votesmartApi;
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
        ///     candidateList.candidate*.candidateId
        ///     candidateList.candidate*.firstName
        ///     candidateList.candidate*.nickName
        ///     candidateList.candidate*.middleName
        ///     candidateList.candidate*.lastName
        ///     candidateList.candidate*.suffix
        ///     candidateList.candidate*.title
        ///     candidateList.candidate*.electionParties
        ///     candidateList.candidate*.officeParties
        ///     candidateList.candidate*.officeStatus
        ///     candidateList.candidate*.officeDistrictId
        ///     candidateList.candidate*.officeDistrictName
        ///     candidateList.candidate*.officeTypeId
        ///     candidateList.candidate*.officeId
        ///     candidateList.candidate*.officeName
        ///     candidateList.candidate*.officeStateId
        /// </returns>
        public string GetUrlRequestForAllOfficialsWithSimilarLastName(string lastName)
        {// Tests Generated
            string api = _votesmartApi;
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
        /// </returns>
        public string GetUrlRequestForAllOfficialsByDistrict(string districtId)
        {
            string api = _votesmartApi;
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
        /// </returns>
        public string GetUrlRequestForAllOfficialsByDistrict(string zip5, string zip4)
        {
            string api = _votesmartApi;
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
        /// </returns>
        public string GetUrlRequestForAllStates()
        {
            string api = _votesmartApi;
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
        /// </returns>
        public string GetUrlRequestForInformationForSpecifiedState(string stateId = "OH")
        {
            string api = _votesmartApi;
            string path = "State.getState?";
            string key = _votesmartApiKey;
            string andChar = "&";

            return string.Concat(api, path,
                                 andChar, "key=", key,
                                 andChar, "stateId=", stateId);
        }











    }
}