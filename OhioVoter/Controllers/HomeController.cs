using OhioVoter.Models;
using OhioVoter.Services;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.Election;
using OhioVoter.ViewModels.RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace OhioVoter.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomePageViewModel viewModel = new HomePageViewModel()
            {
                SideBar = GetSideBarViewModel(),
                Calendar = GetCalendarViewModel(),
                CnnRssFeed = GetCNNRSSPoliticalFeedViewModel(),
                FoxNewsRssFeed = GetFoxNewsRSSPoliticalFeedViewModel()
            };

            return View(viewModel);
        }




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



        public Location GetAddressForOhioSecretaryOfState()
        {
            return new Location()
            {
                LocationName = "Ohio Secretary of State",
                StreetAddress = "180 E Broad St., 15th Floor",
                City = "Columbus",
                StateName = "OH",
                ZipCode = "43215-3726",
                Website = "http://www.sos.state.oh.us/elections.aspx"
            };
        }



        // ********************************************
        // Sidebar information for supplied address
        // ********************************************

        // TODO: fix error validation to display errors and messages when Location form submitted

        public ActionResult Location()
        {
            SessionExtensions instanceSessionExtensions = new SessionExtensions();
            instanceSessionExtensions.ChangeVoterLocationStatusToUpdateVoterLocationForm();

            if (!Request.IsAjaxRequest())
                return RedirectToAction("Index", new { });

            SideBar viewModel = GetSideBarViewModel();
            return PartialView("_VoterLocationForm", viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Location(Location voterLocation)
        {
            // if(modelstate.isvalid)
            voterLocation.StateAbbreviation = "OH";
            voterLocation.Status = "Display";
            GoogleAPIManagement instanceGoogleAPIManagement = new GoogleAPIManagement();
            SideBar sideBarViewModel = instanceGoogleAPIManagement.GetGoogleCivicInformationForVoterLocation(voterLocation);

            SessionExtensions instanceSessionExtensions = new SessionExtensions();

            // if (sideBarViewModel.PollingLocation.Status == "Update")
            if (sideBarViewModel.PollingLocation.City == null || sideBarViewModel.PollingLocation.City == "")
            {
                // update polling location in session
                // .LocationName = "Location not found";
                UpdateSessionFromSideBarViewModel(sideBarViewModel);
                instanceSessionExtensions.ChangeVoterLocationStatusToUpdateVoterLocationForm();

                if (!Request.IsAjaxRequest())
                    return RedirectToAction("Index", new { });

                SideBar formViewModel = GetSideBarViewModel();
                return PartialView("_VoterLocationForm", formViewModel);
            }

            instanceSessionExtensions.ChangeVoterLocationStatusToDisplayVoterLocation();

            sideBarViewModel.PollingLocation.GoogleLocationMapAPI = instanceGoogleAPIManagement.GetGoogleMapAPIRequestForVoterAndPollingLocation(sideBarViewModel.VoterLocation, sideBarViewModel.PollingLocation);
            instanceSessionExtensions.UpdatePollingLocationInSession(sideBarViewModel.PollingLocation);
            UpdateSessionFromSideBarViewModel(sideBarViewModel);

            if (!Request.IsAjaxRequest())
                return RedirectToAction("Index", new { });

            // TODO: implement partial view using AJAX
            SideBar viewModel = GetSideBarViewModel();
            return PartialView("_VoterLocation", viewModel);
        }



        private void UpdateSessionFromSideBarViewModel(SideBar sideBarViewModel)
        {
            SessionExtensions instanceSessionExtensions = new SessionExtensions();

            instanceSessionExtensions.UpdateVoterLocationInSession(sideBarViewModel.VoterLocation);
            instanceSessionExtensions.UpdatePollingLocationInSession(sideBarViewModel.PollingLocation);
            instanceSessionExtensions.UpdateCountyLocationInSession(sideBarViewModel.CountyLocation);
        }





        // ********************************************
        // Calendar for upcoming election dates
        // ********************************************

        // TODO: set up link to allow users to sign up for email reminders for upcoming election dates

        private Calendar GetCalendarViewModel()
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddDays(30);

            return new Calendar()
            {
                StartDate = startDate,
                EndDate = endDate,
                ElectionDates = GetListOfUpcomingElectionDates(startDate, endDate)
            };
        }



        private List<ElectionCalendar> GetListOfUpcomingElectionDates(DateTime startDate, DateTime endDate)
        {
            if (startDate <= endDate)
            {
                using (OhioVoterDbContext db = new OhioVoterDbContext())
                {
                    IEnumerable<ElectionCalendar> electionDates = db.ElectionCalendar.ToList();

                    return electionDates.Where(x => x.Date >= startDate)
                                        .Where(x => x.Date < endDate)
                                        .OrderBy(x => x.Date)
                                        .ToList();
                }
            }

            return new List<ElectionCalendar>();
        }







        // ********************************************
        // RSS Feed
        // ********************************************
        private Feed GetCNNRSSPoliticalFeedViewModel()
        {
            CNNRSSManagement instance = new CNNRSSManagement();
            return instance.GetCNNRSSPoliticalFeed();
        }



        private Feed GetFoxNewsRSSPoliticalFeedViewModel()
        {
            FoxNewsRSSManagement instance = new FoxNewsRSSManagement();
            return instance.GetFoxNewsRSSPoliticalFeed();
        }








        // *********************************************

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
    }
}