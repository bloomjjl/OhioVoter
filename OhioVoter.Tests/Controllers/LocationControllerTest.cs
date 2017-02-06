﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhioVoter.Controllers;
using OhioVoter.ViewModels.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Tests.Controllers
{
    [TestClass]
    public class LocationControllerTest
    {
        [TestMethod]
        public void Test_DisplayVoterLocationSideBar_SideBarViewModelWithValidLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithAllValidLocations();

            // Act
            PartialViewResult result = controller.DisplayVoterLocationSideBar(sideBarViewModel) as PartialViewResult;

            // Assert
            Assert.AreEqual("_VoterLocation", result.ViewName);
        }



        [TestMethod]
        public void Test_DisplayVoterLocationSideBar_SideBarViewModelWithNullLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithAllNullLocations();

            // Act
            PartialViewResult result = controller.DisplayVoterLocationSideBar(sideBarViewModel) as PartialViewResult;

            // Assert
            Assert.AreEqual("_VoterLocationForm", result.ViewName);
        }



        [TestMethod]
        public void Test_DisplayGeneralLocationSideBar_SideBarViewModelWithValidLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithAllValidLocations();

            // Act
            PartialViewResult result = controller.DisplayGeneralLocationSideBar(sideBarViewModel) as PartialViewResult;

            // Assert
            Assert.AreEqual("_VoterGeneralInformation", result.ViewName);
        }



        [TestMethod]
        public void Test_DisplayGeneralLocationSideBar_SideBarViewModelWithNullLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithAllNullLocations();

            // Act
            PartialViewResult result = controller.DisplayGeneralLocationSideBar(sideBarViewModel) as PartialViewResult;

            // Assert
            Assert.AreEqual("_VoterGeneralInformation", result.ViewName);
        }



        [TestMethod]
        public void Test_GetAddressForOhioSecretaryOfState()
        {
            // Arrange
            LocationController controller = new LocationController();

            // Act
            LocationViewModel result = controller.GetAddressForOhioSecretaryOfState();

            // Assert
            Assert.AreEqual("Display", result.Status);
            Assert.AreEqual("OHIO SECRETARY OF STATE", result.LocationName);
            Assert.AreEqual("180 E. BROAD ST., 15TH FLOOR", result.StreetAddress);
            Assert.AreEqual("COLUMBUS", result.City);
            Assert.AreEqual("OH", result.StateAbbreviation);
            Assert.AreEqual("43215-3726", result.ZipCode);
            Assert.AreEqual("http://www.sos.state.oh.us/elections.aspx", result.Website);
        }



        [TestMethod]
        public void Test_ValidateZipCodeLocatedInOhio_FromOhio()
        {
            // Arrange
            LocationController controller = new LocationController();
            string zipCode = "44240";

            // Act
            bool result = controller.ValidateZipCodeLocatedInOhio(zipCode);

            // Assert
            Assert.AreEqual(true, result);
        }



        [TestMethod]
        public void Test_ValidateZipCodeLocatedInOhio_NotFromOhio()
        {
            // Arrange
            LocationController controller = new LocationController();
            string zipCode = "59858";

            // Act
            bool result = controller.ValidateZipCodeLocatedInOhio(zipCode);

            // Assert
            Assert.AreEqual(false, result);
        }



        [TestMethod]
        public void Test_ValidateZipCodeLocatedInOhio_NotNull()
        {
            // Arrange
            LocationController controller = new LocationController();
            string zipCode = null;

            // Act
            bool result = controller.ValidateZipCodeLocatedInOhio(zipCode);

            // Assert
            Assert.AreEqual(false, result);
        }



        [TestMethod]
        public void Test_ValidateStreetAddressAndZipCodeLocatedInOhio_OhioAddressSupplied()
        {
            // Arrange
            LocationController controller = new LocationController();
            string streetAddress = "1303 Home Ave";
            string zipCode = "44310";

            // Act
            bool result = controller.ValidateStreetAddressAndZipCodeLocatedInOhio(streetAddress, zipCode);

            // Assert
            Assert.AreEqual(true, result);
        }



        [TestMethod]
        public void Test_ValidateSideBarLocations_ValidLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithAllValidLocations();

            // Act
            bool result = controller.ValidateSideBarLocations(sideBarViewModel);

            // Assert
            Assert.AreEqual(true, result);
        }



        [TestMethod]
        public void Test_ValidateSideBarLocations_NullVoterLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithNullVoterLocation();

            // Act
            bool result = controller.ValidateSideBarLocations(sideBarViewModel);

            // Assert
            Assert.AreEqual(false, result);
        }



        [TestMethod]
        public void Test_ValidateSideBarLocations_NullPollingLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithNullPollingLocation();

            // Act
            bool result = controller.ValidateSideBarLocations(sideBarViewModel);

            // Assert
            Assert.AreEqual(false, result);
        }



        [TestMethod]
        public void Test_ValidateSideBarLocations_NullCountyLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithNullCountyLocation();

            // Act
            bool result = controller.ValidateSideBarLocations(sideBarViewModel);

            // Assert
            Assert.AreEqual(false, result);
        }



        [TestMethod]
        public void Test_ValidateSideBarLocations_NullStateLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithNullStateLocation();

            // Act
            bool result = controller.ValidateSideBarLocations(sideBarViewModel);

            // Assert
            Assert.AreEqual(false, result);
        }



        [TestMethod]
        public void Test_ValidateSideBarLocations_AllNullLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithAllNullLocations();

            // Act
            bool result = controller.ValidateSideBarLocations(sideBarViewModel);

            // Assert
            Assert.AreEqual(false, result);
        }



        [TestMethod]
        public void Test_ValidateVoterLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel voterLocation = GetValidVoterLocation();

            // Act
            bool result = controller.ValidateVoterLocation(voterLocation);

            // Assert
            Assert.AreEqual(true, result);
        }



        [TestMethod]
        public void Test_ValidateVoterLocation_NullLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel voterLocation = new LocationViewModel();

            // Act
            bool result = controller.ValidateVoterLocation(voterLocation);

            // Assert
            Assert.AreEqual(false, result);
        }




        [TestMethod]
        public void Test_ValidatePollingLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel pollingLocation = GetValidPollingLocation();

            // Act
            bool result = controller.ValidatePollingLocation(pollingLocation);

            // Assert
            Assert.AreEqual(true, result);
        }



        [TestMethod]
        public void Test_ValidatePollingLocation_NullLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel pollingLocation = new LocationViewModel();

            // Act
            bool result = controller.ValidatePollingLocation(pollingLocation);

            // Assert
            Assert.AreEqual(false, result);
        }




        [TestMethod]
        public void Test_ValidateCountyLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel countyLocation = GetValidCountyLocation();

            // Act
            bool result = controller.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual(true, result);
        }



        [TestMethod]
        public void Test_ValidateCountyLocation_NullLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel countyLocation = new LocationViewModel();

            // Act
            bool result = controller.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual(false, result);
        }




        [TestMethod]
        public void Test_ValidateStateLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel stateLocation = GetValidStateLocation();

            // Act
            bool result = controller.ValidateStateLocation(stateLocation);

            // Assert
            Assert.AreEqual(true, result);
        }



        [TestMethod]
        public void Test_ValidateStateLocation_NullLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel stateLocation = new LocationViewModel();

            // Act
            bool result = controller.ValidateStateLocation(stateLocation);

            // Assert
            Assert.AreEqual(false, result);
        }




        [TestMethod]
        public void Test_GetSideBarViewModelFromGoogleCivicInformationAPI_ValidVoterLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBar = GetSideBarWithAllValidLocations();

            // Act
            SideBarViewModel result = controller.GetSideBarViewModelFromGoogleCivicInformationAPI(sideBar.VoterLocation);

            // Assert
            Assert.AreEqual(sideBar.VoterLocation.FullAddress, result.VoterLocation.FullAddress);
            Assert.AreEqual(sideBar.PollingLocation.FullAddress, result.PollingLocation.FullAddress);
            Assert.AreEqual(sideBar.CountyLocation.FullAddress, result.CountyLocation.FullAddress);
            //Assert.AreEqual(sideBar.StateLocation.FullAddress, result.StateLocation.FullAddress); DO NOT TEST state location always from Ohio
        }



        [TestMethod]
        public void Test_GetSideBarViewModelFromGoogleCivicInformationAPI_NullVoterLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBar = new SideBarViewModel();

            // Act
            SideBarViewModel result = controller.GetSideBarViewModelFromGoogleCivicInformationAPI(sideBar.VoterLocation);

            // Assert
            Assert.AreEqual("Update", result.VoterLocation.Status);
            Assert.AreEqual("Voter address is not valid.", result.VoterLocation.Message);
        }



        [TestMethod]
        public void Test_GetGoogleMapForPollingLocation_ValidLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel voterLocation = GetValidVoterLocation();
            LocationViewModel pollingLocation = GetValidPollingLocation();

            // Act
            string result = controller.GetGoogleMapForPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.IsNotNull(result);
        }



        [TestMethod]
        public void Test_GetGoogleMapForPollingLocation_NullVoterLocation()
        {// Tests Generated
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel voterLocation = new LocationViewModel();
            LocationViewModel pollingLocation = GetValidPollingLocation();

            // Act
            string result = controller.GetGoogleMapForPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.IsNotNull(result);
        }



        [TestMethod]
        public void Test_GetGoogleMapForPollingLocation_NullPollingLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel voterLocation = GetValidVoterLocation();
            LocationViewModel pollingLocation = new LocationViewModel();

            // Act
            string result = controller.GetGoogleMapForPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.IsNotNull(result);
        }



        [TestMethod]
        public void Test_GetGoogleMapForPollingLocation_NullLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            LocationViewModel voterLocation = new LocationViewModel();
            LocationViewModel pollingLocation = new LocationViewModel();

            // Act
            string result = controller.GetGoogleMapForPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.IsNotNull(result);
        }




        // **********************************************************



        private static SideBarViewModel GetSideBarWithAllValidLocations()
        {
            return new SideBarViewModel()
            {
                VoterLocation = GetValidVoterLocation(),
                PollingLocation = GetValidPollingLocation(),
                CountyLocation = GetValidCountyLocation(),
                StateLocation = GetValidStateLocation()
            };
        }



        private static SideBarViewModel GetSideBarWithNullVoterLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocation = new LocationViewModel(),
                PollingLocation = GetValidPollingLocation(),
                CountyLocation = GetValidCountyLocation(),
                StateLocation = GetValidStateLocation()
            };
        }


        private static SideBarViewModel GetSideBarWithNullPollingLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocation = GetValidVoterLocation(),
                PollingLocation = new LocationViewModel(),
                CountyLocation = GetValidCountyLocation(),
                StateLocation = GetValidStateLocation()
            };
        }


        private static SideBarViewModel GetSideBarWithNullCountyLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocation = GetValidVoterLocation(),
                PollingLocation = GetValidPollingLocation(),
                CountyLocation = new LocationViewModel(),
                StateLocation = GetValidStateLocation()
            };
        }


        private static SideBarViewModel GetSideBarWithNullStateLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocation = GetValidVoterLocation(),
                PollingLocation = GetValidPollingLocation(),
                CountyLocation = GetValidCountyLocation(),
                StateLocation = new LocationViewModel()
            };
        }


        private static SideBarViewModel GetSideBarWithAllNullLocations()
        {
            return new SideBarViewModel()
            {
                VoterLocation = new LocationViewModel(),
                PollingLocation = new LocationViewModel(),
                CountyLocation = new LocationViewModel(),
                StateLocation = new LocationViewModel()
            };
        }



        private static LocationViewModel GetValidVoterLocation()
        {
            return new LocationViewModel()
            {
                Status = "Display",
                StreetAddress = "9282 Gregg Drive",
                City = "West Chester Township",
                StateAbbreviation = "OH",
                ZipCode = "45069"
            };
        }



        private static LocationViewModel GetValidPollingLocation()
        {
            return new LocationViewModel()
            {
                Status = "Display",
                StreetAddress = "9113 CINCINNATI DAYTON RD",
                City = "WEST CHESTER",
                StateAbbreviation = "OH",
                ZipCode = "45069"
            };
        }



        private static LocationViewModel GetValidCountyLocation()
        {
            return new LocationViewModel()
            {
                Status = "Display",
                StreetAddress = "1802 Princeton Rd., Ste 600",
                City = "Hamilton",
                StateAbbreviation = "OH",
                ZipCode = "45011"
            };
        }



        private static LocationViewModel GetValidStateLocation()
        {
            return new LocationViewModel()
            {
                Status = "Display",
                StateAbbreviation = "OH"
            };
        }

    }
}
