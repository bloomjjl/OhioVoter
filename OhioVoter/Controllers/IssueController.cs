using OhioVoter.Models;
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
            int intVotingDateId = ValidateAndReturnInteger(votingDateId);
            int intCountyId = ValidateAndReturnInteger(countyId);
            int intCommunityId = ValidateAndReturnInteger(communityId);
            int intIssueId = ValidateAndReturnInteger(issueId);

            if (intVotingDateId == 0)
            {
                intVotingDateId = GetOldestActiveVotingDateId();
            }

            // get details for view model
            IssueViewModel viewModel = new IssueViewModel()
            {
                ControllerName = _controllerName,
                IssueLookUpViewModel = GetIssueLookUpViewModel(intVotingDateId, intCountyId, intCommunityId),
                IssueDisplayViewModel = GetIssueDisplayViewModel(intVotingDateId, intCountyId, intCommunityId),
                IssueDetailViewModel = GetIssueDetailViewModel(intVotingDateId, intCountyId, intCommunityId, intIssueId)
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



        public ActionResult ViewIssueFullText(string fileName)
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
                Models.ElectionIssue dboIssue = context.ElectionIssues.FirstOrDefault(x => x.Id == issueId);

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
                List<Models.OhioPrecinct> dboPrecincts = GetListOfPrecinctsForCommunityFromDatabase(communityId);
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
                        Id = dboIssues[i].Id,
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


        public List<Models.ElectionIssue> GetListOfIssuesForPrecinctFromDatabase(List<Models.ElectionIssue> dboIssues, List<Models.OhioPrecinct> dboPrecincts)
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                // returning list of issues for supplied precincts
                List<Models.ElectionIssue> precinctIssues = new List<Models.ElectionIssue>();

                // loop through supplied issues
                for (int i = 0; i < dboIssues.Count; i++)
                {
                    // store current issue id
                    int issueId = dboIssues[i].Id;

                    // get list of all matching issues in IssuePrecincts table
                    List<Models.ElectionIssuePrecinct> dboCurrentIssueForPrecincts = context.ElectionIssuePrecincts.Where(x => x.OhioPrecinctId > 0)
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
                        int precinctId = dboPrecincts[j].Id;

                        // search IssuePrecincts table for current precinct id
                        List<Models.ElectionIssuePrecinct> dboIssueForSuppliedPrecincts = dboCurrentIssueForPrecincts.Where(x => x.OhioPrecinctId == precinctId).ToList();

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



        public List<Models.OhioPrecinct> GetListOfPrecinctsForCommunityFromDatabase(int communityId)
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.OhioPrecinct> dboPrecincts = context.OhioPrecincts.Where(x => x.OhioLocalId == communityId).ToList();

                if (dboPrecincts == null)
                {
                    return new List<Models.OhioPrecinct>();
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



        public IssueLookUpViewModel GetIssueLookUpViewModel(int votingDateId, int countyId, int communityId)
        {
            // validate supplied ID == integer
            int intVotingDateId = ValidateAndReturnInteger(votingDateId);
            int intCountyId = ValidateAndReturnInteger(countyId);
            int intCommunityId = ValidateAndReturnInteger(communityId);

            // verify votingDate 
            string votingDate = GetVotingDateForSuppliedVotingDateId(intVotingDateId);
            if (string.IsNullOrEmpty(votingDate))
            {
                intVotingDateId = GetOldestActiveVotingDateId();
                votingDate = GetVotingDateForSuppliedVotingDateId(intVotingDateId);
            }

            // verify selected county
            if (!ValidateCountyLocatedInOhio(intCountyId))
            {
                intCountyId = 0;
            }

            // verify selected community id is valid for county
            if (!ValidateCommunityLocatedInCountyfromDatabase(intCountyId, intCommunityId))
            {
                intCommunityId = 0;
            }

            return new IssueLookUpViewModel()
            {
                ControllerName = _controllerName,
                VotingDateId = intVotingDateId,
                VotingDate = votingDate,
                SelectedCountyId = intCountyId.ToString(),
                CountyNames = GetDropDownListOfCountiesFromDatabase(),
                SelectedCommunityId = intCommunityId.ToString(),
                CommunityNames = GetDropDownListOfCommunities(intCountyId)
            };
        }


        private int GetOldestActiveVotingDateId()
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List <Models.ElectionVotingDate> dbDate = context.ElectionVotingDates.Where(x => x.Active == true).ToList();

                if (dbDate == null) { return 0; }

                int oldestDateId = dbDate[0].Id;
                DateTime votingDate = dbDate[0].Date;


                foreach (var dateDTO in dbDate)
                {
                    if (dateDTO.Date < votingDate )
                    {
                        oldestDateId = dateDTO.Id;
                        votingDate = dateDTO.Date;
                    }
                }

                return oldestDateId;
            }
        }


        public string GetVotingDateForSuppliedVotingDateId(int dateId)
        {
            // validate paramenter
            int intDateId = ValidateAndReturnInteger(dateId);

            // get date from database
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                ElectionVotingDate dateDTO = context.ElectionVotingDates.FirstOrDefault(x => x.Id == intDateId);

                if (dateDTO == null) { return string.Empty; }

                return dateDTO.Date.Date.ToShortDateString();
            }
        }



        public bool ValidateCountyLocatedInOhio(int countyId)
        {
            // validate paramenter
            int intCountyId = ValidateAndReturnInteger(countyId);

            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                OhioCounty countyDTO = context.OhioCounties.FirstOrDefault(x => x.Id == intCountyId);

                if (countyDTO != null) { return true; }

                return false;
            }
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
                    Value = dboDates[i].Id.ToString(),
                    Text = dboDates[i].Date.ToShortDateString()
                });
            }

            return dates;
        }



        public List<SelectListItem> GetDropDownListOfCountiesFromDatabase()
        {
            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                List<Models.OhioCounty> dboCounties = context.OhioCounties.ToList();

                if (dboCounties == null) { return new List<SelectListItem>(); }

                List<SelectListItem> counties = new List<SelectListItem>();

                for (int i = 0; i < dboCounties.Count; i++)
                {
                    counties.Add(new SelectListItem()
                    {
                        Value = dboCounties[i].Id.ToString(),
                        Text = dboCounties[i].Name.ToUpper()
                    });
                }

                return counties;
            }
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
                    Value = dboCommunities[i].Id.ToString(),
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