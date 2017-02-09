using Microsoft.VisualStudio.TestTools.UnitTesting;
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



        // ***************************
        // DisplayVoterLocationSideBar
        // ***************************



        [TestMethod]
        public void Test_DisplayVoterLocationSideBar_SideBarViewModelWithValidLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBarViewModel = GetSideBarWithAllValidLocations();
            // controllerName?
            
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
            // controllerName?

            // Act
            PartialViewResult result = controller.DisplayVoterLocationSideBar(sideBarViewModel) as PartialViewResult;

            // Assert
            Assert.AreEqual("_VoterLocationForm", result.ViewName);
        }



        // *********************************
        // GetAddressForOhioSecretaryOfState
        // *********************************



        [TestMethod]
        public void Test_GetAddressForOhioSecretaryOfState()
        {
            // Arrange
            LocationController controller = new LocationController();

            // Act
            StateLocationViewModel result = controller.GetAddressForOhioSecretaryOfState();

            // Assert
            Assert.AreEqual("Display", result.Status);
            Assert.AreEqual("OHIO SECRETARY OF STATE", result.LocationName);
            Assert.AreEqual("180 E. BROAD ST., 15TH FLOOR", result.StreetAddress);
            Assert.AreEqual("COLUMBUS", result.City);
            Assert.AreEqual("OH", result.StateAbbreviation);
            Assert.AreEqual("43215", result.ZipCode);
            Assert.AreEqual("3726", result.ZipCodeSuffix);
            Assert.AreEqual("http://www.sos.state.oh.us/", result.Website);
        }


        
        // ****************************
        // ValidateZipCodeLocatedInOhio
        // ****************************



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



        // ********************************************
        // ValidateStreetAddressAndZipCodeLocatedInOhio
        // ********************************************



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



        // ************************
        // ValidateSideBarLocations
        // ************************



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



        // *********************
        // ValidateVoterLocation
        // *********************



        [TestMethod]
        public void Test_ValidateVoterLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            VoterLocationViewModel voterLocation = GetValidVoterLocation();

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
            VoterLocationViewModel voterLocation = new VoterLocationViewModel();

            // Act
            bool result = controller.ValidateVoterLocation(voterLocation);

            // Assert
            Assert.AreEqual(false, result);
        }



        // ***********************
        // ValidatePollingLocation
        // ***********************



        [TestMethod]
        public void Test_ValidatePollingLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            PollingLocationViewModel pollingLocation = GetValidPollingLocation();

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
            PollingLocationViewModel pollingLocation = new PollingLocationViewModel();

            // Act
            bool result = controller.ValidatePollingLocation(pollingLocation);

            // Assert
            Assert.AreEqual(false, result);
        }



        // **********************
        // ValidateCountyLocation
        // **********************



        [TestMethod]
        public void Test_ValidateCountyLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            CountyLocationViewModel countyLocation = GetValidCountyLocation();

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
            CountyLocationViewModel countyLocation = new CountyLocationViewModel();

            // Act
            bool result = controller.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual(false, result);
        }



        // *********************
        // ValidateStateLocation
        // *********************



        [TestMethod]
        public void Test_ValidateStateLocation_ValidLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            StateLocationViewModel stateLocation = GetValidStateLocation();

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
            StateLocationViewModel stateLocation = new StateLocationViewModel();

            // Act
            bool result = controller.ValidateStateLocation(stateLocation);

            // Assert
            Assert.AreEqual(false, result);
        }



        // ************************************************
        // GetSideBarViewModelFromGoogleCivicInformationAPI
        // ************************************************



        [TestMethod]
        public void Test_GetSideBarViewModelFromGoogleCivicInformationAPI_ValidVoterLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBar = GetSideBarWithAllValidLocations();

            // Act
            SideBarViewModel result = controller.GetSideBarViewModelFromGoogleCivicInformationAPI(sideBar.VoterLocationViewModel);

            // Assert
            Assert.AreEqual(sideBar.VoterLocationViewModel.FullAddress, result.VoterLocationViewModel.FullAddress);
            Assert.AreEqual(sideBar.PollingLocationViewModel.FullAddress, result.PollingLocationViewModel.FullAddress);
            Assert.AreEqual(sideBar.CountyLocationViewModel.FullAddress, result.CountyLocationViewModel.FullAddress);
            //Assert.AreEqual(sideBar.StateLocation.FullAddress, result.StateLocation.FullAddress); DO NOT TEST state location always from Ohio
        }



        [TestMethod]
        public void Test_GetSideBarViewModelFromGoogleCivicInformationAPI_NullVoterLocation()
        {
            // Arrange
            LocationController controller = new LocationController();
            SideBarViewModel sideBar = new SideBarViewModel();

            // Act
            SideBarViewModel result = controller.GetSideBarViewModelFromGoogleCivicInformationAPI(sideBar.VoterLocationViewModel);

            // Assert
            Assert.AreEqual("Update", result.VoterLocationViewModel.Status);
            Assert.AreEqual("Voter address is not valid.", result.VoterLocationViewModel.Message);
        }



        // ******************************
        // GetGoogleMapForPollingLocation
        // ******************************



        [TestMethod]
        public void Test_GetGoogleMapForPollingLocation_ValidLocations()
        {
            // Arrange
            LocationController controller = new LocationController();
            VoterLocationViewModel voterLocation = GetValidVoterLocation();
            PollingLocationViewModel pollingLocation = GetValidPollingLocation();

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
            VoterLocationViewModel voterLocation = new VoterLocationViewModel();
            PollingLocationViewModel pollingLocation = GetValidPollingLocation();

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
            VoterLocationViewModel voterLocation = GetValidVoterLocation();
            PollingLocationViewModel pollingLocation = new PollingLocationViewModel();

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
            VoterLocationViewModel voterLocation = new VoterLocationViewModel();
            PollingLocationViewModel pollingLocation = new PollingLocationViewModel();

            // Act
            string result = controller.GetGoogleMapForPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.IsNotNull(result);
        }




        // **********************************************************
        // **********************************************************


        private static SideBarViewModel GetSideBarWithAllValidLocations()
        {
            return new SideBarViewModel()
            {
                VoterLocationViewModel = GetValidVoterLocation(),
                PollingLocationViewModel = GetValidPollingLocation(),
                CountyLocationViewModel = GetValidCountyLocation(),
                StateLocationViewModel = GetValidStateLocation()
            };
        }



        private static SideBarViewModel GetSideBarWithNullVoterLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocationViewModel = new VoterLocationViewModel(),
                PollingLocationViewModel = GetValidPollingLocation(),
                CountyLocationViewModel = GetValidCountyLocation(),
                StateLocationViewModel = GetValidStateLocation()
            };
        }


        private static SideBarViewModel GetSideBarWithNullPollingLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocationViewModel = GetValidVoterLocation(),
                PollingLocationViewModel = new PollingLocationViewModel(),
                CountyLocationViewModel = GetValidCountyLocation(),
                StateLocationViewModel = GetValidStateLocation()
            };
        }


        private static SideBarViewModel GetSideBarWithNullCountyLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocationViewModel = GetValidVoterLocation(),
                PollingLocationViewModel = GetValidPollingLocation(),
                CountyLocationViewModel = new CountyLocationViewModel(),
                StateLocationViewModel = GetValidStateLocation()
            };
        }


        private static SideBarViewModel GetSideBarWithNullStateLocation()
        {
            return new SideBarViewModel()
            {
                VoterLocationViewModel = GetValidVoterLocation(),
                PollingLocationViewModel = GetValidPollingLocation(),
                CountyLocationViewModel = GetValidCountyLocation(),
                StateLocationViewModel = new StateLocationViewModel()
            };
        }


        private static SideBarViewModel GetSideBarWithAllNullLocations()
        {
            return new SideBarViewModel()
            {
                VoterLocationViewModel = new VoterLocationViewModel(),
                PollingLocationViewModel = new PollingLocationViewModel(),
                CountyLocationViewModel = new CountyLocationViewModel(),
                StateLocationViewModel = new StateLocationViewModel()
            };
        }



        private static VoterLocationViewModel GetValidVoterLocation()
        {
            return new VoterLocationViewModel()
            {
                Status = "Display",
                StreetAddress = "9282 Gregg Drive",
                City = "West Chester Township",
                StateAbbreviation = "OH",
                ZipCode = "45069"
            };
        }



        private static PollingLocationViewModel GetValidPollingLocation()
        {
            return new PollingLocationViewModel()
            {
                Status = "Display",
                StreetAddress = "9113 CINCINNATI DAYTON RD",
                City = "WEST CHESTER",
                StateAbbreviation = "OH",
                ZipCode = "45069"
            };
        }



        private static CountyLocationViewModel GetValidCountyLocation()
        {
            return new CountyLocationViewModel()
            {
                Status = "Display",
                StreetAddress = "1802 Princeton Rd., Ste 600",
                City = "Hamilton",
                StateAbbreviation = "OH",
                ZipCode = "45011"
            };
        }



        private static StateLocationViewModel GetValidStateLocation()
        {
            return new StateLocationViewModel()
            {
                Status = "Display",
                LocationName = "OHIO SECRETARY OF STATE",
                StreetAddress = "180 E. BROAD ST., 15TH FLOOR",
                City = "COLUMBUS",
                StateAbbreviation = "OH",
                ZipCode = "43215",
            };
        }

    }
}
