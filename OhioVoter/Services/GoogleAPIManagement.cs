using Newtonsoft.Json.Linq;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace OhioVoter.Services
{
    public class GoogleApiManagement
    {



        // **************************
        // Google Civic Info API
        // **************************


        
        public ViewModels.Location.SideBarViewModel GetGoogleCivicRepresentativesForVoterLocation(ViewModels.Location.VoterLocationViewModel voterLocation)
        {
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/

                string urlRequest = GetUrlRequestForRepresentativesFromVoterLocation(voterLocation);
                WebClient client = new WebClient();
                string json = client.DownloadString(urlRequest);
                CivicInfoRootObject googleResult = JToken.Parse(json).ToObject<CivicInfoRootObject>();

                ViewModels.Location.SideBarViewModel sideBar = GetPollingLocationAndCountyLocationFromGoogleResult(googleResult);
                sideBar.VoterLocationViewModel = voterLocation;
                //sideBar.PollingLocation = ValidatePollingLocation(sideBar.PollingLocation);
                //sideBar.CountyLocation = ValidateCountyLocation(sideBar.CountyLocation);

                return sideBar;
            }
            catch (Exception e)
            {// return empty object if bad address supplied
                if (voterLocation == null)
                    voterLocation = new ViewModels.Location.VoterLocationViewModel();

                voterLocation.Status = "Update";
                voterLocation.Message = "Voter address is not valid.";
                return GetEmptyVoterLocationViewModel(voterLocation);
            }
        }



        /// <summary>
        /// validate voter information supplied is valid
        /// then return voting location information
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        public ViewModels.Location.SideBarViewModel GetGoogleCivicInformationForVoterLocation(ViewModels.Location.VoterLocationViewModel voterLocation)
        {
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/

                string urlRequest = GetUrlRequestForGoogleCivicInformationFromVoterLocation(voterLocation);
                WebClient client = new WebClient();
                string json = client.DownloadString(urlRequest);
                CivicInfoRootObject googleResult = JToken.Parse(json).ToObject<CivicInfoRootObject>();

                ViewModels.Location.SideBarViewModel sideBar = GetPollingLocationAndCountyLocationFromGoogleResult(googleResult);
                sideBar.VoterLocationViewModel = voterLocation;
                //sideBar.PollingLocation = ValidatePollingLocation(sideBar.PollingLocation);
                //sideBar.CountyLocation = ValidateCountyLocation(sideBar.CountyLocation);

                return sideBar;
            }
            catch (Exception e)
            {// return empty object if bad address supplied
                if (voterLocation == null)
                    voterLocation = new ViewModels.Location.VoterLocationViewModel();

                voterLocation.Status = "Update";
                voterLocation.Message = "Voter address is not valid.";
                return GetEmptyVoterLocationViewModel(voterLocation);
            }
        }



        /// <summary>
        /// create empty location objects to return
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        private ViewModels.Location.SideBarViewModel GetEmptyVoterLocationViewModel(ViewModels.Location.VoterLocationViewModel voterLocation)
        {
            return new ViewModels.Location.SideBarViewModel()
            {
                VoterLocationViewModel = voterLocation,
                PollingLocationViewModel = new ViewModels.Location.PollingLocationViewModel(),
                CountyLocationViewModel = new ViewModels.Location.CountyLocationViewModel(),
                StateLocationViewModel = new ViewModels.Location.StateLocationViewModel()
            };
        }



        /// <summary>
        /// get location information for SideBar
        /// </summary>
        /// <param name="googleResult"></param>
        /// <returns></returns>
        private ViewModels.Location.SideBarViewModel GetSideBarViewModel(CivicInfoRootObject googleResult)
        {
            return new ViewModels.Location.SideBarViewModel()
            {
                VoterLocationViewModel = GetVoterLocationFromGoogleCivicInformation(googleResult),
                PollingLocationViewModel = GetPollingLocationFromGoogleCivicInformation(googleResult),
                CountyLocationViewModel = GetCountyLocationFromGoogleCivicInformation(googleResult)
            };
        }




        public ViewModels.Location.SideBarViewModel GetPollingLocationAndCountyLocationFromGoogleResult (CivicInfoRootObject googleResult)
        {
            return new ViewModels.Location.SideBarViewModel()
            {
                PollingLocationViewModel = GetPollingLocationFromGoogleCivicInformation(googleResult),
                CountyLocationViewModel = GetCountyLocationFromGoogleCivicInformation(googleResult)
            };

        }




        /// <summary>
        /// get voting information from API for supplied voter location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        public string GetUrlRequestForGoogleCivicInformationFromVoterLocation(ViewModels.Location.VoterLocationViewModel voterLocation)
        {
            string api = "https://www.googleapis.com/civicinfo/v2/voterinfo?";
            string key = _googleApiKey;
            string andChar = "&";
            string electionIdValue = "2000"; // this value may need to be adjusted
            string voterAddress = voterLocation.FullAddress;
/*            string voterAddress = voterLocation.ZipCode;
            if (voterLocation.ZipCodeSuffix != null && voterLocation.ZipCodeSuffix != "")
            {
                voterAddress = voterAddress + "-" + voterLocation.ZipCodeSuffix;
            }
*/
            return string.Concat(api, "address=", voterAddress,
                                 andChar, "electionId=", electionIdValue,
                                 andChar, "key=", key);
        }




        public string GetUrlRequestForRepresentativesFromVoterLocation(ViewModels.Location.VoterLocationViewModel voterLocation)
        {
            string api = "https://www.googleapis.com/civicinfo/v2/representatives?";
            string key = _googleApiKey;
            string andChar = "&";
            string voterAddress = string.Format("{0} {1}", voterLocation.StreetAddress, voterLocation.ZipCode);

            return string.Concat(api, "address=", voterAddress,
                                 andChar, "key=", key);
        }



        /// <summary>
        /// separate and store the voter location information
        /// </summary>
        /// <param name="googleResult"></param>
        /// <returns></returns>
        public ViewModels.Location.VoterLocationViewModel GetVoterLocationFromGoogleCivicInformation(CivicInfoRootObject googleResult)
        {
            return new ViewModels.Location.VoterLocationViewModel()
            {
                StreetAddress = googleResult.normalizedInput.line1.ToString(),
                City = googleResult.normalizedInput.city.ToString(),
                StateAbbreviation = googleResult.normalizedInput.state.ToString(),
                ZipCode = googleResult.normalizedInput.zip.ToString()
            };
        }



        /// <summary>
        /// separate and store the polling location information
        /// </summary>
        /// <param name="googleResult"></param>
        /// <returns></returns>
        public ViewModels.Location.PollingLocationViewModel GetPollingLocationFromGoogleCivicInformation(CivicInfoRootObject googleResult)
        {
            try
            {
                return new ViewModels.Location.PollingLocationViewModel()
                {
                    LocationName = googleResult.pollingLocations[0].address.locationName.ToString(),
                    StreetAddress = googleResult.pollingLocations[0].address.line1.ToString(),
                    City = googleResult.pollingLocations[0].address.city.ToString(),
                    StateAbbreviation = googleResult.pollingLocations[0].address.state.ToString(),
                    ZipCode = googleResult.pollingLocations[0].address.zip.ToString()
                };
            }
            catch
            {
                return new ViewModels.Location.PollingLocationViewModel()
                {
                    Status = "Update",
                    Message = "Polling address not found."
                };
            }

        }



        /// <summary>
        /// separate and store the county location information
        /// </summary>
        /// <param name="googleResult"></param>
        /// <returns></returns>
        public ViewModels.Location.CountyLocationViewModel GetCountyLocationFromGoogleCivicInformation(CivicInfoRootObject googleResult)
        {
            try
            { 
                return new ViewModels.Location.CountyLocationViewModel()
                {
                    LocationName = googleResult.state[0].local_jurisdiction.name.ToString(),
                    StreetAddress = googleResult.state[0].local_jurisdiction.electionAdministrationBody.physicalAddress.line1.ToString(),
                    City = googleResult.state[0].local_jurisdiction.electionAdministrationBody.physicalAddress.city.ToString(),
                    StateAbbreviation = googleResult.state[0].local_jurisdiction.electionAdministrationBody.physicalAddress.state.ToString(),
                    ZipCode = googleResult.state[0].local_jurisdiction.electionAdministrationBody.physicalAddress.zip.ToString(),
                    Phone = googleResult.state[0].local_jurisdiction.electionAdministrationBody.electionOfficials[0].officePhoneNumber.ToString(),
                    Email = googleResult.state[0].local_jurisdiction.electionAdministrationBody.electionOfficials[0].emailAddress.ToString(),
                    Website = googleResult.state[0].local_jurisdiction.electionAdministrationBody.electionInfoUrl.ToString()
                };
            }
            catch
            {
                return new ViewModels.Location.CountyLocationViewModel()
                {
                    Status = "Update",
                    Message = "County address not found."
                };
            }
        }



        /// <summary>
        /// make sure valid city is found in Ohio
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        public ViewModels.Location.VoterLocationViewModel ValidateSuppliedVoterLocation(ViewModels.Location.VoterLocationViewModel voterLocation)
        {
            if (voterLocation.City == null || voterLocation.City == "")
            {
                // city must be found
                voterLocation.Status = "Update";
                voterLocation.Message = "Voter address not found.";
            }
            else if (voterLocation.StateAbbreviation != "OH")
            {
                // state can only be OHIO
                voterLocation.Status = "Update";
                voterLocation.Message = "Voter address must be in Ohio.";
            }
            else
            {
                voterLocation.Status = "Display";
                voterLocation.Message = string.Empty;
            }

            return voterLocation;
        }



        /// <summary>
        /// make sure valid polling location is found in Ohio
        /// </summary>
        /// <param name="pollingLocation"></param>
        /// <returns></returns>
        public ViewModels.Location.PollingLocationViewModel ValidatePollingLocation(ViewModels.Location.PollingLocationViewModel pollingLocation)
        {
            if (pollingLocation.City == null || pollingLocation.City == "")
            {
                // city must be found
                pollingLocation.Status = "Update";
                pollingLocation.Message = "Polling address not found.";
            }
            else if (pollingLocation.StateAbbreviation != "OH")
            {
                // state can only be OHIO
                pollingLocation.Status = "Update";
                pollingLocation.Message = "Polling address must be in Ohio.";
            }
            else
            {
                pollingLocation.Status = "Display";
                pollingLocation.Message = string.Empty;
            }

            return pollingLocation;
        }




        public ViewModels.Location.CountyLocationViewModel ValidateCountyLocation(ViewModels.Location.CountyLocationViewModel countyLocation)
        {// Tests Generated
            if (countyLocation.StateAbbreviation == null || countyLocation.City == null || countyLocation.City == "")
            {
                // city must be found
                countyLocation.Status = "Update";
                countyLocation.Message = "County address not found.";
            }
            else if (countyLocation.StateAbbreviation != "OH")
            {
                // state can only be OHIO
                countyLocation.Status = "Update";
                countyLocation.Message = "County address must be in Ohio.";
            }
            else
            {
                countyLocation.Status = "Display";
                countyLocation.Message = string.Empty;
            }

            return countyLocation;
        }




        // **************************
        // Google Map API
        // **************************

        /// <summary>
        /// get the map information to display the voter location and polling location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <param name="pollingLocation"></param>
        /// <returns></returns>
        public string GetGoogleMapAPIRequestForVoterAndPollingLocation(ViewModels.Location.VoterLocationViewModel voterLocation, ViewModels.Location.PollingLocationViewModel pollingLocation)
        {// Tests Generated
            string api = "https://maps.googleapis.com/maps/api/staticmap?";
            string key = _googleApiKey;
            string andChar = "&";

            return string.Concat(api, "center", voterLocation.FullAddress.ToString(),
                                 andChar, "size=300x300",
                                 andChar, "maptype=roadmap",
                                 andChar, "markers=color:red%7Clabel:H%7C", voterLocation.FullAddress.ToString(),
                                 andChar, "markers=color:blue%7Clabel:P%7C", pollingLocation.FullAddress.ToString(),
                                 andChar, "key=", key);
        }





        // ****************************
        // Google Zip Code API
        // ****************************

        /// <summary>
        /// use google maps to get the state abbreviation based on the zip code
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public string GetStateAbbreviationForSuppliedZipCode(string zipCode)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/

                string urlRequest = GetUrlRequestForStateAbbreviationFromZipCode(zipCode);
                WebClient client = new WebClient();
                string urlResponse = client.DownloadString(urlRequest);
                AddressObject googleResult = JToken.Parse(urlResponse).ToObject<AddressObject>();

                return GetStateAbbreviationFromGoogleAddressObject(googleResult);
            }
            catch (Exception e)
            {// bad address supplied
                return "NA";
            }
        }



        /// <summary>
        /// get the api for a location based on the zip code
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public string GetUrlRequestForStateAbbreviationFromZipCode(string zipCode)
        {// Tests Generated
            string api = "http://maps.googleapis.com/maps/api/geocode/json?";
            string andChar = "&";
            string address = "address=";
            string sensor = "sensor=true";

            return string.Concat(api, 
                                 andChar, address, zipCode,
                                 andChar, sensor);
        }






        // ***********************************************
        // Google Street Address And Zip Code API
        // ***********************************************



        /// <summary>
        /// Look up the state based on the street address and zip code
        /// </summary>
        /// <param name="streetAddress"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public string GetStateAbbreviationForSuppliedStreetAddressAndZipCode(string streetAddress, string zipCode)
        {// Tests Generated
            try
            {
                // create C# classes from json file
                // http://json2csharp.com/

                string urlRequest = GetUrlRequestForFullAddressFromStreetAddressAndZipCode(streetAddress, zipCode);
                WebClient client = new WebClient();
                string urlResponse = client.DownloadString(urlRequest);
                AddressObject googleResult = JToken.Parse(urlResponse).ToObject<AddressObject>();

                return GetStateAbbreviationFromGoogleAddressObject(googleResult);
            }
            catch (Exception e)
            {// bad address supplied
                return "NA";
            }
        }



        /// <summary>
        /// get all location information provided from google for supplied street address and zip code
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public ViewModels.Location.VoterLocationViewModel GetAllLocationInformationForSuppliedAddress(ViewModels.Location.VoterLocationViewModel location)
        {
            // create C# classes from json file
            // http://json2csharp.com/

            string urlRequest = GetUrlRequestForFullAddressFromStreetAddressAndZipCode(location.StreetAddress, location.ZipCode);
            WebClient client = new WebClient();
            string urlResponse = client.DownloadString(urlRequest);
            AddressObject googleResult = JToken.Parse(urlResponse).ToObject<AddressObject>();

            return GetAllLocationInformationFromGoogleAddressObject(googleResult);
        }



        /// <summary>
        /// get the api for a location based on the street address and zip code
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public string GetUrlRequestForFullAddressFromStreetAddressAndZipCode(string streetAddress, string zipCode)
        {// Tests Generated
            string api = "http://maps.googleapis.com/maps/api/geocode/json?";
            string andChar = "&";
            string address = "address=";
            string sensor = "sensor=true";

            return string.Concat(api,
                                 andChar, address, streetAddress, " ", zipCode,
                                 andChar, sensor);
        }



        /// <summary>
        /// get the api for a location based on the zip code
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public string GetUrlRequestForFullAddressFromZipCode(string zipCode)
        { 
            string api = "http://maps.googleapis.com/maps/api/geocode/json?";
            string andChar = "&";
            string address = "address=";
            string sensor = "sensor=true";

            return string.Concat(api,
                                 andChar, address, zipCode,
                                 andChar, sensor);
        }



        /// <summary>
        /// get the state abbreviation from the googleResult
        /// </summary>
        /// <param name="googleResult"></param>
        /// <returns></returns>
        public string GetStateAbbreviationFromGoogleAddressObject(AddressObject googleResult)
        {// Tests Generated
            if (googleResult.results[0].partial_match == true)
                return "NA";

            for(int i = 0; i < googleResult.results[0].address_components.Count; i++)
            {
                if (googleResult.results[0].address_components[i].short_name != null &&
                    googleResult.results[0].address_components[i].short_name != "")
                {
                    for (int j = 0; j < googleResult.results[0].address_components[i].types.Count; j++)
                    {
                        if (googleResult.results[0].address_components[i].types[j] != null &&
                            googleResult.results[0].address_components[i].types[j] == "administrative_area_level_1")
                        {
                            return googleResult.results[0].address_components[i].short_name.ToString();
                        }
                    }
                }
            }

            return "NA";
        }



        public ViewModels.Location.VoterLocationViewModel GetAllLocationInformationFromGoogleAddressObject(AddressObject googleResult)
        {
            ViewModels.Location.VoterLocationViewModel location = new ViewModels.Location.VoterLocationViewModel();
            string streetNumber = "";
            string streetName = "";

            // make sure address found
            if (googleResult.results.Count == 0 || googleResult.results == null)
                return location;

            // copy location information to ViewModel
            for (int i = 0; i < googleResult.results[0].address_components.Count; i++)
            {
                string type = googleResult.results[0].address_components[i].types[0].ToString();
                        
                switch (type)
                {
                    case null:
                        break;
                    case "street_number":
                        streetNumber = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    case "route":
                        streetName = googleResult.results[0].address_components[i].short_name.ToString();
                        break;
                    case "neighborhood":
                        location.Neighborhood = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    case "locality":
                        location.City = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    case "administrative_area_level_3":
                        location.City = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    case "administrative_area_level_2":
                        location.County = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    case "administrative_area_level_1":
                        location.StateAbbreviation = googleResult.results[0].address_components[i].short_name.ToString();
                        break;
                    case "country":
                        location.Country = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    case "postal_code":
                        location.ZipCode = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    case "postal_code_suffix":
                        location.ZipCodeSuffix = googleResult.results[0].address_components[i].long_name.ToString();
                        break;
                    default:
                        break;
                }
            }

            location.StreetAddress = string.Concat(streetNumber, " ", streetName);

            return location;
        }



    }
}