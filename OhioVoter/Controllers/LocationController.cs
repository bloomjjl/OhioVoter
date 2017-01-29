using OhioVoter.Services;
using OhioVoter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult DisplayVoterLocationSideBar(SideBar sideBarViewModel)
        {// Tests Generated
            return ValidateSideBarLocations(sideBarViewModel) ?
                    PartialView("_VoterLocation", sideBarViewModel) :
                    PartialView("_VoterLocationForm", sideBarViewModel);
        }



        /// <summary>
        /// update general information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayGeneralLocationSideBar(SideBar sideBarViewModel)
        {// Tests Generated
            return PartialView("_VoterGeneralInformation", sideBarViewModel);
        }



        /// <summary>
        /// get the voter location information from session to display in the sidebar
        /// </summary>
        /// <returns></returns>
        public SideBar GetSideBarViewModel(string controllerName)
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();

            SideBar viewModel = new SideBar()
            {
                ControllerName = controllerName,
                VoterLocation = session.GetVoterLocationFromSession(),
                PollingLocation = session.GetPollingLocationFromSession(),
                CountyLocation = session.GetCountyLocationFromSession(),
                StateLocation = GetAddressForOhioSecretaryOfState()
            };

            return viewModel;
        }



        /// <summary>
        /// Secretary of State will always be from Ohio
        /// store location information for Ohio SOS
        /// </summary>
        /// <returns></returns>
        public Location GetAddressForOhioSecretaryOfState()
        {// Tests Generated
            return new Location()
            {
                Status = "Display",
                LocationName = "OHIO SECRETARY OF STATE",
                StreetAddress = "180 E. BROAD ST., 15TH FLOOR",
                City = "COLUMBUS",
                StateAbbreviation = "OH",
                ZipCode = "43215-3726",
                Website = "http://www.sos.state.oh.us/elections.aspx"
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
        public ActionResult Update(Location voterLocation, string controllerName)
        {// Tests not generated because of session testing null reference error
            // validate supplied street address and zip code are in OHIO
            if (ValidateVoterLocationInOhio(voterLocation) == false)
            {
                ModelState.AddModelError("", "Valid Ohio Address Required");
                return RedirectToAction("Index", controllerName);
            }

            voterLocation.StateAbbreviation = "OH"; // address must be in Ohio

            SideBar sideBarViewModel = GetSideBarViewModelFromGoogleCivicInformationAPI(voterLocation);
            sideBarViewModel.StateLocation = GetAddressForOhioSecretaryOfState();

            if (ValidateSideBarLocations(sideBarViewModel))
            {
                UpdateSessionStatusToDisplayVoterLocationInformation();

                sideBarViewModel.PollingLocation.GoogleLocationMapAPI = GetGoogleMapForPollingLocation(sideBarViewModel.VoterLocation, sideBarViewModel.PollingLocation);
                UpdateSessionFromSideBarViewModel(sideBarViewModel);
                //return PartialView("_VoterLocation", sideBarViewModel);
                return RedirectToAction("Index", controllerName);
            }

            UpdateSessionStatusToShowVoterLocationForm();
            //return PartialView("_VoterLocationForm", sideBarViewModel);
            return RedirectToAction("Index", controllerName);
        }




        public bool ValidateVoterLocationInOhio(Location voterLocation)
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
        public bool ValidateSideBarLocations(SideBar sideBarViewModel)
        {// Tests Generated
            return ValidateVoterLocation(sideBarViewModel.VoterLocation) &&
                   ValidatePollingLocation(sideBarViewModel.PollingLocation) &&
                   ValidateCountyLocation(sideBarViewModel.CountyLocation) &&
                   ValidateStateLocation(sideBarViewModel.StateLocation) ?
                   true : false;
        }



        public bool ValidateVoterLocation(Location location)
        {// Tests Generated
            return location.Status != null &&
                   location.Status != "" &&
                   location.Status == "Display" &&
                   location.StreetAddress != null &&
                   location.StreetAddress != "" &&
                   location.StateAbbreviation != null &&
                   location.StateAbbreviation != "" &&
                   location.ZipCode != null ?
                   true : false;
        }



        public bool ValidatePollingLocation(Location location)
        {// Tests Generated
            return location.Status != null &&
                   location.Status != "" &&
                   location.Status == "Display" &&
                   location.StreetAddress != null &&
                   location.StreetAddress != "" ?
                   true : false;
        }



        public bool ValidateCountyLocation(Location location)
        {// Tests Generated
            return location.Status != null &&
                   location.Status != "" &&
                   location.Status == "Display" &&
                   location.City != null &&
                   location.City != "" ?
                   true : false;
        }


        public bool ValidateStateLocation(Location location)
        {// Tests Generated
            return location.Status != null &&
                   location.Status != "" &&
                   location.Status == "Display" &&
                   location.StateAbbreviation == "OH" ?
                   true : false;
        }



        /// <summary>
        /// voter location form needs to be displayed
        /// </summary>
        public void UpdateSessionInformationForVoterLocation(Location voterLocation)
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
            session.ChangeVoterLocationStatusToUpdateVoterLocationForm();
        }



        /// <summary>
        /// get the civic information from google api for voter location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        public SideBar GetSideBarViewModelFromGoogleCivicInformationAPI(Location voterLocation)
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
        public string GetGoogleMapForPollingLocation(Location voterLocation, Location pollingLocation)
        {// Tests Generated
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetGoogleMapAPIRequestForVoterAndPollingLocation(voterLocation, pollingLocation);
        }



        /// <summary>
        /// update the values stored in the session based on the voter location provided
        /// </summary>
        /// <param name="sideBarViewModel"></param>
        public void UpdateSessionFromSideBarViewModel(SideBar sideBarViewModel)
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();

            session.UpdateVoterLocationInSession(sideBarViewModel.VoterLocation);
            session.UpdatePollingLocationInSession(sideBarViewModel.PollingLocation);
            session.UpdateCountyLocationInSession(sideBarViewModel.CountyLocation);
        }




    }
}