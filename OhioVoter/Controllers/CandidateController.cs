using OhioVoter.Services;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.VoteSmart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Controllers
{
    public class CandidateController : Controller
    {
        // GET: Candidate
        public ActionResult Index()
        {
            LocationController location = new LocationController();

            CandidatePageViewModel viewModel = new CandidatePageViewModel()
            {
                SideBar = location.GetSideBarViewModel("Candidate"),
                Candidates = GetCadidateViewModel()
            };

            return View(viewModel);
        }



        // ********************************************
        // Sidebar information for supplied address
        // ********************************************



        /// <summary>
        /// get the voter location information from session to display in the sidebar
        /// </summary>
        /// <returns></returns>
        private SideBar GetSideBarViewModel()
        {
            SessionExtensions instanceSessionExtensions = new SessionExtensions();

            SideBar viewModel = new SideBar()
            {
                VoterLocation = instanceSessionExtensions.GetVoterLocationFromSession(),
                PollingLocation = instanceSessionExtensions.GetPollingLocationFromSession(),
                CountyLocation = instanceSessionExtensions.GetCountyLocationFromSession(),
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
        {
            return new Location()
            {
                Status = "Display",
                LocationName = "OHIO SECRETARY OF STATE",
                StreetAddress = "180 E. BROAD ST., 15TH FLOOR",
                City = "COLUMBUS",
                StateName = "OH",
                ZipCode = "43215-3726",
                Website = "http://www.sos.state.oh.us/elections.aspx"
            };
        }



        // TODO: fix error validation to display errors and messages when Location form submitted

        /// <summary>
        /// update voter location information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayVoter()
        {
            SideBar sideBarViewModel = GetSideBarViewModel();

            if (ValidateSideBarLocations(sideBarViewModel))
            {
                return PartialView("_VoterLocation", sideBarViewModel);
            }
            return PartialView("_VoterLocationForm", sideBarViewModel);
        }



        /// <summary>
        /// update general information displayed
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DisplayGeneral()
        {
            SideBar sideBarViewModel = GetSideBarViewModel();
            return PartialView("_VoterGeneralInformation", sideBarViewModel);
        }



        /// <summary>
        /// display information based on the voter location stored in the session
        /// </summary>
        /// <returns></returns>
        public ActionResult Location()
        {
            UpdateSessionToShowVoterLocationForm();

            if (!Request.IsAjaxRequest())
                return RedirectToAction("Index", new { });

            SideBar viewModel = GetSideBarViewModel();
            return PartialView("_VoterLocationForm", viewModel);
        }



        /// <summary>
        /// update page based on the voter location provided from form
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Location(Location voterLocation)
        {
            voterLocation.StateAbbreviation = "OH"; // address must be in Ohio

            SideBar sideBarViewModel = GetSideBarViewModelFromGoogleCivicInformationAPI(voterLocation);
            sideBarViewModel.StateLocation = GetAddressForOhioSecretaryOfState();

            if (ValidateSideBarLocations(sideBarViewModel))
            {
                UpdateSessionToDisplayVoterLocationInformation();

                sideBarViewModel.PollingLocation.GoogleLocationMapAPI = GetGoogleMapForPollingLocation(sideBarViewModel.VoterLocation, sideBarViewModel.PollingLocation);
                UpdateSessionFromSideBarViewModel(sideBarViewModel);
                //return PartialView("_VoterLocation", sideBarViewModel);
                return RedirectToAction("Index", "Home");
            }

            UpdateSessionToShowVoterLocationForm();
            //return PartialView("_VoterLocationForm", sideBarViewModel);
            return RedirectToAction("Index", new { });
        }



        /// <summary>
        /// make sure valid information is provided for all locations
        /// </summary>
        /// <param name="sideBarViewModel"></param>
        /// <returns></returns>
        private bool ValidateSideBarLocations(SideBar sideBarViewModel)
        {
            return sideBarViewModel.VoterLocation.Status == "Display" &&
                   sideBarViewModel.PollingLocation.Status == "Display" &&
                   sideBarViewModel.CountyLocation.Status == "Display" &&
                   sideBarViewModel.StateLocation.Status == "Display" ?
                   true : false;
        }



        /// <summary>
        /// voter location form needs to be displayed
        /// </summary>
        private void UpdateSessionToShowVoterLocationForm()
        {
            SessionExtensions instanceSessionExtensions = new SessionExtensions();
            instanceSessionExtensions.ChangeVoterLocationStatusToUpdateVoterLocationForm();
        }



        /// <summary>
        /// voter information can be displayed
        /// </summary>
        private void UpdateSessionToDisplayVoterLocationInformation()
        {
            SessionExtensions instanceSessionExtensions = new SessionExtensions();
            instanceSessionExtensions.ChangeVoterLocationStatusToUpdateVoterLocationForm();
        }



        /// <summary>
        /// get the civic information from google api for voter location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <returns></returns>
        private SideBar GetSideBarViewModelFromGoogleCivicInformationAPI(Location voterLocation)
        {
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetGoogleCivicInformationForVoterLocation(voterLocation);
        }



        /// <summary>
        /// get google map for voter location and polling location
        /// </summary>
        /// <param name="voterLocation"></param>
        /// <param name="pollingLocation"></param>
        /// <returns></returns>
        private string GetGoogleMapForPollingLocation(Location voterLocation, Location pollingLocation)
        {
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
            return instanceGoogleAPIManagement.GetGoogleMapAPIRequestForVoterAndPollingLocation(voterLocation, pollingLocation);
        }



        /// <summary>
        /// update the values stored in the session based on the voter location provided
        /// </summary>
        /// <param name="sideBarViewModel"></param>
        private void UpdateSessionFromSideBarViewModel(SideBar sideBarViewModel)
        {
            SessionExtensions instanceSessionExtensions = new SessionExtensions();

            instanceSessionExtensions.UpdateVoterLocationInSession(sideBarViewModel.VoterLocation);
            instanceSessionExtensions.UpdatePollingLocationInSession(sideBarViewModel.PollingLocation);
            instanceSessionExtensions.UpdateCountyLocationInSession(sideBarViewModel.CountyLocation);
        }



        // ********************************************
        // Candidate List based on provided name
        // ********************************************


        public List<Candidate> GetCadidateViewModel(string lastName)
        {
            List<Candidate> candidates = GetListOfCandidatesWithMatchingLastNameFromVoteSmart(lastName);
            return candidates.Concat(GetListOfCandidatesWithSimilarLastNameFromVoteSmart(lastName)).ToList();
        }



        public List<Candidate> GetListOfCandidatesWithMatchingLastNameFromVoteSmart(string lastName)
        {
            VoteSmartApiManagement voteSmartApi = new VoteSmartApiManagement();
            return voteSmartApi.GetVoteSmartMatchingCandidateListFromSuppliedLastName(lastName);
        }



        public List<Candidate> GetListOfCandidatesWithSimilarLastNameFromVoteSmart(string lastName)
        {
            VoteSmartApiManagement voteSmartApi = new VoteSmartApiManagement();
            return voteSmartApi.GetVoteSmartSimilarCandidateListFromSuppliedLastName(lastName);
        }



    }
}