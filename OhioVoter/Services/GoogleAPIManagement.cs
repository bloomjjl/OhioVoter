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
        private static string _googleApiKey = "AIzaSyCmAeCrJzlOqmOlPx0q-MuZYZZnOMVfgXU";



        // **************************
        // Google Civic Info API
        // **************************

        public SideBar GetGoogleCivicInformationForVoterLocation(Location voterLocation)
        {
            try
            {
                string urlRequest = GetGoogleCivicInformationAPIRequestForVoterPollingLocation(voterLocation);

                // create C# classes from json file
                // http://json2csharp.com/

                WebClient client = new WebClient();
                string json = client.DownloadString(urlRequest);
                RootObject googleResult = JToken.Parse(json).ToObject<RootObject>();

                SideBar sideBarViewModel = new SideBar()
                {
                    VoterLocation = GetVoterLocationFromGoogleCivicInformation(googleResult),
                    PollingLocation = GetPollingLocationFromGoogleCivicInformation(googleResult),
                    CountyLocation = GetCountyLocationFromGoogleCivicInformation(googleResult)
                };

                sideBarViewModel.VoterLocation = ValidateSuppliedVoterLocation(sideBarViewModel.VoterLocation);
                sideBarViewModel.PollingLocation = ValidatePollingLocation(sideBarViewModel.PollingLocation);

                return sideBarViewModel;
            }
            catch
            {// return empty object if bad address supplied
                SideBar sideBarViewModel = new SideBar()
                {
                    VoterLocation = voterLocation,
                    PollingLocation = new Location(),
                    CountyLocation = new Location(),
                    StateLocation = new Location()
                };

                sideBarViewModel.VoterLocation.Message = "Address is not valid.";

                return sideBarViewModel;
            }
        }



        public string GetGoogleCivicInformationAPIRequestForVoterPollingLocation(Location voterLocation)
        {
            string api = "https://www.googleapis.com/civicinfo/v2/voterinfo?";
            string key = _googleApiKey;
            string andChar = "&";
            string electionIdValue = "2000"; // this value may need to be adjusted
            string voterAddress = voterLocation.FullAddress;

            return api + "address=" + voterAddress +
                   andChar + "electionId=" + electionIdValue +
                   andChar + "key=" + key;
        }




        public Location GetVoterLocationFromGoogleCivicInformation(RootObject googleResult)
        {
            Location voterLocation = new Location()
            {
                StreetAddress = googleResult.normalizedInput.line1.ToString(),
                City = googleResult.normalizedInput.city.ToString(),
                StateAbbreviation = googleResult.normalizedInput.state.ToString(),
                ZipCode = googleResult.normalizedInput.zip.ToString()
            };

            return voterLocation;
        }




        public Location GetPollingLocationFromGoogleCivicInformation(RootObject googleResult)
        {
            Location pollingLocation = new Location()
            {
                LocationName = googleResult.pollingLocations[0].address.locationName.ToString(),
                StreetAddress = googleResult.pollingLocations[0].address.line1.ToString(),
                City = googleResult.pollingLocations[0].address.city.ToString(),
                StateAbbreviation = googleResult.pollingLocations[0].address.state.ToString(),
                ZipCode = googleResult.pollingLocations[0].address.zip.ToString()
            };

            return pollingLocation;
        }



        public Location GetCountyLocationFromGoogleCivicInformation(RootObject googleResult)
        {
            Location countyLocation = new Location()
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

            return countyLocation;
        }



        public Location ValidateSuppliedVoterLocation(Location voterLocation)
        {
            if (voterLocation.City == null || voterLocation.City == "")
            {
                // city must be found
                voterLocation.Status = "Update";
                voterLocation.Message = "Address Supplied is not valid.";
            }
            else if (voterLocation.StateAbbreviation != "OH")
            {
                // state can only be OHIO
                voterLocation.Status = "Update";
                voterLocation.Message = "Address must be in Ohio.";
            }
            else
            {
                voterLocation.Message = "";
            }

            return voterLocation;
        }



        public Location ValidatePollingLocation(Location pollingLocation)
        {
            if (pollingLocation.City == null || pollingLocation.City == "")
            {
                // city must be found
                pollingLocation.Status = "Update";
                pollingLocation.Message = "Address Supplied is not valid.";
            }
            else if (pollingLocation.StateAbbreviation != "OH")
            {
                // state can only be OHIO
                pollingLocation.Status = "Update";
                pollingLocation.Message = "Address must be in Ohio.";
            }
            else
            {
                pollingLocation.Message = "";
            }

            return pollingLocation;
        }






        // **************************
        // Google Map API
        // **************************

        public string GetGoogleMapAPIRequestForVoterAndPollingLocation(Location voterLocation, Location pollingLocation)
        {
            string api = "https://maps.googleapis.com/maps/api/staticmap?";
            string key = _googleApiKey;
            string andChar = "&";

            return api + "center" + voterLocation.FullAddress.ToString() +
                   andChar + "size=300x300" +
                   andChar + "maptype=roadmap" +
                   andChar + "markers=color:red%7Clabel:H%7C" + voterLocation.FullAddress.ToString() +
                   andChar + "markers=color:blue%7Clabel:P%7C" + pollingLocation.FullAddress.ToString() +
                   andChar + "key=" + key;
        }


    }
}