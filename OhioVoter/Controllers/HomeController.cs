using OhioVoter.Services;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.Home;
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
        /// <summary>
        /// get the information to display on page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            LocationController location = new LocationController();

            string controllerName = "Home";
            UpdateSessionWithNewControllerNameForSideBar(controllerName);

            HomeViewModel viewModel = new HomeViewModel()
            {
                ControllerName = controllerName,
                Calendar = GetCalendarViewModel(),
                Poll = GetPollResultsViewModel(),
                RssFeeds = GetRssFeedViewModel()
            };

            return View(viewModel);
        }




        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }



        // ********************************************
        // Calendar for upcoming election dates
        // ********************************************

        // TODO: set up link to allow users to sign up for email reminders for upcoming election dates

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
                ElectionDates = GetListOfUpcomingElectionDates(startDate, endDate)
            };
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



        public IEnumerable<ElectionDate> CopyElectionDatesToViewModel(List<Models.ElectionDate> electionDates)
        {
            List<ElectionDate> dates = new List<ElectionDate>();

            for (int i = 0; i < electionDates.Count; i++)
            {
                dates.Add(new ElectionDate()
                {
                    DateId = electionDates[i].ElectionDateId,
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

        // TODO: set up database and retrieve polling data
        // TODO: allow user to change the office if location provided

        /// <summary>
        /// get polling information for users that have filled out the ballot from database
        /// sort and store the polling results to display on page
        /// </summary>
        /// <returns></returns>
        private PollViewModel GetPollResultsViewModel()
        {
            PollViewModel poll = new PollViewModel()
            {
                ElectionDate = Convert.ToDateTime("11/08/2016"),
                OfficeName = "President",
                CandidateVotes = GetCandidateVotesFromBallot()
            };

            // sort candidates by highest to lowest votes received and display if greater than 0.0
            int totalVotesForOffice = GetTotalVotesForOfficeFromBallot(poll.CandidateVotes);
            poll.CandidateVotes = GetPercentageOfVotesEachCandidateReceivedFromBallot(poll.CandidateVotes, totalVotesForOffice);
            poll.CandidateVotes = RemoveCandidatesWithZeroVotesFromList(poll.CandidateVotes);
            poll.CandidateVotes = poll.CandidateVotes.OrderByDescending(x => x.VoteCount).ToList();

            return poll;
        }


        // ***************
        // moch database
        // ***************
        private IEnumerable<CandidateVote> GetCandidateVotesFromBallot()
        {
            List<CandidateVote> candidateResults = new List<CandidateVote>()
            {
                new CandidateVote() { Candidate = "Hillary Clinton", CoCandidate = "", Party = "Democrat", PartyColor = "Blue", VoteCount = 2394164,  ImageUrl = "http://static.votesmart.org/canphoto/55463.jpg" },
                new CandidateVote() { Candidate = "Gary Johnson", CoCandidate = "", Party = "Libertarian", PartyColor = "Yellow", VoteCount = 174498,  ImageUrl = "http://static.votesmart.org/canphoto/22377.jpg"  },
                new CandidateVote() { Candidate = "Jill Stein", CoCandidate = "", Party = "Green", PartyColor = "Green", VoteCount = 46271,  ImageUrl = "http://static.votesmart.org/canphoto/35775.jpg"  },
                new CandidateVote() { Candidate = "Donald Trump", CoCandidate = "", Party = "Republican", PartyColor = "Red", VoteCount = 2841005, ImageUrl = "http://static.votesmart.org/canphoto/15723.jpg" },
                new CandidateVote() { Candidate = "Richard Duncan", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 24235,  ImageUrl = "http://static.votesmart.org/canphoto/65939.jpg"  },
                new CandidateVote() { Candidate = "Evan McMullin", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 12574,  ImageUrl = "http://static.votesmart.org/canphoto/174905.jpg"  },
                new CandidateVote() { Candidate = "Darrell Castle", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 1887,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Ben Hartnell", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 589,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Michael Maturen", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 552,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Tom Hoefling", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 268,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Chris Keniston", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 114,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Laurence Kotlikoff", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 90,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Joe Schriner", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 62,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Mike Smith", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 62,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Josiah Stroh", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 30,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Monica Moorehead", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 19,  ImageUrl = ""  },
                new CandidateVote() { Candidate = "Joseph Maldonado", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 18,  ImageUrl = "" },
                new CandidateVote() { Candidate = "Barry Kirschner", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 15,  ImageUrl = "" },
                new CandidateVote() { Candidate = "James Bell", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 9,  ImageUrl = "" },
                new CandidateVote() { Candidate = "Bruce Jaynes", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 8,  ImageUrl = "" },
                new CandidateVote() { Candidate = "Michael Bickelmeyer", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 6,  ImageUrl = "" },
                new CandidateVote() { Candidate = "Douglas Thomson", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 6,  ImageUrl = "" },
                new CandidateVote() { Candidate = "Cherunda Fox", CoCandidate = "", Party ="", PartyColor = "Gray", VoteCount = 5,  ImageUrl = "" }
            };

            return candidateResults;
        }



        /// <summary>
        /// get the total number of votes each candidate received
        /// </summary>
        /// <param name="candidateVotes"></param>
        /// <returns></returns>
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



        /// <summary>
        /// store the percentage of total votes each candidate received
        /// </summary>
        /// <param name="candidateVotes"></param>
        /// <param name="totalVotes"></param>
        /// <returns></returns>
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



        /// <summary>
        /// only store the candidates who have an average of the total votes greater than 0.0
        /// </summary>
        /// <param name="candidateVotes"></param>
        /// <returns></returns>
        private IEnumerable<CandidateVote> RemoveCandidatesWithZeroVotesFromList(IEnumerable<CandidateVote> candidateVotes)
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
            return new RssFeeds()
            {
                FoxNewsRssFeed = CopyFoxNewsRssPoliticalFeedToViewModel(),
                CnbcRssFeed = CopyCnbcRssPoliticalFeedToViewModel(),
                CnnRssFeed = CopyCnnRssPoliticalFeedToViewModel()
            };
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