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
                Poll = GetPollResultsViewModel(),
                RssFeeds = GetRssFeedsViewModel()
            };

            /*
            VoteSmartApiManagement voteSmartApi = new VoteSmartApiManagement();
            string test = voteSmartApi.GetVoteSmartCandidateInformation();
            */

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
            GoogleApiManagement instanceGoogleAPIManagement = new GoogleApiManagement();
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



        private List<ElectionDate> GetListOfUpcomingElectionDates(DateTime startDate, DateTime endDate)
        {
            if (startDate <= endDate)
            {
                using (OhioVoterDbContext db = new OhioVoterDbContext())
                {
                    IEnumerable<ElectionDate> electionDates = db.ElectionDates.ToList();

                    return electionDates.Where(x => x.Date >= startDate)
                                        .Where(x => x.Date < endDate)
                                        .OrderBy(x => x.Date)
                                        .ToList();
                }
            }

            return new List<ElectionDate>();
        }





        // ********************************************
        // Poll results from users filling out ballot
        // ********************************************

        // TODO: set up database and retrieve polling data

        private Poll GetPollResultsViewModel()
        {
            Poll poll = new Poll()
            {
                ElectionDate = Convert.ToDateTime("11/08/2016"),
                OfficeName = "President",
                CandidateVotes = GetCandidateVotesFromBallot()
            };

            // sort candidates by highest to lowest votes received
            int totalVotesForOffice = GetTotalVotesForOfficeFromBallot(poll.CandidateVotes);
            poll.CandidateVotes = GetPercentageOfVotesEachCandidateReceivedFromBallot(poll.CandidateVotes, totalVotesForOffice);
            poll.CandidateVotes = RemoveCandidatesWithZeroVotesFromList(poll.CandidateVotes);

            return poll;
        }



        // moch database
        private IEnumerable<CandidateVote> GetCandidateVotesFromBallot()
        {
            List<CandidateVote> candidateResults = new List<CandidateVote>()
            {
                new CandidateVote() { Candidate = "Donald Trump", CoCandidate = "", Party = "Republican", PartyColor = "Red", VoteCount = 2841005 },
                new CandidateVote() { Candidate = "Hillary Clinton", CoCandidate = "", Party = "Democrat", PartyColor = "Blue", VoteCount = 2394164 },
                new CandidateVote() { Candidate = "Gary Johnson", CoCandidate = "", Party = "Libertarian", PartyColor = "Yellow", VoteCount = 174498 },
                new CandidateVote() { Candidate = "Jill Stein", CoCandidate = "", Party = "Green", PartyColor = "Green", VoteCount = 46271 },
                new CandidateVote() { Candidate = "Richard Duncan", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 24235 },
                new CandidateVote() { Candidate = "Evan McMullin", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 12574 },
                new CandidateVote() { Candidate = "Darrell Castle", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 1887 },
                new CandidateVote() { Candidate = "Ben Hartnell", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 589 },
                new CandidateVote() { Candidate = "Michael Maturen", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 552 },
                new CandidateVote() { Candidate = "Tom Hoefling", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 268 },
                new CandidateVote() { Candidate = "Chris Keniston", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 114 },
                new CandidateVote() { Candidate = "Laurence Kotlikoff", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 90 },
                new CandidateVote() { Candidate = "Joe Schriner", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 62 },
                new CandidateVote() { Candidate = "Mike Smith", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 62 },
                new CandidateVote() { Candidate = "Josiah Stroh", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 30 },
                new CandidateVote() { Candidate = "Monica Moorehead", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 19 },
                new CandidateVote() { Candidate = "Joseph Maldonado", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 18 },
                new CandidateVote() { Candidate = "Barry Kirschner", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 15 },
                new CandidateVote() { Candidate = "James Bell", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 9 },
                new CandidateVote() { Candidate = "Bruce Jaynes", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 8 },
                new CandidateVote() { Candidate = "Michael Bickelmeyer", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 6 },
                new CandidateVote() { Candidate = "Douglas Thomson", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 6 },
                new CandidateVote() { Candidate = "Cherunda Fox", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 5 }
            };

            return candidateResults;
        }



        private int GetTotalVotesForOfficeFromBallot(IEnumerable<CandidateVote> candidateVotes)
        {
            int count = 0;

            foreach (CandidateVote candidate in candidateVotes)
            {
                if (candidate.VoteCount > 0)
                    count += candidate.VoteCount;
            }

            return count;
        }



        private IEnumerable<CandidateVote> GetPercentageOfVotesEachCandidateReceivedFromBallot(IEnumerable<CandidateVote> candidateVotes, int totalVotes)
        {
            foreach (CandidateVote candidate in candidateVotes)
            {
                if (candidate.VoteCount > 0)
                {
                    // store decimal values in a percent format = 100.0 %
                    candidate.VotePercent = Decimal.Round(((decimal)candidate.VoteCount / totalVotes) * 100, 1);
                }
            }

            return candidateVotes;
        }



        private IEnumerable<CandidateVote> RemoveCandidatesWithZeroVotesFromList(IEnumerable<CandidateVote> candidateVotes)
        {
            return candidateVotes.Where(c => c.VotePercent > 0.0m);
        }








        // ********************************************
        // RSS Feed
        // ********************************************

        private RssFeed GetRssFeedsViewModel()
        {
            RssManagement rssManager = new RssManagement();

            return new RssFeed()
            {
                FoxNewsRssFeed = rssManager.GetFoxNewsRssPoliticalFeed(),
                CnbcRssFeed = rssManager.GetCnbcRSSPoliticalFeed(),
                CnnRssFeed = rssManager.GetCnnRssPoliticalFeed()
            };
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