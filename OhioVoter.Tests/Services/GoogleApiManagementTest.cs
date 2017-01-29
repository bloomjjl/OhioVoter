using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhioVoter.Services;
using OhioVoter.ViewModels.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhioVoter.Tests.Services
{
    [TestClass]
    public class GoogleApiManagementTest
    {
        private static string _googleApiKey = "AIzaSyCmAeCrJzlOqmOlPx0q-MuZYZZnOMVfgXU";


        [TestMethod]
        public void Test_ValidateCountyLocation_FromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location countyLocation = GetValidCountyLocation();

            // Act
            ViewModels.Location result = service.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual("Display", result.Status);
            Assert.AreEqual(string.Empty, result.Message);
        }



        [TestMethod]
        public void Test_ValidateCountyLocation_NotFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location countyLocation = GetValidCountyLocation();
            countyLocation.StateAbbreviation = "AR";

            // Act
            ViewModels.Location result = service.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual("Update", result.Status);
            Assert.AreEqual("County address must be in Ohio.", result.Message);
        }



        [TestMethod]
        public void Test_ValidateCountyLocation_NullState()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location countyLocation = GetValidCountyLocation();
            countyLocation.StateAbbreviation = null;

            // Act
            ViewModels.Location result = service.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual("Update", result.Status);
            Assert.AreEqual("County address not found.", result.Message);
        }



        [TestMethod]
        public void Test_ValidateCountyLocation_NullCity()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location countyLocation = GetValidCountyLocation();
            countyLocation.City = null;

            // Act
            ViewModels.Location result = service.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual("Update", result.Status);
            Assert.AreEqual("County address not found.", result.Message);
        }



        [TestMethod]
        public void Test_ValidateCountyLocation_NullCityAndState()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location countyLocation = new ViewModels.Location();

            // Act
            ViewModels.Location result = service.ValidateCountyLocation(countyLocation);

            // Assert
            Assert.AreEqual("Update", result.Status);
            Assert.AreEqual("County address not found.", result.Message);
        }



        [TestMethod]
        public void Test_GetGoogleMapAPIRequestForVoterAndPollingLocation_ValidVoterAndPollingLocations()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location voterLocation = GetValidVoterLocation();
            ViewModels.Location pollingLocation = GetValidPollingLocation();

            // Act
            string result = service.GetGoogleMapAPIRequestForVoterAndPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.AreEqual(string.Concat("https://maps.googleapis.com/maps/api/staticmap?center", voterLocation.FullAddress.ToString(),
                                 "&size=300x300&maptype=roadmap&markers=color:red%7Clabel:H%7C", voterLocation.FullAddress.ToString(),
                                 "&markers=color:blue%7Clabel:P%7C", pollingLocation.FullAddress.ToString(),
                                 "&key=", _googleApiKey), result);
        }



        [TestMethod]
        public void Test_GetGoogleMapAPIRequestForVoterAndPollingLocation_NullVoterLocation()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location voterLocation = new ViewModels.Location();
            ViewModels.Location pollingLocation = GetValidPollingLocation();

            // Act
            string result = service.GetGoogleMapAPIRequestForVoterAndPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.AreEqual(string.Concat("https://maps.googleapis.com/maps/api/staticmap?center", voterLocation.FullAddress.ToString(),
                                 "&size=300x300&maptype=roadmap&markers=color:red%7Clabel:H%7C", voterLocation.FullAddress.ToString(),
                                 "&markers=color:blue%7Clabel:P%7C", pollingLocation.FullAddress.ToString(),
                                 "&key=", _googleApiKey), result);
        }



        [TestMethod]
        public void Test_GetGoogleMapAPIRequestForVoterAndPollingLocation_NullPollingLocation()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location voterLocation = GetValidVoterLocation();
            ViewModels.Location pollingLocation = new ViewModels.Location();

            // Act
            string result = service.GetGoogleMapAPIRequestForVoterAndPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.AreEqual(string.Concat("https://maps.googleapis.com/maps/api/staticmap?center", voterLocation.FullAddress.ToString(),
                                 "&size=300x300&maptype=roadmap&markers=color:red%7Clabel:H%7C", voterLocation.FullAddress.ToString(),
                                 "&markers=color:blue%7Clabel:P%7C", pollingLocation.FullAddress.ToString(),
                                 "&key=", _googleApiKey), result);
        }



        [TestMethod]
        public void Test_GetGoogleMapAPIRequestForVoterAndPollingLocation_NullVoterAndPollingLocations()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            ViewModels.Location voterLocation = new ViewModels.Location();
            ViewModels.Location pollingLocation = new ViewModels.Location();

            // Act
            string result = service.GetGoogleMapAPIRequestForVoterAndPollingLocation(voterLocation, pollingLocation);

            // Assert
            Assert.AreEqual(string.Concat("https://maps.googleapis.com/maps/api/staticmap?center", voterLocation.FullAddress.ToString(),
                                 "&size=300x300&maptype=roadmap&markers=color:red%7Clabel:H%7C", voterLocation.FullAddress.ToString(),
                                 "&markers=color:blue%7Clabel:P%7C", pollingLocation.FullAddress.ToString(),
                                 "&key=", _googleApiKey), result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedZipCode_ZipCodeFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string zipCode = "45030";

            // Act
            string result = service.GetStateAbbreviationForSuppliedZipCode(zipCode) as string;

            // Assert
            Assert.AreEqual("OH", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedZipCode_ZipCodeNotFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string zipCode = "77379";

            // Act
            string result = service.GetStateAbbreviationForSuppliedZipCode(zipCode) as string;

            // Assert
            Assert.AreEqual("TX", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedZipCode_ZipCodeWithNullValue()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string zipCode = null;

            // Act
            string result = service.GetStateAbbreviationForSuppliedZipCode(zipCode) as string;

            // Assert
            Assert.AreEqual("NA", result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromZipCode_ZipCodeFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string zipCode = "45030";

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromZipCode(zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&address=", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromZipCode_ZipCodeNotFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string zipCode = "77379";

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromZipCode(zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&address=", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromZipCode_ZipCodeWithNullValue()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string zipCode = null;

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromZipCode(zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&address=", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedStreetAddressAndZipCode_StreetAddressAndZipCodeFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "9282 Gregg Dr";
            string zipCode = "45069";

            // Act
            string result = service.GetStateAbbreviationForSuppliedStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual("OH", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedStreetAddressAndZipCode_StreetAddressWithDirectionAndZipCodeFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "2101 W 8th st";
            string zipCode = "45204";

            // Act
            string result = service.GetStateAbbreviationForSuppliedStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual("OH", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedStreetAddressAndZipCode_StreetAddressFromOhioAndZipCodeNotFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "9282 Gregg Dr";
            string zipCode = "46001";

            // Act
            string result = service.GetStateAbbreviationForSuppliedStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual("NA", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedStreetAddressAndZipCode_StreetAddressNotFromOhioAndZipCodeFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "9281 Gregg Dr";
            string zipCode = "43130";

            // Act
            string result = service.GetStateAbbreviationForSuppliedStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual("NA", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationForSuppliedStreetAddressAndZipCode_StreetAddressNotFromOhioAndZipCodeNotFromOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "316 W 12th ST";
            string zipCode = "65210";

            // Act
            string result = service.GetStateAbbreviationForSuppliedStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual("NA", result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode_ValidStreetAddressAndZipCode()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "316 W 12th ST";
            string zipCode = "65210";

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&", "address=", streetAddress, " ", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode_BlankStreetAddress()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "";
            string zipCode = "65210";

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&", "address=", streetAddress, " ", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode_BlankZipCode()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "316 W 12th ST";
            string zipCode = "";

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&", "address=", streetAddress, " ", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode_BlankStreetAddressAndZipCode()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "";
            string zipCode = "";

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&", "address=", streetAddress, " ", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode_NullStreetAddress()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = null;
            string zipCode = "65210";

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&", "address=", streetAddress, " ", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode_NullZipCode()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = "316 W 12th ST";
            string zipCode = null;

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&", "address=", streetAddress, " ", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode_NullStreetAddressAndZipCode()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            string streetAddress = null;
            string zipCode = null;

            // Act
            string result = service.GetUrlRequestForStateAbbreviationFromStreetAddressAndZipCode(streetAddress, zipCode) as string;

            // Assert
            Assert.AreEqual(string.Concat("http://maps.googleapis.com/maps/api/geocode/json?&", "address=", streetAddress, " ", zipCode, "&sensor=true"), result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationFromGoogleAddressObject_StateEqualsOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            AddressObject addressObject = GetAddressObjectForSuppliedState("OH", "administrative_area_level_1");

            // Act
            string result = service.GetStateAbbreviationFromGoogleAddressObject(addressObject) as string;

            // Assert
            Assert.AreEqual("OH", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationFromGoogleAddressObject_StateNotOhio()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            AddressObject addressObject = GetAddressObjectForSuppliedState("AL", "administrative_area_level_1");

            // Act
            string result = service.GetStateAbbreviationFromGoogleAddressObject(addressObject) as string;

            // Assert
            Assert.AreEqual("AL", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationFromGoogleAddressObject_StateNull()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            AddressObject addressObject = GetAddressObjectForSuppliedState(null, "administrative_area_level_1");

            // Act
            string result = service.GetStateAbbreviationFromGoogleAddressObject(addressObject) as string;

            // Assert
            Assert.AreEqual("NA", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationFromGoogleAddressObject_AddressComponentTypeIsNull()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            AddressObject addressObject = GetAddressObjectForSuppliedState("AL", null);

            // Act
            string result = service.GetStateAbbreviationFromGoogleAddressObject(addressObject) as string;

            // Assert
            Assert.AreEqual("NA", result);
        }



        [TestMethod]
        public void Test_GetStateAbbreviationFromGoogleAddressObject_StateAndAddressComponentTypeAreNull()
        {
            // Arrange
            GoogleApiManagement service = new GoogleApiManagement();
            AddressObject addressObject = GetAddressObjectForSuppliedState(null, null);

            // Act
            string result = service.GetStateAbbreviationFromGoogleAddressObject(addressObject) as string;

            // Assert
            Assert.AreEqual("NA", result);
        }






        // ***********************************************************


        private AddressObject GetAddressObjectForSuppliedState(string stateAbbreviation, string addressComponentType)
        {
            List<string> typesOther = new List<string>();
            typesOther.Add("Text1");
            typesOther.Add("Text2");

            AddressComponent addressComponentOther = new AddressComponent()
            {
                short_name = "NA",
                types = typesOther
            };

            List<string> typesState = new List<string>();
            typesState.Add("Text1");
            typesState.Add(addressComponentType);
            typesState.Add("Text2");

            AddressComponent addressComponentState = new AddressComponent()
            {
                short_name = stateAbbreviation,
                types = typesState
            };

            List<AddressComponent> addressComponents = new List<AddressComponent>();
            addressComponents.Add(addressComponentOther);
            addressComponents.Add(addressComponentOther);
            addressComponents.Add(addressComponentOther);
            addressComponents.Add(addressComponentOther);
            addressComponents.Add(addressComponentOther);
            addressComponents.Add(addressComponentOther);
            addressComponents.Add(addressComponentState);
            addressComponents.Add(addressComponentOther);

            Result result = new Result()
            {
                address_components = addressComponents
            };

            List<Result> addressResults = new List<Result>();
            addressResults.Add(result);

            AddressObject addressObject = new AddressObject()
            {
                results = addressResults
            };

            return addressObject;
        }




        // **************************************************************


        private static ViewModels.Location GetValidVoterLocation()
        {
            return new ViewModels.Location()
            {
                Status = "Display",
                StreetAddress = "9282 Gregg Drive",
                City = "West Chester Township",
                StateAbbreviation = "OH",
                ZipCode = "45069"
            };
        }



        private static ViewModels.Location GetValidPollingLocation()
        {
            return new ViewModels.Location()
            {
                Status = "Display",
                StreetAddress = "9113 CINCINNATI DAYTON RD",
                City = "WEST CHESTER",
                StateAbbreviation = "OH",
                ZipCode = "45069"
            };
        }


        private static ViewModels.Location GetValidCountyLocation()
        {
            return new ViewModels.Location()
            {
                Status = "Display",
                StreetAddress = "1802 Princeton Rd., Ste 600",
                City = "Hamilton",
                StateAbbreviation = "OH",
                ZipCode = "45011"
            };
        }



        private static ViewModels.Location GetValidStateLocation()
        {
            return new ViewModels.Location()
            {
                Status = "Display",
                StateAbbreviation = "OH"
            };
        }


    }
}
