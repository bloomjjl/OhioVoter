using OhioVoter.Services;
using OhioVoter.ViewModels.Issue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Controllers
{
    public class IssueController : Controller
    {
        private static string _controllerName = "Issue";



        public ActionResult Index(int? votingDateId, int? countyId, int? communityId, int? issueId)
        {
            // update session with controller info
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);

            // validate supplied ID == integer
            int validVotingDateId = ValidateAndReturnInteger(votingDateId);
            int validCountyId = ValidateAndReturnInteger(countyId);
            int validCommunityId = ValidateAndReturnInteger(communityId);
            int validIssueId = ValidateAndReturnInteger(issueId);

            // get details for view model
            IssueViewModel viewModel = new IssueViewModel()
            {
                ControllerName = _controllerName,
                IssueLookUpViewModel = GetIssueLookUpViewModel(validVotingDateId, validCountyId, validCommunityId),
                IssueDisplayViewModel = GetIssueDisplayViewModel(validVotingDateId, validCountyId, validCommunityId),
                IssueDetailViewModel = GetIssueDetailViewModel(validVotingDateId, validCountyId, validCommunityId, validIssueId)
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LookUp(int? selectedVotingDateId, int? selectedCountyId, int? selectedCommunityId)
        {
            // validate supplied ID == integer
            int votingDateId = ValidateAndReturnInteger(selectedVotingDateId);
            int countyId = ValidateAndReturnInteger(selectedCountyId);
            int communityId = ValidateAndReturnInteger(selectedCommunityId);

            /*
            // update session with controller info
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);

            // get details for view model
            IssueViewModel viewModel = new IssueViewModel()
            {
                ControllerName = _controllerName,
                IssueLookUpViewModel = GetIssueLookUpViewModel(votingDateId, countyId, communityId),
                IssueDisplayViewModel = GetIssueDisplayViewModel(votingDateId, countyId, communityId)
            };
            */

            return RedirectToAction("Index", "Issue", new { votingDateId = votingDateId, countyId = countyId, communityId = communityId });
        }



        [ChildActionOnly]
        public ActionResult DisplayIssueInformation(IssueViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Content("");
            }

            if (viewModel.IssueDetailViewModel != null && viewModel.IssueDetailViewModel.IssueId != 0)
            {
                return PartialView("_IssueDetails", viewModel.IssueDetailViewModel);
            }

            return PartialView("_IssueList", viewModel.IssueDisplayViewModel);
        }



        public IssueDetailViewModel GetIssueDetailViewModel(int votingDateId, int countyId, int communityId, int issueId)
        {
            // supplied issueId is not valid
            if (issueId == 0)
            {
                return new IssueDetailViewModel();
            }

            Models.ElectionIssue issue = GetIssueDetailsForIssueIdFromDatabase(issueId);

            if (issue == null)
            {
                return new IssueDetailViewModel();
            }

            return new IssueDetailViewModel()
            {
                DateId = votingDateId,
                CountyId = countyId,
                CommunityId = communityId,
                IssueId = issueId,
                IssueTitle = issue.IssueTitle,
                IssueRequirement = issue.IssueRequirement,
                IssueDetails = issue.IssueDetails,
                IssueFullTextLink = issue.IssueFullTextLink,
                IssueOption1 = issue.IssueOption1,
                IssueOption2 = issue.IssueOption2
            };
        }




        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }



        public IssueDisplayViewModel GetIssueDisplayViewModel(int votingDateId, int countyId, int communityId)
        {
            // validate supplied ID == integer
            //int votingDateId = ValidateAndReturnInteger(selectedVotingDateId);
            //int countyId = ValidateAndReturnInteger(selectedCountyId);
            //int communityId = ValidateAndReturnInteger(selectedCommunityId);

            // must have before displaying
            if (votingDateId == 0 || countyId == 0)
            {
                return new IssueDisplayViewModel()
                {
                    Issues = new List<Issue>()
                };
            }

            return new IssueDisplayViewModel()
            {
                DateId = votingDateId,
                CountyId = countyId,
                CommunityId = communityId,
                Issues = GetListOfIssuesForIssueDisplayViewModel(votingDateId, countyId, communityId)
            };
        }



        public Models.ElectionIssue GetIssueDetailsForIssueIdFromDatabase(int issueId)
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                Models.ElectionIssue dboIssue = context.ElectionIssues.FirstOrDefault(x => x.ElectionIssueId == issueId);

                if (dboIssue == null)
                {
                    return new Models.ElectionIssue();
                }

                return dboIssue;
            }
        }



        public List<Issue> GetListOfIssuesForIssueDisplayViewModel(int votingDateId, int countyId, int communityId)
        {
            // Get list of issues by date && county
            List<Models.ElectionIssue> dboIssues = GetListOfIssuesForDateAndCountyFromDatabase(votingDateId, countyId);

            // narrow list for community
            if (communityId > 0)
            {
                List<Models.ElectionPrecinct> dboPrecincts = GetListOfPrecinctsForCommunityFromDatabase(communityId);
                List<Models.ElectionIssue> dboPrecinctIssues = GetListOfIssuesForPrecinctFromDatabase(dboIssues, dboPrecincts);
                return GetFormatedListOfIssuesForIssueDisplayViewModel(dboPrecinctIssues);
            }

            return GetFormatedListOfIssuesForIssueDisplayViewModel(dboIssues);
        }



        public List<Issue> GetFormatedListOfIssuesForIssueDisplayViewModel(List<Models.ElectionIssue> dboIssues)
        {
            List<Issue> issues = new List<Issue>();

            for (int i = 0; i < dboIssues.Count; i++)
            {
                issues.Add(
                    new Issue()
                    {
                        Id = dboIssues[i].ElectionIssueId,
                        Title = dboIssues[i].IssueTitle,
                        Requirement = dboIssues[i].IssueRequirement,
                        Details = dboIssues[i].IssueDetails,
                        FullTextLink = dboIssues[i].IssueFullTextLink,
                        Option1 = dboIssues[i].IssueOption1,
                        Option2 = dboIssues[i].IssueOption2
                    }
                );
            }

            return issues;
        }


        public List<Models.ElectionIssue> GetListOfIssuesForPrecinctFromDatabase(List<Models.ElectionIssue> dboIssues, List<Models.ElectionPrecinct> dboPrecincts)
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                // returning list of issues for supplied precincts
                List<Models.ElectionIssue> precinctIssues = new List<Models.ElectionIssue>();

                // loop through supplied issues
                for (int i = 0; i < dboIssues.Count; i++)
                {
                    // store current issue id
                    int issueId = dboIssues[i].ElectionIssueId;

                    // get list of all matching issues in IssuePrecincts table
                    List<Models.ElectionIssuePrecinct> dboCurrentIssueForPrecincts = context.ElectionIssuePrecincts.Where(x => x.ElectionPrecinctId > 0)
                                                                                                                   .Where(x => x.ElectionIssueId == issueId)
                                                                                                                   .ToList();
                    if (issueId == 49){
                        issueId = 49;}
                    if (issueId == 52){
                        issueId = 52;}

                    // loop through supplied precincts
                    for (int j = 0; j < dboPrecincts.Count; j++)
                    {
                        // store current precinct id
                        int precinctId = dboPrecincts[j].ElectionPrecinctId;

                        // search IssuePrecincts table for current precinct id
                        List<Models.ElectionIssuePrecinct> dboIssueForSuppliedPrecincts = dboCurrentIssueForPrecincts.Where(x => x.ElectionPrecinctId == precinctId).ToList();

                        //Models.ElectionIssuePrecinct dboIssueForPrecinct = context.ElectionIssuePrecincts.Find(issueId, precinctId);

                        // if found then store issue information and move on to next issue id
                        if (dboIssueForSuppliedPrecincts != null && dboIssueForSuppliedPrecincts.Count != 0)
                        {
                            precinctIssues.Add(dboIssues[i]);
                            j = dboPrecincts.Count;
                        }
                    }
                }

                return precinctIssues;
            }
        }



        public List<Models.ElectionPrecinct> GetListOfPrecinctsForCommunityFromDatabase(int communityId)
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.ElectionPrecinct> dboPrecincts = context.ElectionPrecincts.Where(x => x.LocalId == communityId).ToList();

                if (dboPrecincts == null)
                {
                    return new List<Models.ElectionPrecinct>();
                }

                return dboPrecincts;
            }
        }



        public List<Models.ElectionIssue> GetListOfIssuesForDateAndCountyFromDatabase(int votingDateId, int countyId)
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.ElectionIssue> dboIssues = context.ElectionIssues.Where(x => x.ElectionVotingDateId == votingDateId).ToList();
                List<Models.ElectionIssue> dboCountyIssues = new List<Models.ElectionIssue>();

                for(int i = 0; i < dboIssues.Count; i++)
                {
                    if(dboIssues[i].OhioCountyId == countyId)
                    {
                        dboCountyIssues.Add(dboIssues[i]);
                    }
                }

                return dboCountyIssues;
            }
        }



        public IssueLookUpViewModel GetIssueLookUpViewModel(int selectedVotingDateId, int selectedCountyId, int selectedCommunityId)
        {
            // validate supplied ID == integer
            //int votingDateId = ValidateAndReturnInteger(selectedVotingDateId);
            //int countyId = ValidateAndReturnInteger(selectedCountyId);
            //int communityId = ValidateAndReturnInteger(selectedCommunityId);

            // verify selectd community id is valid for county
            if (!ValidateCommunityLocatedInCountyfromDatabase(selectedCountyId, selectedCommunityId))
            {
                selectedCommunityId = 0;
            }

            return new IssueLookUpViewModel()
            {
                ControllerName = _controllerName,
                SelectedVotingDateId = selectedVotingDateId.ToString(),
                VotingDates = GetDropDownListOfVotingDates(),
                SelectedCountyId = selectedCountyId.ToString(),
                CountyNames = GetDropDownListOfCounties(),
                SelectedCommunityId = selectedCommunityId.ToString(),
                CommunityNames = GetDropDownListOfCommunities(selectedCountyId)
            };
        }


        public bool ValidateCommunityLocatedInCountyfromDatabase(int countyId, int communityId)
        {
            bool isValid = false;

            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                Models.OhioLocal dboCommunity = context.OhioLocals.Find(communityId);

                if (dboCommunity != null && dboCommunity.OhioCountyId == countyId)
                {
                    isValid = true;
                }
            }

            return isValid;
        }




        /// <summary>
        /// Validate parameter is an integer
        /// </summary>
        /// <param name="id"></param>
        /// <returns>value if integer, or zero if not valid integer</returns>
        public int ValidateAndReturnInteger(int? id)
        {
            int intId;

            if (id == null)
            {// no value provided
                // default to zero.
                return 0;
            }
            else if (int.TryParse(id.ToString(), out intId))
            {// value is an integer
                // return supplied integer
                return int.Parse(id.ToString());
            }
            else
            {// value provided is not an integer
                // default to zero
                return 0;
            }
        }



        public List<SelectListItem> GetDropDownListOfVotingDates()
        {
            List<Models.ElectionVotingDate> dboDates = GetVotingDateListFromDatabase();
            List<SelectListItem> dates = new List<SelectListItem>();

            for (int i = 0; i < dboDates.Count; i++)
            {
                dates.Add(new SelectListItem()
                {
                    Value = dboDates[i].ElectionVotingDateId.ToString(),
                    Text = dboDates[i].Date.ToShortDateString()
                });
            }

            return dates;
        }



        public List<SelectListItem> GetDropDownListOfCounties()
        {
            List<Models.OhioCounty> dboCounties = GetCountyListFromDatabase();
            List<SelectListItem> counties = new List<SelectListItem>();

            for (int i = 0; i < dboCounties.Count; i++)
            {
                counties.Add(new SelectListItem()
                {
                    Value = dboCounties[i].OhioCountyId.ToString(),
                    Text = dboCounties[i].CountyName
                });
            }

            return counties;
        }



        public List<SelectListItem> GetDropDownListOfCommunities(int countyId)
        {
            // must have a county
            if (countyId == 0)
            {
                return new List<SelectListItem>();
            }

            List<Models.OhioLocal> dboCommunities = GetCommunityListFromDatabase(countyId);
            List<SelectListItem> communities = new List<SelectListItem>();

            for (int i = 0; i < dboCommunities.Count; i++)
            {
                communities.Add(new SelectListItem()
                {
                    Value = dboCommunities[i].OhioLocalId.ToString(),
                    Text = dboCommunities[i].LocalName
                });
            }

            return communities;
        }



        public List<Models.ElectionVotingDate> GetVotingDateListFromDatabase()
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.ElectionVotingDate> dboDates = context.ElectionVotingDates.Where(x => x.Active == true).ToList();

                if (dboDates == null)
                {
                    return new List<Models.ElectionVotingDate>();
                }

                return dboDates;
            }
        }



        public List<Models.OhioCounty> GetCountyListFromDatabase()
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.OhioCounty> dboCounties = context.OhioCounties.ToList();

                if (dboCounties == null)
                {
                    return new List<Models.OhioCounty>();
                }

                return dboCounties;
            }
        }



        public List<Models.OhioLocal> GetCommunityListFromDatabase(int countyId)
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.OhioLocal> dboCommunities = context.OhioLocals.Where(x => x.OhioCountyId == countyId).ToList();

                if (dboCommunities == null)
                {
                    return new List<Models.OhioLocal>();
                }

                return dboCommunities;
            }
        }




    }
}