using OhioVoter.Services;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace OhioVoter.Controllers
{
    public class HomeController : Controller
    {
        private static string _controllerName = "Home";


        /// <summary>
        /// get the information to display on page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            LocationController location = new LocationController();

            UpdateSessionWithNewControllerNameForSideBar(_controllerName);
            Models.ElectionVotingDate votingDate = GetOldestActiveElectionVotingDate();
            int pollingElectionOfficeId = 0;

            HomeViewModel viewModel = new HomeViewModel()
            {
                ControllerName = _controllerName,
                CalendarViewModel = GetCalendarViewModel(),
                PollViewModel = GetPollResultsViewModel(votingDate, pollingElectionOfficeId),
                RssFeedsViewModel = GetRssFeedViewModel()
            };

            return View(viewModel);
        }




        protected override void OnException(ExceptionContext filterContext)
        {            
            // set the result without redirection:
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Error/Index.cshtml"
            };            
        }





        [HttpGet]
        public ActionResult EmailSignUp()
        {
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);

            // display form for user to fill out
            return View("EmailSignUp", new EmailSignUpViewModel(_controllerName));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailSignup(string emailAddress)
        {
            // validate email format
            if (string.IsNullOrEmpty(emailAddress))
            {
                ModelState.AddModelError("", "Email Address is required");
                return View("EmailSignUp");
            }

            // update database to start sending email correspondence
            Services.Email email = new Services.Email();
            string result = email.SetUpSuppliedEmailAddressToReceiveEmailRemindersInDatabase(emailAddress);

            // present user with view to verify email supplied
            if (result == "Success")
            {
                TempData["EmailSignUpMessage"] = "You have successfully signed up.";
                return View("EmailSignUp");
            }
            else if (result == "Already Setup")
            {
                TempData["EmailSignUpMessage"] = "You have successfully signed up.";
                return View("EmailSignUp");
            }
            else if (result == "Email Required")
            {
                TempData["EmailSignUpMessage"] = string.Empty;
                ModelState.AddModelError("", "Check your Email Address");
                return View("EmailSignUp");
            }
            else if (result == "Problem")
            {
                TempData["EmailSignUpMessage"] = string.Empty;
                ModelState.AddModelError("", "We are unable to handle your request at this time. Please try again later.");
                return View("EmailSignUp");
            }
            else
            {
                TempData["EmailSignUpMessage"] = string.Empty;
                ModelState.AddModelError("", "We are unable to handle your request at this time. Please try again later.");
                return View("EmailSignUp");
            }
        }






        public void SendEmailToUserForVerification(string emailAddress, bool isSetUp)
        {
            Services.Email email = new Services.Email();
            bool isEmailSent = false;
            string subject = string.Format("{0}", "Email Verification");
            string body = string.Format("{0} {1}\r\n\r\n{2}{3}\r\n\r\n{4}\r\n\r\n{5}\r\n\r\n{6}\r\n\r\n{7}",
                "We have received a request to authorize this email address to receive monthly correspondence from OhioVoter.org about upcoming election dates.",
                "If you requested this verification, please go to the following URL to confirm that you are authorized to use this email address:",
                "https://ohiovoter.org/home/emailverification?emailAddress=",
                emailAddress,
                "Your request will not be processed unless you confirm the address using this URL. This link expires 24 hours after your original verification request.",
                "If you did NOT request to verify this email address, do not click on the link.",
                "Sincerely",
                "The OhioVoter.org Team");


            if (isSetUp)
            {
                // send email address initial email to verify ownership
                isEmailSent = email.SendEmail(emailAddress, subject, body);
                // display message to check inbox to validate supplied email
            }
            else
            {
                // display message that encountered a problem setting up email check email address and try again.
            }
        }






        public ActionResult EmailVerification(string emailAddress)
        {
            // validate parameter
            if (string.IsNullOrEmpty(emailAddress))
            {
                // no email provided to check
            }

            // update email list
            bool isUpdated = UpdateEmailVerificationInEmailListInDatabase(emailAddress);
            string messageHeader;
            string messageBody;

            if (isUpdated)
            {
                messageHeader = "SUCCESS!";
                messageBody = "Your email has been verified.";
            }
            else
            {
                messageHeader = "PROBLEM!";
                messageBody = string.Format("{0}", 
                    "Your email was not verified. Please click the following link to resend a verification email.");
            }
            // send email verifying update
            return View();
        }



        public bool UpdateEmailVerificationInEmailListInDatabase(string emailAddress)
        {
            bool isUpdated = false;

            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                Models.EmailList emailDTO = context.EmailLists.FirstOrDefault(x => x.EmailAddress == emailAddress);
                if (emailDTO == null) { return isUpdated; }

                if (emailDTO.IsActive)
                {
                    // make sure updated within time constraint
                    int hourResponseMax = 24;
                    DateTime beginDate = emailDTO.DateModified;


                    if (DateTime.Now.CompareTo(beginDate.AddHours(hourResponseMax)) <= 0)
                    {
                        emailDTO.IsVerified = true;

                        context.SaveChanges();

                        isUpdated = true;
                    }
                }
            }

            return isUpdated;
        }


        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }



        // ********************************************
        // Calendar for upcoming election dates
        // ********************************************

        /// <summary>
        /// get the election date information to display on page
        /// </summary>
        /// <returns></returns>
        public CalendarViewModel GetCalendarViewModel()
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddDays(30);

            return new CalendarViewModel()
            {
                StartDate = startDate,
                EndDate = endDate,
                FullElectionCalenderFile = "2017_OhioElectionCalendar.pdf",
                ElectionDates = GetListOfUpcomingElectionDates(startDate, endDate)
            };
        }



        public ActionResult ViewFullElectionCalendar(string fileName)
        {
            string filePath = Server.MapPath("~/Content/images/") + fileName;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "inline;filename=" + filePath);
            Response.ContentType = "application/pdf";
            Response.WriteFile(filePath);
            Response.Flush();
            Response.Clear();
            return View();
        }





        /// <summary>
        /// Get election date information from database
        /// sort and store the election dates for a specified period of time
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<ElectionDate> GetListOfUpcomingElectionDates(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return new List<ElectionDate>();

                List<Models.ElectionDate> electionDates = new List<Models.ElectionDate>();
                using (Models.OhioVoterDbContext db = new Models.OhioVoterDbContext())
                {
                    electionDates = db.ElectionDates.Where(x => x.Date >= startDate)
                                                    .Where(x => x.Date < endDate)
                                                    .OrderBy(x => x.Date)
                                                    .ToList();
                }

                return CopyElectionDatesToViewModel(electionDates);
            }
            catch
            {
                return new List<ElectionDate>();
            }
        }


        public Models.ElectionVotingDate GetOldestActiveElectionVotingDate()
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.ElectionVotingDate> dbVotingDates = context.ElectionVotingDates.Where(x => x.Active == true).ToList();
                if(dbVotingDates == null) { return new Models.ElectionVotingDate(); }

                return dbVotingDates[0];
            }
        }



        public IEnumerable<ElectionDate> CopyElectionDatesToViewModel(List<Models.ElectionDate> electionDates)
        {
            List<ElectionDate> dates = new List<ElectionDate>();

            for (int i = 0; i < electionDates.Count; i++)
            {
                dates.Add(new ElectionDate()
                {
                    DateId = electionDates[i].Id,
                    Date = electionDates[i].Date,
                    Description = electionDates[i].Description
                });
            }

            IEnumerable<ElectionDate> iDates = dates;

            return iDates;
        }



        // ********************************************
        // Poll results from users filling out ballot
        // ********************************************


            
            // TODO: allow user to change the office if location provided



        
        public ActionResult UpdatePollGraph(int selectedElectionOfficeId)
        {
            Models.ElectionVotingDate votingDate = GetOldestActiveElectionVotingDate();
            IEnumerable<CandidateVoteViewModel> candidateVotes = GetCandidateVotesFromBallotFromDatabase(votingDate.Id, selectedElectionOfficeId);
            candidateVotes = GetListOfCandidatesForOfficeSortedByNumberOfVotes(candidateVotes);

            if (candidateVotes.Count() > 0)
            {
                return PartialView("_PollGraph", candidateVotes);
            }

            return PartialView("_PollGraphEmpty");
        }



        /// <summary>
        /// get polling information for users that have filled out the ballot from database
        /// sort and store the polling results to display on page
        /// </summary>
        /// <returns></returns>
        private PollViewModel GetPollResultsViewModel(Models.ElectionVotingDate votingDate, int electionOfficeId)
        {
            // check parameters
            if (electionOfficeId <= 0) { electionOfficeId = 1; }

            // get list of election offices on ballots
            IEnumerable<SelectListItem> electionOfficeSelectList;

            // get dropdownlist of election offices with electionOfficeId selected
            electionOfficeSelectList = GetElectionPollingOfficeListItems(votingDate.Id, electionOfficeId);                  
                    
            PollViewModel pollVM = new PollViewModel()
            {
                ElectionDate = votingDate.Date.ToShortDateString(),
                //OfficeName = "President",
                ElectionOfficeNames = electionOfficeSelectList,
                CandidateVotes = GetCandidateVotesFromBallotFromDatabase(votingDate.Id, electionOfficeId)
            };

            // sort candidates by highest to lowest votes received and display if greater than 0.0
            pollVM.CandidateVotes = GetListOfCandidatesForOfficeSortedByNumberOfVotes(pollVM.CandidateVotes);
            
            return pollVM;
        }



        public IEnumerable<CandidateVoteViewModel> GetListOfCandidatesForOfficeSortedByNumberOfVotes(IEnumerable<CandidateVoteViewModel> candidateVotes)
        {
            // sort candidates by highest to lowest votes received and display if greater than 0.0
            int totalVotesForOffice = GetTotalVotesForOfficeFromBallot(candidateVotes);
            candidateVotes = GetPercentageOfVotesEachCandidateReceivedFromBallot(candidateVotes, totalVotesForOffice);
            candidateVotes= RemoveCandidatesWithZeroVotesFromList(candidateVotes);
            return candidateVotes.OrderByDescending(x => x.VoteCount).ToList();
        }



        public IEnumerable<SelectListItem> GetElectionPollingOfficeListItems(int dateId, int selectedOfficeId)
        {
            // validate input values
            if (dateId <= 0) { return new List<SelectListItem>(); }

            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                // create select list object
                List<SelectListItem> electionOffices = new List<SelectListItem>();

                // get list of offices with candidates for election
                // exclude runningmate offices from list (Vice President)
                int vicePresident = 2;

                List<Models.ElectionCandidate> dbElectionCandidateOffices = context.ElectionCandidates
                    .Include("ElectionOffice")
                    .Include("ElectionOffice.Office")
                    .Where(x => x.ElectionVotingDateId == dateId)
                    .Where(x => x.ElectionOffice.OfficeId != vicePresident)
                    .ToArray()
                    .GroupBy(x => x.ElectionOfficeId)
                    .Select(g => g.First())
                    .ToList();

                if (dbElectionCandidateOffices == null) { return new List<SelectListItem>(); }

                for (int i = 0; i < dbElectionCandidateOffices.Count(); i++)
                {
                    string officeName;

                    // add term to office if one is available
                    if (!string.IsNullOrEmpty(dbElectionCandidateOffices[i].ElectionOffice.OfficeTerm))
                    {
                        officeName = string.Format("{0} ({1})", dbElectionCandidateOffices[i].ElectionOffice.Office.OfficeName.ToUpper(), dbElectionCandidateOffices[i].ElectionOffice.OfficeTerm.ToUpper());
                    }
                    else
                    {
                        officeName = dbElectionCandidateOffices[i].ElectionOffice.Office.OfficeName.ToUpper();
                    }

                    electionOffices.Add(new SelectListItem()
                    {
                        Value = dbElectionCandidateOffices[i].ElectionOfficeId.ToString(),
                        Text = officeName,
                        Selected = dbElectionCandidateOffices[i].ElectionOfficeId == selectedOfficeId
                    });
                }

                return electionOffices;
            }
        }



        public IEnumerable<CandidateVoteViewModel> GetCandidateVotesFromBallotFromDatabase(int votingDateId, int electionOfficeId)
        {
            // validate parameters
            if (votingDateId <= 0) { return new List<CandidateVoteViewModel>(); }

            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                // get complete list of candidates for current election
                List<Models.ElectionCandidate> dbElectionCandidates = context.ElectionCandidates
                    .Include("Candidate")
                    .Include("Party")
                    .Where(x => x.ElectionOfficeId == electionOfficeId)
                    .ToList();

                if (dbElectionCandidates == null) { return new List<CandidateVoteViewModel>(); }

                // get a list of candidates (by office) that were selected on any ballot emailed
                List<Models.BallotCandidate> dbBallotCandidates = context.BallotCandidates
                    .Include("BallotOffice")
                    .Where(x => x.BallotOffice.ElectionOfficeId == electionOfficeId)
                    .ToList();

                if (dbBallotCandidates == null) { return new List<CandidateVoteViewModel>(); }

                // convert database objects to ViewModel
                List<CandidateVoteViewModel> candidateListVM = new List<CandidateVoteViewModel>();
                for (int i = 0; i < dbElectionCandidates.Count(); i++)
                {
                    string votesmartImageUrl = dbElectionCandidates[i].Candidate.VoteSmartPhotoUrl;
                    string gender = dbElectionCandidates[i].Candidate.Gender;

                    candidateListVM.Add(new CandidateVoteViewModel()
                    {
                        Candidate = dbElectionCandidates[i].Candidate.CandidateFirstLastName,
                        Party = dbElectionCandidates[i].Party.PartyName,
                        PartyColor = dbElectionCandidates[i].Party.PartyColor,
                        VoteCount = dbBallotCandidates.Where(x => x.ElectionCandidateId == dbElectionCandidates[i].Id).Count(),
                        ImageUrl = GetValidImageLocationToDisplay(votesmartImageUrl, gender)
                    });
                }

                return candidateListVM;
            }
        }



        public string GetValidImageLocationToDisplay(string voteSmartUrl, string gender)
        {
            if (!string.IsNullOrEmpty(voteSmartUrl)) { return voteSmartUrl; }

            if (gender == "F" || gender == "Female")
            {
                return "~/Content/images/image_female.png";
            }
            else
            {
                return "~/Content/images/image_male.png";
            }
        }



        private IEnumerable<CandidateVoteViewModel> GetCandidateVotesFromBallot()
        {
            List<CandidateVoteViewModel> candidateResults = new List<CandidateVoteViewModel>()
            {
                new CandidateVoteViewModel() { Candidate = "Hillary Clinton", CoCandidate = "", Party = "Democrat", PartyColor = "Blue", VoteCount = 2394164,  ImageUrl = "https://static.votesmart.org/canphoto/55463.jpg" },
                new CandidateVoteViewModel() { Candidate = "Gary Johnson", CoCandidate = "", Party = "Libertarian", PartyColor = "Yellow", VoteCount = 174498,  ImageUrl = "https://static.votesmart.org/canphoto/22377.jpg"  },
                new CandidateVoteViewModel() { Candidate = "Jill Stein", CoCandidate = "", Party = "Green", PartyColor = "Green", VoteCount = 46271,  ImageUrl = "https://static.votesmart.org/canphoto/35775.jpg"  },
                new CandidateVoteViewModel() { Candidate = "Donald Trump", CoCandidate = "", Party = "Republican", PartyColor = "Red", VoteCount = 2841005, ImageUrl = "https://static.votesmart.org/canphoto/15723.jpg" },
                new CandidateVoteViewModel() { Candidate = "Richard Duncan", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 24235,  ImageUrl = "https://static.votesmart.org/canphoto/65939.jpg"  },
                new CandidateVoteViewModel() { Candidate = "Evan McMullin", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 12574,  ImageUrl = "https://static.votesmart.org/canphoto/174905.jpg"  },
                new CandidateVoteViewModel() { Candidate = "Darrell Castle", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 1887,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Ben Hartnell", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 589,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Michael Maturen", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 552,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Tom Hoefling", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 268,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Chris Keniston", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 114,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Laurence Kotlikoff", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 90,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Joe Schriner", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 62,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Mike Smith", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 62,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Josiah Stroh", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 30,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Monica Moorehead", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 19,  ImageUrl = ""  },
                new CandidateVoteViewModel() { Candidate = "Joseph Maldonado", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 18,  ImageUrl = "" },
                new CandidateVoteViewModel() { Candidate = "Barry Kirschner", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 15,  ImageUrl = "" },
                new CandidateVoteViewModel() { Candidate = "James Bell", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 9,  ImageUrl = "" },
                new CandidateVoteViewModel() { Candidate = "Bruce Jaynes", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 8,  ImageUrl = "" },
                new CandidateVoteViewModel() { Candidate = "Michael Bickelmeyer", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 6,  ImageUrl = "" },
                new CandidateVoteViewModel() { Candidate = "Douglas Thomson", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 6,  ImageUrl = "" },
                new CandidateVoteViewModel() { Candidate = "Cherunda Fox", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 5,  ImageUrl = "" }
            };

            return candidateResults;
        }



        /// <summary>
        /// get the total number of votes each candidate received
        /// </summary>
        /// <param name="candidateVotes"></param>
        /// <returns></returns>
        private int GetTotalVotesForOfficeFromBallot(IEnumerable<CandidateVoteViewModel> candidateVotes)
        {
            int count = 0;

            foreach (CandidateVoteViewModel candidate in candidateVotes)
            {
                if (candidate.VoteCount > 0)
                    count += candidate.VoteCount;
            }

            return count;
        }



        /// <summary>
        /// store the percentage of total votes each candidate received
        /// </summary>
        /// <param name="candidateVotes"></param>
        /// <param name="totalVotes"></param>
        /// <returns></returns>
        private IEnumerable<CandidateVoteViewModel> GetPercentageOfVotesEachCandidateReceivedFromBallot(IEnumerable<CandidateVoteViewModel> candidateVotes, int totalVotes)
        {
            foreach (CandidateVoteViewModel candidate in candidateVotes)
            {
                if (candidate.VoteCount > 0)
                {
                    // store decimal values in a percent format = 100.0 %
                    candidate.VotePercent = Decimal.Round(((decimal)candidate.VoteCount / totalVotes) * 100, 1);
                }
            }

            return candidateVotes;
        }



        /// <summary>
        /// only store the candidates who have an average of the total votes greater than 0.0
        /// </summary>
        /// <param name="candidateVotes"></param>
        /// <returns></returns>
        private IEnumerable<CandidateVoteViewModel> RemoveCandidatesWithZeroVotesFromList(IEnumerable<CandidateVoteViewModel> candidateVotes)
        {
            return candidateVotes.Where(c => c.VotePercent > 0.0m);
        }








        // ********************************************
        // RSS Feed
        // ********************************************

        /// <summary>
        /// get the information for rss feeds
        /// </summary>
        /// <returns></returns>
        public RssFeeds GetRssFeedViewModel()
        {
            RssFeeds feeds = new RssFeeds()
            {
                //FoxNewsRssFeed = CopyFoxNewsRssPoliticalFeedToViewModel(),
                CnbcRssFeed = CopyCnbcRssPoliticalFeedToViewModel(),
                //CnnRssFeed = CopyCnnRssPoliticalFeedToViewModel()
                //OhioSecretaryOfStateRssFeed = CopyOhioSecretaryOfStateRssFeedToViewModel()
            };

            feeds.CnbcRssFeed = ConvertRssHttpLinkToHttps(feeds.CnbcRssFeed);
            //feeds.CnnRssFeed = ConvertRssHttpLinkToHttps(feeds.CnnRssFeed);

            return feeds;
        }


        public RssFeedViewModel ConvertRssHttpLinkToHttps(RssFeedViewModel feed)
        {
            if(feed.Channel.Element.Link_0 != null && feed.Channel.Element.Link_0.Contains("http:"))
            {
                string path = feed.Channel.Element.Link_0.Remove(0, 5);
                feed.Channel.Element.Link_0 = string.Format("https:{0}", path);
            }
            if(feed.Channel.Element.Link_1 != null && feed.Channel.Element.Link_1.Contains("http:"))
            {
                string path = feed.Channel.Element.Link_1.Remove(0, 5);
                feed.Channel.Element.Link_1 = string.Format("https:{0}", path);
            }
            if (feed.Channel.Element.Link_2 != null && feed.Channel.Element.Link_2.Contains("http:"))
            {
                string path = feed.Channel.Element.Link_2.Remove(0, 5);
                feed.Channel.Element.Link_2 = string.Format("https:{0}", path);
            }

            foreach (var item in feed.Items)
            {
                if (item.Element.Link_0 != null && item.Element.Link_0.Contains("http:"))
                {
                    string path = item.Element.Link_0.Remove(0, 5);
                    item.Element.Link_0 = string.Format("https:{0}", path);
                }
                if (item.Element.Link_1 != null && item.Element.Link_1.Contains("http:"))
                {
                    string path = item.Element.Link_1.Remove(0, 5);
                    item.Element.Link_1 = string.Format("https:{0}", path);
                }
                if (item.Element.Link_2 != null && item.Element.Link_2.Contains("http:"))
                {
                    string path = item.Element.Link_2.Remove(0, 5);
                    item.Element.Link_2 = string.Format("https:{0}", path);
                }

            }

            return feed;
        }



        public RssFeedViewModel CopyFoxNewsRssPoliticalFeedToViewModel()
        {
            RssManagement rssManager = new RssManagement();
            return CopyRssFeedToViewModel(rssManager.GetFoxNewsRssPoliticalFeed());
        }



        public RssFeedViewModel CopyCnbcRssPoliticalFeedToViewModel()
        {
            RssManagement rssManager = new RssManagement();
            return CopyRssFeedToViewModel(rssManager.GetCnbcRSSPoliticalFeed());
        }



        public RssFeedViewModel CopyCnnRssPoliticalFeedToViewModel()
        {
            RssManagement rssManager = new RssManagement();
            return CopyRssFeedToViewModel(rssManager.GetCnnRssPoliticalFeed());
        }


        public RssFeedViewModel CopyOhioSecretaryOfStateRssFeedToViewModel()
        {
            RssManagement rssManager = new RssManagement();
            return CopyRssFeedToViewModel(rssManager.GetOhioSecretaryOfStateRssFeed());
        }



        public RssFeedViewModel CopyRssFeedToViewModel(ViewModels.Rss.Feed feed)
        {
            return new RssFeedViewModel()
            {
                Channel = CopyRssChannelToViewModel(feed),
                Items = CopyAllItemsToViewModel(feed)
            };

        }



        public Channel CopyRssChannelToViewModel(ViewModels.Rss.Feed feed)
        {
            return new Channel()
                {
                    Element = CopyElementForChannel(feed)
                };
        }



        public Element CopyElementForChannel(ViewModels.Rss.Feed feed)
        {
            return new Element()
            {
                Image = feed.Channel.Element.Image,
                Link_0 = feed.Channel.Element.Link_0,
                Link_1 = feed.Channel.Element.Link_1,
                Link_2 = feed.Channel.Element.Link_2,
                Title = feed.Channel.Element.Title
            };
        }



        public IEnumerable<Item> CopyAllItemsToViewModel(ViewModels.Rss.Feed feed)
        {
            List<Item> items = new List<Item>();

            foreach (var item in feed.Items)
            {
                items.Add(CopyCurrentItemToViewModel(item));
            };

            return items;
        }



        public Item CopyCurrentItemToViewModel(ViewModels.Rss.Item item)
        {
            return new Item()
            {
                Element = CopyElementforItem(item)
            };
        }



        private Element CopyElementforItem(ViewModels.Rss.Item item)
        {
            Element element = new Element()
            {
                Title = item.Element.Title,
                PubDate = item.Element.PubDate,
                Summary = item.Element.Summary,
                Link_0 = item.Element.Link_0,
                Id = item.Element.Id
            };

            return element;
        }






    }
}