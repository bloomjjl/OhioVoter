using OhioVoter.Models;
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





        [ChildActionOnly]
        public ActionResult DisplaySideBar()
        {
            SideBarViewModel sideBar = GetSideBarViewModel();

            return PartialView("_SideBar", sideBar);
        }






        /// <summary>
        /// update voter location information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayVoterLocationSideBar(SideBarViewModel sideBar)
        {// Tests Generated
            if (!ModelState.IsValid)
            {
                return Content("");
            }

            sideBar.VoterLocationViewModel.ControllerName = sideBar.ControllerName;

            if (ValidateVoterLocation(sideBar.VoterLocationViewModel))
            {
                return PartialView("_VoterLocation", sideBar.VoterLocationViewModel);
            }
            else
            {
                return PartialView("_VoterLocationForm", sideBar.VoterLocationViewModel);
            }
        }




        /// <summary>
        /// update polling location information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayPollingLocationSideBar(SideBarViewModel sideBar)
        {// Tests Generated
            if (!ModelState.IsValid)
            {
                return Content("");
            }

            if (sideBar.VoterLocationViewModel.Status != "Display")
            {
                return Content("");
            }
            else if (ValidatePollingLocation(sideBar.PollingLocationViewModel))
            {
                return PartialView("_VoterPollingLocation", sideBar.PollingLocationViewModel);
            }
            else
            {
                sideBar.PollingLocationViewModel.StreetAddress = "Location not found.";
                return PartialView("_VoterPollingLocation", sideBar.PollingLocationViewModel);
            }
        }



        /// <summary>
        /// update county information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayCountyLocationSideBar(SideBarViewModel sideBar)
        {// Tests Generated
            if (!ModelState.IsValid)
            {
                return Content("");
            }

            if (sideBar.VoterLocationViewModel.Status != "Display")
            {
                return Content("");
            }
            else
            {
                return PartialView("_VoterCountyInformation", sideBar.CountyLocationViewModel);
            }
        }



        /// <summary>
        /// update state information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayStateLocationSideBar(SideBarViewModel sideBar)
        {// Tests Generated
            if (!ModelState.IsValid)
            {
                return Content("");
            }

            return PartialView("_VoterStateInformation", sideBar.StateLocationViewModel);
        }





        /// <summary>
        /// get the voter location information from session to display in the sidebar
        /// </summary>
        /// <returns></returns>
        public SideBarViewModel GetSideBarViewModel()
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();

            SideBarViewModel sideBar = new SideBarViewModel()
            {
                ControllerName = session.GetControllerNameFromSession(),
                VoterLocationViewModel = session.GetVoterLocationFromSession(),
                PollingLocationViewModel = session.GetPollingLocationFromSession(),
                CountyLocationViewModel = session.GetCountyLocationFromSession(),
                StateLocationViewModel = GetAddressForOhioSecretaryOfState()
            };

            // is user in process of filling out ballot?
            if (TempData["VoterLocation"] != null)
            {
                VoterLocationViewModel location = (VoterLocationViewModel)TempData["VoterLocation"];
            }
            if (sideBar.VoterLocationViewModel.Message != "")
            {
                ModelState.AddModelError("VoterLocationViewModel", sideBar.VoterLocationViewModel.Message);
            }
                sideBar.PollingLocationViewModel = GetVoterLocationInformationToDisplayOnMap(sideBar.VoterLocationViewModel, sideBar.PollingLocationViewModel);
                        
            return sideBar;
        }



        /// <summary>
        /// Secretary of State will always be from Ohio
        /// store location information for Ohio SOS
        /// </summary>
        /// <returns></returns>
        public StateLocationViewModel GetAddressForOhioSecretaryOfState()
        {// Tests Generated
            return new StateLocationViewModel()
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



        public PollingLocationViewModel GetVoterLocationInformationToDisplayOnMap(VoterLocationViewModel voterLocationVM, PollingLocationViewModel pollingLocationVM)
        {

            pollingLocationVM.VoterStreetAddress = voterLocationVM.StreetAddress;
            pollingLocationVM.VoterCity = voterLocationVM.StateAbbreviation;
            pollingLocationVM.VoterStateAbbreviation = voterLocationVM.City;
            pollingLocationVM.VoterZipCode = voterLocationVM.ZipCode;

            return pollingLocationVM;
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
        public ActionResult Update(VoterLocationViewModel voterLocation)
        {// Tests not generated because of session testing null reference error
            // set up the controller for RedirectToAction
            string controllerName = GetControllerNameFromVoterLocationViewModel(voterLocation);

            if (!ModelState.IsValid)
            {
                TempData["VoterLocation"] = new VoterLocationViewModel();
                TempData["VoterLocation"] = voterLocation;
                ModelState.AddModelError("", "Valid street address and zip code are required.");
                voterLocation.Message = "Please enter a valid street address and zip code";
                UpdateSessionInformationForVoterLocation(voterLocation);
                return RedirectToAction("Index", controllerName);
            }

            VoterLocationViewModel location = UpdateSideBarInformationStoredInSessionFromVoterSuppliedStreetAddressAndZipCode(voterLocation.StreetAddress, voterLocation.ZipCode);
            
            if (location.Status == "Update")
            {
                location.StreetAddress = voterLocation.StreetAddress;
                location.ZipCode = voterLocation.ZipCode;
                UpdateSessionInformationForVoterLocation(location);
                UpdateSessionStatusToShowVoterLocationForm();
                return RedirectToAction("Index", controllerName);
            }

            return RedirectToAction("Index", controllerName);
        }



        public string GetControllerNameFromVoterLocationViewModel(VoterLocationViewModel voterLocation)
        {
            return (voterLocation.ControllerName == null || voterLocation.ControllerName == "") ?
                    "Home" :
                    voterLocation.ControllerName;
        }



        public VoterLocationViewModel UpdateSideBarInformationStoredInSessionFromVoterSuppliedStreetAddressAndZipCode(string streetAddress, string zipCode)
        {
            SideBarViewModel sideBar;            

            // validate format of provided values
            VoterLocationViewModel location = CheckVoterLocationFormFieldsAreValid(streetAddress, zipCode);
            if (location.Status == "Update") { return location; }

            // validate voter location
            VoterLocationViewModel voterLocation = GetVoterLocationInformationForSuppliedStreetAddressAndZipCodeFromDatabase(streetAddress, zipCode);
            if (voterLocation == null || voterLocation.StateAbbreviation != "OH")
            {
                voterLocation.StreetAddress = streetAddress;
                voterLocation.ZipCode = zipCode;

                // NOT A REGISTERED VOTER! Check if location is found on Google Map
                voterLocation = GetLocationFromGoogle(voterLocation);
                if (voterLocation.Status == "Update")
                {
                    return voterLocation;
                }

                // valid ohio address found on Google Map
                voterLocation.Message = "No registered voters at supplied address.";
                sideBar = GetSideBarViewModelFromGoogleCivicInformationAPI(voterLocation);
                sideBar.StateLocationViewModel = GetAddressForOhioSecretaryOfState();
            }
            else
            {
                PollingLocationViewModel pollingLocation = GetPollingLocationFromDatabase(voterLocation);
                CountyLocationViewModel countyLocation = GetCountyLocationFromDatabase(pollingLocation);
                StateLocationViewModel stateLocation = GetAddressForOhioSecretaryOfState();
                sideBar = new SideBarViewModel(voterLocation.ControllerName, voterLocation, pollingLocation, countyLocation, stateLocation);
            }

            sideBar = CheckEachLocationInSideBarIsValid(sideBar);

            if (sideBar.VoterLocationViewModel.Status == "Display")
            {
                sideBar.PollingLocationViewModel.GoogleLocationMapAPI = GetGoogleMapForPollingLocation(voterLocation, sideBar.PollingLocationViewModel);
                UpdateSessionFromSideBarViewModel(sideBar);
            }

            return voterLocation;
        }



        public VoterLocationViewModel GetLocationFromGoogle(VoterLocationViewModel location)
        {
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            VoterLocationViewModel googleLocation = instanceGoogleAPIManagement.GetAllLocationInformationForSuppliedAddress(location);
            
            // make sure google found a location matching the provided zip code
            if (googleLocation.ZipCode != location.ZipCode)
            {
                googleLocation.Status = "Update";
                googleLocation.Message = "Street address and/or zip code are not valid.";
                return googleLocation;
            }

            return CheckVoterLocationIsValid(googleLocation);
        }



        public bool ValidateVoterLocationInOhio(VoterLocationViewModel voterLocation)
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




        public VoterLocationViewModel GetVoterLocationInformationForSuppliedStreetAddressAndZipCodeFromDatabase(string streetAddress, string zipCode)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                string capsStreetAddress = streetAddress.ToUpper();
                int intZipCode = GetIntegerFromStringValue(zipCode);

                // look up most precise address first
                Models.HamiltonOhioVoter locationDTO = context.HamiltonOhioVoters.Where(x => x.AddressNumberAndPreDirectionAndStreetAndSuffix_Short == capsStreetAddress)
                                                                     .FirstOrDefault(x => x.AddressZip == intZipCode);

                // if not found remove only pre-direction from address
                if (locationDTO == null)
                {
                    locationDTO = context.HamiltonOhioVoters.Where(x => x.AddressNumberAndStreetAndSuffix_Short == capsStreetAddress)
                                                                             .FirstOrDefault(x => x.AddressZip == intZipCode);
                    // if not found remove only suffix from address
                    // if not found remove both pre-direction and suffix from address

                    if (locationDTO == null) { return new VoterLocationViewModel(); }
                }

                return new VoterLocationViewModel(locationDTO, "OH");
            }
        }



        public int GetIntegerFromStringValue(string strValue)
        {
            int intValue = 0;
            if (int.TryParse(strValue, out intValue))
            {
                intValue = int.Parse(strValue);
            }

            return intValue;
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
            return ValidateVoterLocation(sideBar.VoterLocationViewModel) &&
            ValidatePollingLocation(sideBar.PollingLocationViewModel) &&
            ValidateCountyLocation(sideBar.CountyLocationViewModel) &&
            ValidateStateLocation(sideBar.StateLocationViewModel) ?
            true : false;
        }




        public SideBarViewModel CheckEachLocationInSideBarIsValid(SideBarViewModel sideBar)
        {
            sideBar.VoterLocationViewModel = CheckVoterLocationIsValid(sideBar.VoterLocationViewModel);
            sideBar.PollingLocationViewModel = CheckPollingLocationIsValid(sideBar.PollingLocationViewModel);
            sideBar.CountyLocationViewModel = CheckCountyLocationIsValid(sideBar.CountyLocationViewModel);
            sideBar.StateLocationViewModel = CheckStateLocationIsValid(sideBar.StateLocationViewModel);

            return sideBar;
        }



        // ******************************************************************



        public VoterLocationViewModel CheckVoterLocationIsValid(VoterLocationViewModel location)
        {
            location.Message = "";
            location.Status = "Display";

            // check street address and zip code
            if (!ValidateStreetAddressIsFound(location.StreetAddress) || !ValidateZipCodeIsFound(location.ZipCode))
            {
                location.Message = "Address not found.";
                location.Status = "Update";
                return location;
            }

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



        public PollingLocationViewModel CheckPollingLocationIsValid(PollingLocationViewModel location)
        {
            location.Message = "";
            location.Status = "Display";

            // check street address and zip code
            if (!ValidateStreetAddressIsFound(location.StreetAddress) || !ValidateZipCodeIsFound(location.ZipCode))
            {
                location.Message = "Address not found.";
                location.Status = "Update";
                return location;
            }

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



        public CountyLocationViewModel CheckCountyLocationIsValid(CountyLocationViewModel location)
        {
            location.Message = "";
            location.Status = "Display";

            // check street address and zip code
            if (!ValidateStreetAddressIsFound(location.StreetAddress) || !ValidateZipCodeIsFound(location.ZipCode.ToString()))
            {
                location.Message = "Address not found.";
                location.Status = "Update";
                return location;
            }

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



        public StateLocationViewModel CheckStateLocationIsValid(StateLocationViewModel location)
        {
            location.Message = "";
            location.Status = "Display";

            // check street address and zip code
            if (!ValidateStreetAddressIsFound(location.StreetAddress) || !ValidateZipCodeIsFound(location.ZipCode))
            {
                location.Message = "Address not found.";
                location.Status = "Update";
                return location;
            }

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



        // ******************************************************************



        public VoterLocationViewModel CheckVoterLocationFormFieldsAreValid(string streetAddress, string zipCode)
        {
            VoterLocationViewModel location = new VoterLocationViewModel()
            {
                Message = "",
                Status = "Display"
            };

            // check street
            if (!ValidateStreetAddressIsFound(streetAddress))
            {
                location.Message = "Street address must be provided.";
                location.Status = "Update";
                return location;
            }

            // check zipcode
            if (!ValidateZipCodeIsFound(zipCode))
            {
                location.Message = "Zip code must be provided.";
                location.Status = "Update";
                return location;
            }
            else if (!ValidateZipCodeIsCorrectFormat(zipCode))
            {
                location.Message = "Zip code must be 5 numbers.";
                location.Status = "Update";
                return location;
            }

            return location;
        }



        // *******************************************************************



        public bool ValidateStateIsOhio(string stateAbbreviation)
        {
            return stateAbbreviation == "OH" ? true : false;
        }


        public bool ValidateStreetAddressIsFound(string streetAddress)
        {
            return streetAddress == null || streetAddress == "" ? false : true;
        }


        public bool ValidateZipCodeIsFound(string zipCode)
        {
            return zipCode == null || zipCode == "" ? false : true;
        }


        public bool ValidateZipCodeIsCorrectFormat(string zipCode)
        {
            return zipCode.Length == 5 && Regex.IsMatch(zipCode, @"^\d+$") ? true : false;
        }



        public bool ValidateCityIsFound(string city)
        {
            return city == null || city == "" ? false : true;
        }



        // **********************************************************



        public bool ValidateVoterLocation(VoterLocationViewModel location)
        {// Tests Generated
            if (location.Status != "Display")
            {
                return false;
            }
            else if (location.StreetAddress == null || location.StreetAddress == "" ||
            location.City == null || location.City == "" ||
            location.StateAbbreviation == null || location.StateAbbreviation == "" ||
            location.ZipCode == null || location.ZipCode == "")
            {
                return false;
            }
            else if (location.Message == null || location.Message == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public bool ValidatePollingLocation(PollingLocationViewModel location)
        {// Tests Generated
            if (location.Status != "Display")
            {
                return false;
            }
            else if (location.StreetAddress == null || location.StreetAddress == "" ||
            location.City == null || location.City == "" ||
            location.StateAbbreviation == null || location.StateAbbreviation == "" ||
            location.ZipCode == null || location.ZipCode == "")
            {
                return false;
            }
            else if (location.Message == null || location.Message == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        public bool ValidateCountyLocation(CountyLocationViewModel location)
        {// Tests Generated
            if (location.Status != "Display")
            {
                return false;
            }
            else if (location.StreetAddress == null || location.StreetAddress == "" ||
            location.City == null || location.City == "" ||
            location.StateAbbreviation == null || location.StateAbbreviation == "" ||
            location.ZipCode == null || location.ZipCode == "")
            {
                return false;
            }
            else if (location.Message == null || location.Message == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool ValidateStateLocation(StateLocationViewModel location)
        {// Tests Generated
            if (location.Status != "Display")
            {
                return false;
            }
            else if (location.StreetAddress == null || location.StreetAddress == "" ||
                location.City == null || location.City == "" ||
                location.StateAbbreviation == null || location.StateAbbreviation == "" ||
                location.ZipCode == null || location.ZipCode == "")
            {
                return false;
            }
            else if (location.Message == null || location.Message == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        // ******************************************************************************



        /// <summary>
        /// voter location form needs to be displayed
        /// </summary>
        public void UpdateSessionInformationForVoterLocation(VoterLocationViewModel voterLocation)
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




        public PollingLocationViewModel GetPollingLocationFromDatabase(VoterLocationViewModel voterLocation)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                Models.OhioPrecinct precinctDTO = context.OhioPrecincts.FirstOrDefault(x => x.Id == voterLocation.PrecinctId);

                if (precinctDTO == null) { return new PollingLocationViewModel(); }

                return new PollingLocationViewModel(precinctDTO);
            }
        }



        public CountyLocationViewModel GetCountyLocationFromDatabase(PollingLocationViewModel pollingLocationVM)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                Models.OhioBoardOfElection countyDTO = context.OhioBoardOfElections.FirstOrDefault(x => x.CountyId == pollingLocationVM.CountyId);

                if (countyDTO == null) { return new CountyLocationViewModel(); }

                return new CountyLocationViewModel(countyDTO);
            }
        }



        /// <summary>
        /// get the civic information from google api for voter location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        public SideBarViewModel GetSideBarViewModelFromGoogleCivicInformationAPI(VoterLocationViewModel voterLocation)
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
        public string GetGoogleMapForPollingLocation(VoterLocationViewModel voterLocation, PollingLocationViewModel pollingLocation)
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

            session.UpdateVoterLocationInSession(sideBarViewModel.VoterLocationViewModel);
            session.UpdatePollingLocationInSession(sideBarViewModel.PollingLocationViewModel);
            session.UpdateCountyLocationInSession(sideBarViewModel.CountyLocationViewModel);
        }




    }
}
 