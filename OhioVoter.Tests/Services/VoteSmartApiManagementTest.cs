using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhioVoter.Services;
using OhioVoter.ViewModels.VoteSmart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OhioVoter.Tests.Services
{
    [TestClass]
    public class VoteSmartApiManagementTest
    {
        private static string _votesmartApiKey = "1c5073e4c73532c7d2daf51651023d8f";


        // *********************************************************
        // GetVoteSmartElectionInformationFromSuppliedZipCodeAndYear
        // *********************************************************



        [TestMethod]
        public void Test_GetVoteSmartElectionInformationFromSuppliedZipCodeAndYear_ValidNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = "45069";
            string zipCodeSuffix = "3915";
            int year = 2016;

            // Act
            Elections result = service.GetVoteSmartElectionInformationFromSuppliedZipCodeAndYear(zipCode, zipCodeSuffix, year);

            // Assert
            Assert.AreNotEqual(0, result.Election.Count);
            Assert.AreEqual("OH", result.Election[0].StateId);
        }



        // *****************************************************************************
        // GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear
        // *****************************************************************************



        [TestMethod]
        public void Test_GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear_ValidNameAndYearProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "Strickland";
            int year = 2016;
            string stageId = "";

            // Act
            List<Candidate> result = service.GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear(lastName, year, stageId);

            // Assert
            Assert.AreNotEqual(0, result.Count);
        }




        [TestMethod]
        public void Test_GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear_NullNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = null;
            int year = DateTime.Today.Year;
            string stageId = "";

            // Act
            List<Candidate> result = service.GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear(lastName, year, stageId);

            // Assert
            Assert.AreEqual(0, result.Count);
        }



        [TestMethod]
        public void Test_GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear_EmptyNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "";
            int year = DateTime.Today.Year;
            string stageId = "";

            // Act
            List<Candidate> result = service.GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear(lastName, year, stageId);

            // Assert
            Assert.AreEqual(0, result.Count);
        }



        // ****************************************************
        // GetVoteSmartSimilarCandidateListFromSuppliedLastName
        // ****************************************************

        [TestMethod]
        public void Test_GetVoteSmartSimilarCandidateListFromSuppliedLastName_ValidNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "O";

            // Act
            List<Candidate> result = service.GetVoteSmartSimilarCandidateListFromSuppliedLastName(lastName);

            // Assert
            Assert.AreNotEqual(0, result.Count);
        }



        [TestMethod]
        public void Test_GetVoteSmartSimilarCandidateListFromSuppliedLastName_NoNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "";

            // Act
            List<Candidate> result = service.GetVoteSmartSimilarCandidateListFromSuppliedLastName(lastName);

            // Assert
            Assert.AreEqual(0, result.Count);
        }



        [TestMethod]
        public void Test_GetVoteSmartSimilarCandidateListFromSuppliedLastName_NullNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = null;

            // Act
            List<Candidate> result = service.GetVoteSmartSimilarCandidateListFromSuppliedLastName(lastName);

            // Assert
            Assert.AreEqual(0, result.Count);
        }



        // ***********
        // MakeRequest
        // ***********



        [TestMethod]
        public void Test_MakeRequest_ValidNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateId = "OH";
            string requestUrl = string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateId);

            // Act
            XmlDocument result = service.MakeRequest(requestUrl);

            // Assert
            Assert.AreEqual(requestUrl, result.BaseURI);
            string innerText = "Project Vote Smart - Counties - http://votesmart.org/official_local.php?state_id=3262Adams Countyhttp://adamscountyoh.com/3263Allen Countyhttp://www.co.allen.oh.us/3264Ashland Countyhttp://www.ashlandcounty.org/3265Ashtabula Countyhttp://www.co.ashtabula.oh.us/3266Athens Countyhttp://www.athenscountygovernment.com/3267Auglaize Countyhttp://www2.auglaizecounty.org/3268Belmont Countyhttp://www.belmontcountyohio.org/3269Brown Countyhttp://www.browncountyohio.gov/3270Butler Countyhttp://www.butlercountyohio.org/3271Carroll Countyhttp://www.carrollcountyohio.us/3272Champaign Countyhttp://www.co.champaign.oh.us/3273Clark Countyhttp://www.clarkcountyohio.gov/3274Clermont Countyhttp://www.co.clermont.oh.us/3275Clinton Countyhttp://co.clinton.oh.us/3276Columbiana Countyhttp://www.columbianacounty.org/3277Coshocton Countyhttp://www.coshoctoncounty.net3278Crawford Countyhttp://www.crawford-co.org/3279Cuyahoga Countyhttp://cuyahogacounty.us/3280Darke Countyhttp://co.darke.oh.us/3281Defiance Countyhttp://www.defiance-county.com/3282Delaware Countyhttp://www.co.delaware.oh.us/3283Erie Countyhttp://www.erie-county-ohio.net/3284Fairfield Countyhttp://www.co.fairfield.oh.us/3285Fayette Countyhttp://www.fayette-co-oh.com/3286Franklin Countyhttp://www.co.franklin.oh.us/3287Fulton Countyhttp://www.fultoncountyoh.com/3288Gallia Countyhttp://www.gallianet.net/3289Geauga Countyhttp://www.co.geauga.oh.us/3290Greene Countyhttp://www.co.greene.oh.us/3291Guernsey Countyhttp://www.guernseycounty.org/3292Hamilton Countyhttp://www.hamilton-co.org/hc/default.asp3293Hancock Countyhttp://www.co.hancock.oh.us/3294Hardin County3295Harrison Countyhttp://www.harrisoncountyohio.org/3296Henry Countyhttp://www.henrycountyohio.com/3297Highland Countyhttp://www.co.highland.oh.us/3298Hocking Countyhttp://www.co.hocking.oh.us/3299Holmes Countyhttp://www.co.holmes.oh.us/3300Huron Countyhttp://www.huroncounty-oh.gov/3301Jackson Countyhttp://www.jacksoncountygovernment.org/3302Jefferson Countyhttp://www.jeffersoncountyoh.com/cgi-bin/template.pl?3303Knox Countyhttp://www.co.knox.oh.us/3304Lake Countyhttp://www.lakecountyohio.org/3305Lawrence Countyhttp://www.lawrencecountyohio.org/3306Licking Countyhttp://www.lcounty.com/3307Logan Countyhttp://www.co.logan.oh.us/3308Lorain Countyhttp://www.loraincounty.us/3309Lucas Countyhttp://www.lucascountyoh.gov/3310Madison Countyhttp://www.co.madison.oh.us/3311Mahoning Countyhttp://www.mahoningcountyoh.gov/3312Marion Countyhttp://mcoprx.co.marion.oh.us/3313Medina Countyhttp://www.co.medina.oh.us/3314Meigs County3315Mercer Countyhttp://www.mercercountyohio.org/3316Miami Countyhttp://www.co.miami.oh.us/3317Monroe Countyhttp://www.monroecountyohio.net/3318Montgomery Countyhttp://www.co.montgomery.oh.us/3319Morgan Countyhttp://www.morgancounty-oh.gov/3320Morrow Countyhttp://www.morrowcounty.info/www/3321Muskingum Countyhttp://www.muskingumcounty.org/3322Noble County3323Ottawa Countyhttp://www.co.ottawa.oh.us/3324Paulding Countyhttp://www.pauldingcountyoh.com/3325Perry County3326Pickaway Countyhttp://www.pickaway.org/3327Pike County3328Portage Countyhttp://www.co.portage.oh.us/3329Preble Countyhttp://www.prebco.org/3330Putnam Countyhttp://www.putnamcountyohio.gov/3331Richland Countyhttp://www.richlandcountyoh.us/3332Ross Countyhttp://www.co.ross.oh.us/3333Sandusky Countyhttp://www.sandusky-county.org/3334Scioto Countyhttp://www.sciotocountyohio.com/3335Seneca Countyhttp://www.seneca-county.com/3336Shelby Countyhttp://www.co.shelby.oh.us/3337Stark Countyhttp://www.co.stark.oh.us/3338Summit Countyhttps://co.summitoh.net/3339Trumbull Countyhttp://www.co.trumbull.oh.us/3340Tuscarawas Countyhttp://www.co.tuscarawas.oh.us/3341Union Countyhttp://www.co.union.oh.us/3342Van Wert Countyhttp://www.vanwertcounty.org/3343Vinton Countyhttp://www.vintoncounty.com3344Warren Countyhttp://www.co.warren.oh.us/3345Washington Countyhttp://www.washingtongov.org/3346Wayne Countyhttp://www.wayneohio.org/3347Williams Countyhttp://www.co.williams.oh.us/3348Wood Countyhttp://www.co.wood.oh.us/3349Wyandot Countyhttp://www.co.wyandot.oh.us/";
            Assert.AreEqual(innerText, result.InnerText);
        }



        [TestMethod]
        public void Test_MakeRequest_InvalidNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateId = "QQ";
            string requestUrl = string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateId);

            // Act
            XmlDocument result = service.MakeRequest(requestUrl);

            // Assert
            Assert.AreEqual(requestUrl, result.BaseURI);
            string innerText = "This probably shouldn't have happened.";
            Assert.AreEqual(innerText, result.InnerText);
        }



        [TestMethod]
        public void Test_MakeRequest_NullNameProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateId = null;
            string requestUrl = string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateId);

            // Act
            XmlDocument result = service.MakeRequest(requestUrl);

            // Assert
            Assert.AreEqual(requestUrl, result.BaseURI);
            string innerText = "State ID has not been provided.";
            Assert.AreEqual(innerText, result.InnerText);
        }



        // *******************************
        // ProcessResponseForCandidateList
        // *******************************



        [TestMethod]
        public void Test_ProcessResponseForCandidateList_ValidXmlDocument()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = GetXmlDocumentForCandidateList();

            // Act
            List<Candidate> result = service.ProcessResponseForCandidateList(xmlDoc);

            // Assert
            Assert.AreEqual("152042", result[0].ElectionCandidateId);
            Assert.AreEqual("Erin", result[0].FirstName);
            Assert.AreEqual("Oban", result[0].LastName);
            Assert.AreEqual("174396", result[1].ElectionCandidateId);
            Assert.AreEqual("Shawn", result[1].FirstName);
            Assert.AreEqual("Oban", result[1].LastName);
        }



        [TestMethod]
        public void Test_ProcessResponseForCandidateList_NullXmlDocument()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = null;

            // Act
            List<Candidate> result = service.ProcessResponseForCandidateList(xmlDoc);

            // Assert
            Assert.AreEqual(0, result.Count);
        }



        [TestMethod]
        public void Test_ProcessResponseForCandidateList_NotCandidateXmlDocument()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = GetXmlDocumentForOfficeList();

            // Act
            List<Candidate> result = service.ProcessResponseForCandidateList(xmlDoc);

            // Assert
            Assert.AreEqual(0, result.Count);
        }



        // ****************************************
        // GetUrlRequestForCountiesForSuppliedState
        // ****************************************

        [TestMethod]
        public void Test_GetUrlRequestForCountiesForSuppliedState_StateIsOhio()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "OH";

            // Act
            string result = service.GetUrlRequestForCountiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCountiesForSuppliedState_StateNotOhio()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "AL";

            // Act
            string result = service.GetUrlRequestForCountiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCountiesForSuppliedState_StateEmpty()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "";

            // Act
            string result = service.GetUrlRequestForCountiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCountiesForSuppliedState_StateNull()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = null;

            // Act
            string result = service.GetUrlRequestForCountiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCountiesForSuppliedState_StateNotValidNumbers()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "22";

            // Act
            string result = service.GetUrlRequestForCountiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCounties?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        // **************************************
        // GetUrlRequestForCitiesForSuppliedState
        // **************************************

        [TestMethod]
        public void Test_GetUrlRequestForCitiesForSuppliedState_StateIsOhio()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "OH";

            // Act
            string result = service.GetUrlRequestForCitiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCities?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCitiesForSuppliedState_StateNotOhio()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "AL";

            // Act
            string result = service.GetUrlRequestForCitiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCities?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCitiesForSuppliedState_StateEmpty()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "";

            // Act
            string result = service.GetUrlRequestForCitiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCities?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCitiesForSuppliedState_StateNull()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = null;

            // Act
            string result = service.GetUrlRequestForCitiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCities?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCitiesForSuppliedState_StateNotValidNumbers()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "22";

            // Act
            string result = service.GetUrlRequestForCitiesForSuppliedState(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getCities?&key=", _votesmartApiKey, "&stateId=", stateAbbreviation), result);
        }



        // **********************************************
        // GetUrlRequestForCandidatesForSpecifiedLocation
        // **********************************************

        [TestMethod]
        public void Test_GetUrlRequestForCandidatesForSpecifiedLocation_CountyInOhio()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string hamiltonCountyLocalId = "3292";

            // Act
            string result = service.GetUrlRequestForCandidatesForSpecifiedLocation(hamiltonCountyLocalId);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getOfficials?&key=", _votesmartApiKey, "&localId=", hamiltonCountyLocalId), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCandidatesForSpecifiedLocation_CityInOhio()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string cincinnatiCityLocalId = "930";

            // Act
            string result = service.GetUrlRequestForCandidatesForSpecifiedLocation(cincinnatiCityLocalId);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getOfficials?&key=", _votesmartApiKey, "&localId=", cincinnatiCityLocalId), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCandidatesForSpecifiedLocation_LocationNotOhio()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "AL";

            // Act
            string result = service.GetUrlRequestForCandidatesForSpecifiedLocation(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getOfficials?&key=", _votesmartApiKey, "&localId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCandidatesForSpecifiedLocation_StateEmpty()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "";

            // Act
            string result = service.GetUrlRequestForCandidatesForSpecifiedLocation(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getOfficials?&key=", _votesmartApiKey, "&localId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCandidatesForSpecifiedLocation_StateNull()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = null;

            // Act
            string result = service.GetUrlRequestForCandidatesForSpecifiedLocation(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getOfficials?&key=", _votesmartApiKey, "&localId=", stateAbbreviation), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForCandidatesForSpecifiedLocation_StateNotValidNumbers()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string stateAbbreviation = "22";

            // Act
            string result = service.GetUrlRequestForCandidatesForSpecifiedLocation(stateAbbreviation);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Local.getOfficials?&key=", _votesmartApiKey, "&localId=", stateAbbreviation), result);
        }



        // ******************************
        // GetUrlRequestForAllOfficeTypes
        // ******************************

        [TestMethod]
        public void Test_GetUrlRequestForAllOfficeTypes()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();

            // Act
            string result = service.GetUrlRequestForAllOfficeTypes();

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Office.getTypes?&key=", _votesmartApiKey), result);
        }



        // ***************************************
        // GetUrlRequestForAllBranchesOfGovernment
        // ***************************************

        [TestMethod]
        public void Test_GetUrlRequestForAllBranchesOfGovernment()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();

            // Act
            string result = service.GetUrlRequestForAllBranchesOfGovernment();

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Office.getBranches?&key=", _votesmartApiKey), result);
        }



        // *************************************
        // GetUrlRequestForAllLevelsOfGovernment
        // *************************************

        [TestMethod]
        public void Test_GetUrlRequestForAllLevelsOfGovernment()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();

            // Act
            string result = service.GetUrlRequestForAllLevelsOfGovernment();

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Office.getLevels?&key=", _votesmartApiKey), result);
        }



        // ******************************************
        // GetUrlRequestForAllOfficesForSpecifiedType
        // ******************************************

        [TestMethod]
        public void Test_GetUrlRequestForAllOfficesForSpecifiedType_ValidOfficeType()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string officeTypeId = "P";

            // Act
            string result = service.GetUrlRequestForAllOfficesForSpecifiedType(officeTypeId);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Office.getOfficesByType?&key=", _votesmartApiKey, "&officeTypeId=", officeTypeId), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForAllOfficesForSpecifiedType_NullOfficeType()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string officeTypeId = null;

            // Act
            string result = service.GetUrlRequestForAllOfficesForSpecifiedType(officeTypeId);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Office.getOfficesByType?&key=", _votesmartApiKey, "&officeTypeId=", officeTypeId), result);
        }



        // *******************************************
        // GetUrlRequestForAllOfficesForSpecifiedLevel
        // *******************************************

        [TestMethod]
        public void Test_GetUrlRequestForAllOfficesForSpecifiedLevel_ValidLevidId()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string levelId = "F";

            // Act
            string result = service.GetUrlRequestForAllOfficesForSpecifiedLevel(levelId);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Office.getOfficesByLevel?&key=", _votesmartApiKey, "&levelId=", levelId), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForAllOfficesForSpecifiedLevel_NullLevidId()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string levelId = null;

            // Act
            string result = service.GetUrlRequestForAllOfficesForSpecifiedLevel(levelId);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Office.getOfficesByLevel?&key=", _votesmartApiKey, "&levelId=", levelId), result);
        }



        // ************************************************
        // GetUrlRequestForAllOfficialsWithMatchingLastName
        // ************************************************



        [TestMethod]
        public void Test_GetUrlRequestForAllOfficialsWithMatchingLastName_ValidLastName()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "Clinton";

            // Act
            string result = service.GetUrlRequestForAllOfficialsWithMatchingLastName(lastName);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Officials.getByLastname?&key=", _votesmartApiKey, "&lastName=", lastName), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForAllOfficialsWithMatchingLastName_EmptyLastName()
        {// Tests Generated
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "";

            // Act
            string result = service.GetUrlRequestForAllOfficialsWithMatchingLastName(lastName);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Officials.getByLastname?&key=", _votesmartApiKey, "&lastName=", lastName), result);
        }



        // ***********************************************
        // GetUrlRequestForAllOfficialsWithSimilarLastName
        // ***********************************************

        [TestMethod]
        public void Test_GetUrlRequestForAllOfficialsWithSimilarLastName_ValidLastName()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "Clinton";

            // Act
            string result = service.GetUrlRequestForAllOfficialsWithSimilarLastName(lastName);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Officials.getByLevenshtein?&key=", _votesmartApiKey, "&lastName=", lastName), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForAllOfficialsWithSimilarLastName_EmptyLastName()
        {// Tests Generated
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = "";

            // Act
            string result = service.GetUrlRequestForAllOfficialsWithSimilarLastName(lastName);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Officials.getByLevenshtein?&key=", _votesmartApiKey, "&lastName=", lastName), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForAllOfficialsWithSimilarLastName_NullLastName()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string lastName = null;

            // Act
            string result = service.GetUrlRequestForAllOfficialsWithSimilarLastName(lastName);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Officials.getByLevenshtein?&key=", _votesmartApiKey, "&lastName=", lastName), result);
        }



        // *****************************************************
        // GetUrlRequestForDistrictInformationForSuppliedZipCode
        // *****************************************************



        [TestMethod]
        public void Test_GetUrlRequestForDistrictInformationForSuppliedZipCode_ValidZipCodeAndSuffix()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = "45224";
            string zipCodeSuffix = "2636";

            // Act
            string result = service.GetUrlRequestForDistrictInformationForSuppliedZipCode(zipCode, zipCodeSuffix);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/District.getByZip?&key=", _votesmartApiKey, "&zip5=", zipCode, "&zip4=", zipCodeSuffix), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForDistrictInformationForSuppliedZipCode_NullZipCodeAndSuffix()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = null;
            string zipCodeSuffix = null;

            // Act
            string result = service.GetUrlRequestForDistrictInformationForSuppliedZipCode(zipCode, zipCodeSuffix);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/District.getByZip?&key=", _votesmartApiKey, "&zip5=", zipCode, "&zip4=", zipCodeSuffix), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForDistrictInformationForSuppliedZipCode_NullSuffix()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = "45224";
            string zipCodeSuffix = null;

            // Act
            string result = service.GetUrlRequestForDistrictInformationForSuppliedZipCode(zipCode, zipCodeSuffix);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/District.getByZip?&key=", _votesmartApiKey, "&zip5=", zipCode, "&zip4=", zipCodeSuffix), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForDistrictInformationForSuppliedZipCode_EmptyZipCodeAndSuffix()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = "";
            string zipCodeSuffix = "";

            // Act
            string result = service.GetUrlRequestForDistrictInformationForSuppliedZipCode(zipCode, zipCodeSuffix);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/District.getByZip?&key=", _votesmartApiKey, "&zip5=", zipCode, "&zip4=", zipCodeSuffix), result);
        }



        // ************************************************************
        // GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear
        // ************************************************************



        [TestMethod]
        public void Test_GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear_ValidZipCodeAndSuffixAndYear()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = "45069";
            string zipCodeSuffix = "3915";
            int year = 2016;

            // Act
            string result = service.GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear(zipCode, zipCodeSuffix, year);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Election.getElectionByZip?&key=", _votesmartApiKey, "&zip5=", zipCode, "&zip4=", zipCodeSuffix, "&year=", year), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear_NoYearProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = "45069";
            string zipCodeSuffix = "3915";
            int year = DateTime.Today.Year;
            int noYear = 0;

            // Act
            string result = service.GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear(zipCode, zipCodeSuffix, noYear);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Election.getElectionByZip?&key=", _votesmartApiKey, "&zip5=", zipCode, "&zip4=", zipCodeSuffix, "&year=", year), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear_NegativeYearProvided()
        {
            // Arrange
            VoteSmartApiManagement service = new VoteSmartApiManagement();
            string zipCode = "45069";
            string zipCodeSuffix = "3915";
            int year = DateTime.Today.Year;
            int noYear = -2016;

            // Act
            string result = service.GetUrlRequestForElectionInformationForSuppliedZipCodeAndYear(zipCode, zipCodeSuffix, noYear);

            // Assert
            Assert.AreEqual(string.Concat("http://api.votesmart.org/Election.getElectionByZip?&key=", _votesmartApiKey, "&zip5=", zipCode, "&zip4=", zipCodeSuffix, "&year=", year), result);
        }




        // ********************************************************************************



        private XmlDocument GetXmlDocumentForCandidateList()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode candidateListNode = doc.CreateElement("candidateList");
            doc.AppendChild(candidateListNode);

            XmlNode candidateNode = doc.CreateElement("candidate");
            candidateListNode.AppendChild(candidateNode);
            XmlNode candidateIdNode = doc.CreateElement("candidateId");
            candidateIdNode.AppendChild(doc.CreateTextNode("152042"));
            candidateNode.AppendChild(candidateIdNode);
            XmlNode firstNameNode = doc.CreateElement("firstName");
            firstNameNode.AppendChild(doc.CreateTextNode("Erin"));
            candidateNode.AppendChild(firstNameNode);
            XmlNode lastNameNode = doc.CreateElement("lastName");
            lastNameNode.AppendChild(doc.CreateTextNode("Oban"));
            candidateNode.AppendChild(lastNameNode);

            candidateNode = doc.CreateElement("candidate");
            candidateListNode.AppendChild(candidateNode);
            candidateIdNode = doc.CreateElement("candidateId");
            candidateIdNode.AppendChild(doc.CreateTextNode("174396"));
            candidateNode.AppendChild(candidateIdNode);
            firstNameNode = doc.CreateElement("firstName");
            firstNameNode.AppendChild(doc.CreateTextNode("Shawn"));
            candidateNode.AppendChild(firstNameNode);
            lastNameNode = doc.CreateElement("lastName");
            lastNameNode.AppendChild(doc.CreateTextNode("Oban"));
            candidateNode.AppendChild(lastNameNode);

            return doc;
        }



        // this can be updated with valid office list data if/when needed
        private XmlDocument GetXmlDocumentForOfficeList()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode candidateListNode = doc.CreateElement("officeList");
            doc.AppendChild(candidateListNode);

            XmlNode candidateNode = doc.CreateElement("office");
            candidateListNode.AppendChild(candidateNode);
            XmlNode candidateIdNode = doc.CreateElement("officeId");
            candidateIdNode.AppendChild(doc.CreateTextNode("152042"));
            candidateNode.AppendChild(candidateIdNode);
            XmlNode firstNameNode = doc.CreateElement("officeLongName");
            firstNameNode.AppendChild(doc.CreateTextNode("Erin"));
            candidateNode.AppendChild(firstNameNode);
            XmlNode lastNameNode = doc.CreateElement("officeShortName");
            lastNameNode.AppendChild(doc.CreateTextNode("Oban"));
            candidateNode.AppendChild(lastNameNode);

            candidateNode = doc.CreateElement("office");
            candidateListNode.AppendChild(candidateNode);
            candidateIdNode = doc.CreateElement("officeId");
            candidateIdNode.AppendChild(doc.CreateTextNode("174396"));
            candidateNode.AppendChild(candidateIdNode);
            firstNameNode = doc.CreateElement("officeLongName");
            firstNameNode.AppendChild(doc.CreateTextNode("Shawn"));
            candidateNode.AppendChild(firstNameNode);
            lastNameNode = doc.CreateElement("officeShortName");
            lastNameNode.AppendChild(doc.CreateTextNode("Oban"));
            candidateNode.AppendChild(lastNameNode);

            return doc;
        }



    }
}
