using OhioVoter.Services;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Controllers
{
    public class LocationController : Controller
    {
        // ********************************************
        // Sidebar information for supplied address
        // ********************************************

        // TODO: fix error validation to display errors and messages when Location form submitted


        /// <summary>
        /// update voter location information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayVoterLocationSideBar(SideBarViewModel sideBar)
        {// Tests Generated
            if (ValidateLocation(sideBar.VoterLocation))
            {
                if(ValidateLocation(sideBar.PollingLocation))
                {
                    return PartialView("_VoterLocation", sideBar);
                }
                else
                {
                    sideBar.PollingLocation.StreetAddress = "Location not found.";
                    return PartialView("_VoterLocation", sideBar);
                }
            }
            else
            {
                return PartialView("_VoterLocationForm", sideBar);
            }
        }



        /// <summary>
        /// update general information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayGeneralLocationSideBar(SideBarViewModel sideBar)
        {// Tests Generated
            return PartialView("_VoterGeneralInformation", sideBar);
        }



        /// <summary>
        /// get the voter location information from session to display in the sidebar
        /// </summary>
        /// <returns></returns>
        public SideBarViewModel GetSideBarViewModel(string controllerName)
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();

            SideBarViewModel sideBar = new SideBarViewModel()
            {
                ControllerName = controllerName,
                VoterLocation = session.GetVoterLocationFromSession(),
                PollingLocation = session.GetPollingLocationFromSession(),
                CountyLocation = session.GetCountyLocationFromSession(),
                StateLocation = GetAddressForOhioSecretaryOfState()
            };

            return sideBar;
        }



        /// <summary>
        /// Secretary of State will always be from Ohio
        /// store location information for Ohio SOS
        /// </summary>
        /// <returns></returns>
        public LocationViewModel GetAddressForOhioSecretaryOfState()
        {// Tests Generated
            return new LocationViewModel()
            {
                Status = "Display",
                LocationName = "OHIO SECRETARY OF STATE",
                StreetAddress = "180 E. BROAD ST., 15TH FLOOR",
                City = "COLUMBUS",
                StateAbbreviation = "OH",
                ZipCode = "43215",
                ZipCodeSuffix = "3726",
                Website = "http://www.sos.state.oh.us/"
            };
        }



        /// <summary>
        /// display information based on the voter location stored in the session
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(string controllerName)
        {// Tests not generated because of session testing null reference error
            UpdateSessionStatusToShowVoterLocationForm();
            return RedirectToAction("Index", controllerName);
        }



        /// <summary>
        /// update page based on the voter location provided from form
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(LocationViewModel voterLocation, string controllerName)
        {// Tests not generated because of session testing null reference error
            voterLocation = CheckFormFieldsAreValid(voterLocation);
            if (voterLocation.Status == "Update")
            {
                UpdateSessionInformationForVoterLocation(voterLocation);
                return RedirectToAction("Index", controllerName);
            }

            LocationViewModel completeVoterLocation = GetAllLocationInformationForSuppliedStreetAddressAndZipCode(voterLocation);

            // make sure google found a location matching the provided zip code
            if (voterLocation.ZipCode != completeVoterLocation.ZipCode)
            {
                voterLocation.Message = "Street address and/or zip code are not valid.";
                UpdateSessionInformationForVoterLocation(voterLocation);
                return RedirectToAction("Index", controllerName);
            }

            completeVoterLocation = CheckLocationIsValid(completeVoterLocation);

            // validate supplied street address and zip code are in OHIO
            if (completeVoterLocation.Status == "Update")
            {
                UpdateSessionInformationForVoterLocation(completeVoterLocation);
                return RedirectToAction("Index", controllerName);
            }

            SideBarViewModel sideBar = GetSideBarViewModelFromGoogleCivicInformationAPI(completeVoterLocation);
            sideBar.StateLocation = GetAddressForOhioSecretaryOfState();
            sideBar = CheckEachLocationInSideBarIsValid(sideBar);

            if (sideBar.VoterLocation.Status == "Display")
            {
                sideBar.PollingLocation.GoogleLocationMapAPI = GetGoogleMapForPollingLocation(sideBar.VoterLocation, sideBar.PollingLocation);
                UpdateSessionFromSideBarViewModel(sideBar);
                return RedirectToAction("Index", controllerName);
            }

            UpdateSessionStatusToShowVoterLocationForm();
            return RedirectToAction("Index", controllerName);
        }




        public bool ValidateVoterLocationInOhio(LocationViewModel voterLocation)
        {// Tests not generated because of session testing null reference error
            // street address and zip code must be provided
            if (string.IsNullOrWhiteSpace(voterLocation.StreetAddress) == true || string.IsNullOrWhiteSpace(voterLocation.ZipCode) == true)
            {
                voterLocation.Message = "Street Address and Zip Code must be provided";
                UpdateSessionInformationForVoterLocation(voterLocation);
                return false;
            }
            // validate street address and zip code is in OHIO
            if (ValidateStreetAddressAndZipCodeLocatedInOhio(voterLocation.StreetAddress.ToString(), voterLocation.ZipCode.ToString()) == false)
            {
                voterLocation.Message = "Address must be a valid location in Ohio";
                UpdateSessionInformationForVoterLocation(voterLocation);
                return false;
            }
            return true;
        }



        /// <summary>
        /// make sure zip code is in Ohio
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public bool ValidateZipCodeLocatedInOhio(string zipCode)
        {// Tests Generated
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetStateAbbreviationForSuppliedZipCode(zipCode) == "OH" ? true : false;
        }



        public LocationViewModel GetAllLocationInformationForSuppliedStreetAddressAndZipCode(LocationViewModel location)
        {
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetAllLocationInformationForSuppliedAddress(location);
        }


        /// <summary>
        /// make sure street address and zip code is in Ohio
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public bool ValidateStreetAddressAndZipCodeLocatedInOhio(string streetAddress, string zipCode)
        {// Tests Generated
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetStateAbbreviationForSuppliedStreetAddressAndZipCode(streetAddress, zipCode) == "OH" ? true : false;
        }




        /// <summary>
        /// make sure valid information is provided for all locations
        /// </summary>
        /// <param name="sideBarViewModel"></param>
        /// <returns></returns>
        public bool ValidateSideBarLocations(SideBarViewModel sideBar)
        {// Tests Generated
            return ValidateLocation(sideBar.VoterLocation) &&
                   ValidateLocation(sideBar.PollingLocation) &&
                   ValidateLocation(sideBar.CountyLocation) &&
                   ValidateLocation(sideBar.StateLocation) ?
                   true : false;
        }




        public SideBarViewModel CheckEachLocationInSideBarIsValid(SideBarViewModel sideBar)
        {
            sideBar.VoterLocation = CheckLocationIsValid(sideBar.VoterLocation);
            sideBar.PollingLocation = CheckLocationIsValid(sideBar.PollingLocation);
            sideBar.CountyLocation = CheckLocationIsValid(sideBar.CountyLocation);
            sideBar.StateLocation = CheckLocationIsValid(sideBar.StateLocation);

            return sideBar;
        }



        public LocationViewModel CheckLocationIsValid(LocationViewModel location)
        {
            location.Message = "";
            location.Status = "Display";

            // check street address and zip code
            location = CheckFormFieldsAreValid(location);
            if (location.Status == "Update")
                return location;

            // check state
            if (!ValidateStateIsOhio(location.StateAbbreviation))
            {
                location.Message = "Address must be in Ohio.";
                location.Status = "Update";
                return location;
            }

            // check city
            if (!ValidateCityIsFound(location.City))
            {
                location.Message = "Invalid address.";
                location.Status = "Update";
                return location;
            }

            return location;
        }


        public LocationViewModel CheckFormFieldsAreValid(LocationViewModel location)
        {
            location.Message = "";
            location.Status = "Display";

            // check street
            if (!ValidateStreetAddressIsFound(location.StreetAddress))
            {
                location.Message = "Street address must be provided.";
                location.Status = "Update";
                return location;
            }

            // check zipcode
            if (!ValidateZipCodeIsFound(location.ZipCode))
            {
                location.Message = "Zip code must be provided.";
                location.Status = "Update";
                return location;
            }
            else if (!ValidateZipCodeIsCorrectFormat(location.ZipCode))
            {
                location.Message = "Zip code must be 5 numbers.";
                location.Status = "Update";
                return location;
            }

            return location;
        }


        public bool ValidateStateIsOhio(string stateAbbreviation)
        {
            return stateAbbreviation == "OH" ? true : false;
        }


        public bool ValidateStreetAddressIsFound(string streetAddress )
        {
            return streetAddress == null || streetAddress == "" ? false : true;
        }


        public bool ValidateZipCodeIsFound(string zipCode)
        {
            return zipCode == null || zipCode == "" ? false : true;
        }


        public bool ValidateZipCodeIsCorrectFormat(string zipCode)
        {
            return zipCode.Length == 5 && Regex.IsMatch(zipCode, @"^\d+$")? true : false;
        }



        public bool ValidateCityIsFound(string city)
        {
            return city == null || city == "" ? false : true;
        }



        // **********************************************************



        public bool ValidateLocation(LocationViewModel location)
        {// Tests Generated
            // status 
            if(location.Status == "Update")
                return false;

            if (location.StreetAddress == null || location.City == null || location.StateAbbreviation == null || location.ZipCode == null)
                return false;

            if (location.Message == null || location.Message == "")
                return true;

            return false;
        }



        // *********************************************************************



        public bool ValidateVoterLocation(LocationViewModel location)
        {// Tests Generated
            if (location.StateAbbreviation == null || location.StateAbbreviation == "" || location.Status != "Display")
                return false;

            if (location.Message == null || location.Message == "")
                return true;

            return false;
        }



        public bool ValidatePollingLocation(LocationViewModel location)
        {// Tests Generated
            if (location.StateAbbreviation == null || location.StateAbbreviation == "" || location.Status != "Display")
                return false;

            if (location.Message == null || location.Message == "")
                return true;

            return false;
        }



        public bool ValidateCountyLocation(LocationViewModel location)
        {// Tests Generated
            if (location.StateAbbreviation == null || location.StateAbbreviation == "" || location.Status != "Display")
                return false;

            if (location.Message == null || location.Message == "")
                return true;

            return false;
        }


        public bool ValidateStateLocation(LocationViewModel location)
        {// Tests Generated
            if (location.StateAbbreviation == null || location.StateAbbreviation == "" || location.Status != "Display")
                return false;

            if (location.Message == null || location.Message == "")
                return true;

            return false;
        }



        // ******************************************************************************



        /// <summary>
        /// voter location form needs to be displayed
        /// </summary>
        public void UpdateSessionInformationForVoterLocation(LocationViewModel voterLocation)
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationInSession(voterLocation);
        }



        /// <summary>
        /// voter location form needs to be displayed
        /// </summary>
        public void UpdateSessionStatusToShowVoterLocationForm()
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();
            session.ChangeVoterLocationStatusToUpdateVoterLocationForm();
        }



        /// <summary>
        /// voter information can be displayed
        /// </summary>
        public void UpdateSessionStatusToDisplayVoterLocationInformation()
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();
            session.ChangeVoterLocationStatusToDisplayVoterLocation();
        }



        /// <summary>
        /// get the civic information from google api for voter location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        public SideBarViewModel GetSideBarViewModelFromGoogleCivicInformationAPI(LocationViewModel voterLocation)
        {// Tests Generated
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetGoogleCivicInformationForVoterLocation(voterLocation);
        }



        /// <summary>
        /// get google map for voter location and polling location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <param name="pollingLocation"></param>
        /// <returns></returns>
        public string GetGoogleMapForPollingLocation(LocationViewModel voterLocation, LocationViewModel pollingLocation)
        {// Tests Generated
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetGoogleMapAPIRequestForVoterAndPollingLocation(voterLocation, pollingLocation);
        }



        /// <summary>
        /// update the values stored in the session based on the voter location provided
        /// </summary>
        /// <param name="sideBarViewModel"></param>
        public void UpdateSessionFromSideBarViewModel(SideBarViewModel sideBarViewModel)
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();

            session.UpdateVoterLocationInSession(sideBarViewModel.VoterLocation);
            session.UpdatePollingLocationInSession(sideBarViewModel.PollingLocation);
            session.UpdateCountyLocationInSession(sideBarViewModel.CountyLocation);
        }




    }
}