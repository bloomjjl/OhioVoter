using OhioVoter.Models;
using OhioVoter.Services;
using OhioVoter.ViewModels.Ballot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Controllers
{
    public class BallotController : Controller
    {
        private static string _controllerName = "Ballot";



        public ActionResult Index()
        {
            // update session with controller info
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);

            // get details for view model
            BallotViewModel viewModel = new BallotViewModel()
            {
                ControllerName = _controllerName
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BallotViewModel ballotVM)
        {
            if (!ModelState.IsValid) { return View("Index"); }

            return View("Index");
        }



        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }



        [ChildActionOnly]
        public ActionResult DisplayBallotInformation()
        {
            // get election voting date from View (id / date)
            int votingDateId = GetVotingDateForNextElectionFromDatabase();
            if (votingDateId == 0) { return Content(""); }

            // make sure user has provided location
            BallotLocationViewModel ballotLocationVM = GetLocationInformationFromSessionForVoter();
            ballotLocationVM.ControllerName = _controllerName;

            bool hasLocation = ValidateLocationInformationForVoter(ballotLocationVM);
            if (hasLocation == false)
            {
                // location not available -- get location information 
                ViewModels.Location.VoterLocationViewModel voterLocationVM = new ViewModels.Location.VoterLocationViewModel(ballotLocationVM);
                return PartialView("_VoterLocationForm", voterLocationVM);
            }

            // get view model
            BallotViewModel ballotVM = new BallotViewModel()
            {
                BallotVoterViewModel = GetVoterInformationforBallotLocationFromDatabase(ballotLocationVM)
            };

            // get voter information from database
            ballotVM.BallotVoterViewModel = GetVoterInformationforBallotLocationFromDatabase(ballotLocationVM);
            ballotVM.BallotVoterViewModel.ElectionVotingDateId = votingDateId;

            // get office information for voter from databse
            ballotVM.BallotOfficeViewModel = GetListOfOfficesForBallotVoterFromDatabase(ballotVM.BallotVoterViewModel);

            // get candidate information for each office from database
            ballotVM.BallotOfficeViewModel = GetListOfCandidatesForBallotOfficeFromDatabase(ballotVM.BallotOfficeViewModel);

            // get runningmate information from database
            ballotVM.BallotOfficeViewModel = GetRunningMateInformationForBallotOfficeFromDatabase(ballotVM.BallotOfficeViewModel);

            // get candidate images from VoteSmart
            //ballotVM.BallotOfficeViewModel = GetCandidateImagesForBallotOfficeFromDatabase(ballotVM.BallotOfficeViewModel);

            // get issue information for voter from database
            ballotVM.BallotIssueViewModel = GetListOfIssuesForBallotVoterFromDatabase(ballotVM.BallotVoterViewModel);

            // display ballot
            return PartialView("_Ballot", ballotVM);
        }



        public int GetVotingDateForNextElectionFromDatabase()
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.ElectionVotingDate> votingDateDTO = context.ElectionVotingDates.OrderBy(x => x.Date)
                                                                                           .Where(x => x.Active == true)
                                                                                           .ToList();
                if (votingDateDTO == null) { return 0; }

                return votingDateDTO[0].Id;
            }
        }



        public BallotLocationViewModel GetLocationInformationFromSessionForVoter()
        {
            // get voter location from session
            Services.SessionExtensions session = new Services.SessionExtensions();
            ViewModels.Location.VoterLocationViewModel locationVM = session.GetVoterLocationFromSession();
            return new BallotLocationViewModel(locationVM);
        }



        public bool ValidateLocationInformationForVoter(BallotLocationViewModel ballotLocationVM)
        {
            // validate voter location
            Controllers.LocationController location = new Controllers.LocationController();
            ViewModels.Location.VoterLocationViewModel locationVM = new ViewModels.Location.VoterLocationViewModel(ballotLocationVM);

            return location.ValidateVoterLocation(locationVM);
        }



        public BallotVoterViewModel GetVoterInformationforBallotLocationFromDatabase(BallotLocationViewModel ballotLocationVM)
        {
            // convert string values to integer
            int intStreetNumber = GetIntegerFromStringValue(ballotLocationVM.StreetNumber);
            int intZipCode = GetIntegerFromStringValue(ballotLocationVM.ZipCode);

            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                Models.HamiltonOhioVoter hamiltonOhioVoterDTO = context.HamiltonOhioVoters.Where(x => x.AddressNumber == intStreetNumber)
                                                                                            .Where(x => x.AddressStreetAndSuffix == ballotLocationVM.StreetName)
                                                                                            .Where(x => x.AddressCityName == ballotLocationVM.City)
                                                                                            .FirstOrDefault(x => x.AddressZip == intZipCode);

                if (hamiltonOhioVoterDTO == null) { return new BallotVoterViewModel(); }

                return new BallotVoterViewModel(hamiltonOhioVoterDTO);
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



        public List<BallotOfficeViewModel> GetListOfOfficesForBallotVoterFromDatabase(BallotVoterViewModel ballotVoterVM)
        {
            List<BallotOfficeViewModel> officeVM = new List<BallotOfficeViewModel>();

            // get list of all offices for current election date
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.ElectionOffice> dbOffices = context.ElectionOffices.Where(x => x.ElectionVotingDateId == ballotVoterVM.ElectionVotingDateId)
                                                                                       .Distinct()
                                                                                       .OrderBy(x => x.Office.OfficeSortOrder)
                                                                                       .ToList();

                if (dbOffices == null) { return officeVM; }

                foreach (var officeDTO in dbOffices)
                {
                    // ** NATIONAL LEVEL OFFICES **
                    if (officeDTO.Office.OfficeLevel == "National")
                    {
                        officeVM.Add(new BallotOfficeViewModel(officeDTO));
                    }
                    // ** STATE LEVEL OFFICES **
                    else if (officeDTO.Office.OfficeLevel == "State")
                    {
                        officeVM.Add(new BallotOfficeViewModel(officeDTO));
                    }
                    // ** DISTRICT LEVEL OFFICES **
                    else if (officeDTO.Office.OfficeLevel == "District")
                    {
                        if (officeDTO.Office.DistrictCode == ballotVoterVM.CongressOfficeCode)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                        else if (officeDTO.Office.DistrictCode == ballotVoterVM.SenateOfficeCode)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                        else if (officeDTO.Office.DistrictCode == ballotVoterVM.HouseOfficeCode)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                        else if (officeDTO.Office.DistrictCode == ballotVoterVM.SchoolOfficeCode)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                        else if (officeDTO.Office.DistrictCode == ballotVoterVM.CountySchoolOfficeCode)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                        else if (officeDTO.Office.DistrictCode == ballotVoterVM.VocationalSchoolOfficeCode)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                        else if (officeDTO.Office.DistrictCode == ballotVoterVM.CourtOfAppeasOfficeCode)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                    }
                    // ** COUNTY LEVEL OFFICES **
                    else if (officeDTO.Office.OfficeLevel == "County")
                    {
                        if (officeDTO.OhioLocal.OhioCountyId == ballotVoterVM.CountyId)
                        {
                            officeVM.Add(new BallotOfficeViewModel(officeDTO));
                        }
                    }
                }

                return officeVM;
            }
        }



        public List<BallotOfficeViewModel> GetListOfCandidatesForBallotOfficeFromDatabase (List<BallotOfficeViewModel> ballotOfficesVM)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                // get office ID
                for (int i = 0; i < ballotOfficesVM.Count(); i++)
                {
                    // create candidate object
                    //ballotOfficesVM[i].BallotCandidatesViewModel = new 

                    int currentOfficeId = ballotOfficesVM[i].OfficeId;

                    // do not display offices for runningmates
                    if (currentOfficeId == 2)
                    {
                        ballotOfficesVM.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {

                        // get election candidate/runningmate for office ID
                        List<Models.ElectionCandidate> dbCandidates = context.ElectionCandidates.Where(x => x.ElectionOfficeId == currentOfficeId)
                                                                                                .OrderBy(x => x.Candidate.FirstName)
                                                                                                .OrderBy(x => x.Candidate.LastName)
                                                                                                .ToList();

                        List<BallotCandidateViewModel> collectionListedCandidateVM = new List<BallotCandidateViewModel>();
                        List<BallotCandidateViewModel> collectionWriteInCandidateVM = new List<BallotCandidateViewModel>();

                        // group candidates by how they will appear on the ballot (Listed or WriteIn)
                        foreach (var candidateDTO in dbCandidates)
                        {
                            if (candidateDTO.CertifiedCandidate.Id == "L")
                            {
                                collectionListedCandidateVM.Add(new BallotCandidateViewModel(candidateDTO));
                            }
                            else
                            {
                                collectionWriteInCandidateVM.Add(new BallotCandidateViewModel(candidateDTO));
                            }
                        }

                        ballotOfficesVM[i].BallotListedCandidatesViewModel = collectionListedCandidateVM;
                        ballotOfficesVM[i].BallotwriteInCandidatesViewModel = collectionWriteInCandidateVM;
                    }
                }
            }

            return ballotOfficesVM;
        }



        public List<BallotOfficeViewModel> GetRunningMateInformationForBallotOfficeFromDatabase(List<BallotOfficeViewModel> ballotOfficesVM)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.Candidate> dbCandidates = context.Candidates.ToList();

                for (int i = 0; i < ballotOfficesVM.Count; i++)
                {
                    // find Listed Candidates where office = President
                    if (ballotOfficesVM[i].OfficeId == 1)
                    {
                        for (int j = 0; j < ballotOfficesVM[i].BallotListedCandidatesViewModel.Count; j++)
                        {
                            // get VP information
                            Models.Candidate candidateDTO = dbCandidates.FirstOrDefault(x => x.Id == ballotOfficesVM[i].BallotListedCandidatesViewModel[j].RunningMateId);

                            ballotOfficesVM[i].BallotListedCandidatesViewModel[j] = new BallotCandidateViewModel(ballotOfficesVM[i].BallotListedCandidatesViewModel[j], candidateDTO);
                        }
                    }

                    // find WriteIn Candidates where office = President
                    if (ballotOfficesVM[i].OfficeId == 1)
                    {
                        for (int j = 0; j < ballotOfficesVM[i].BallotwriteInCandidatesViewModel.Count; j++)
                        {
                            // get VP information
                            Models.Candidate candidateDTO = dbCandidates.FirstOrDefault(x => x.Id == ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j].RunningMateId);

                            ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j] = new BallotCandidateViewModel(ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j], candidateDTO);
                        }
                    }
                }
            }

            return ballotOfficesVM;
        }



        public List<BallotOfficeViewModel>  GetCandidateImagesForBallotOfficeFromDatabase(List<BallotOfficeViewModel> ballotOfficesVM)
        {
            VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();
            
            for (int i = 0; i < ballotOfficesVM.Count(); i++)
            {
                for (int j = 0; j < ballotOfficesVM[i].BallotListedCandidatesViewModel.Count(); j++)
                {
                    // get listed candidates
                    ballotOfficesVM[i].BallotListedCandidatesViewModel[j].VoteSmartCandidateImageUrl = voteSmart.GetVoteSmartCandidateImageUrlFromSuppliedVoteSmartCandidateId(ballotOfficesVM[i].BallotListedCandidatesViewModel[j].VoteSmartCandidateId);
                    // get listed runningmates
                    ballotOfficesVM[i].BallotListedCandidatesViewModel[j].VoteSmartRunningMateId = voteSmart.GetVoteSmartCandidateImageUrlFromSuppliedVoteSmartCandidateId(ballotOfficesVM[i].BallotListedCandidatesViewModel[j].VoteSmartRunningMateId);
                }
                for (int j = 0; j < ballotOfficesVM[i].BallotwriteInCandidatesViewModel.Count(); j++)
                {
                    // get writein candidates
                    ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j].VoteSmartCandidateImageUrl = voteSmart.GetVoteSmartCandidateImageUrlFromSuppliedVoteSmartCandidateId(ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j].VoteSmartCandidateId);
                    // get writein runningmates0
                    ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j].VoteSmartRunningMateImageUrl = voteSmart.GetVoteSmartCandidateImageUrlFromSuppliedVoteSmartCandidateId(ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j].VoteSmartRunningMateId);
                }
            }

            return ballotOfficesVM;
        }



        public List<BallotIssueViewModel> GetListOfIssuesForBallotVoterFromDatabase(BallotVoterViewModel ballotVoterVM)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.ElectionIssuePrecinct> dbIssuesPrecincts = context.ElectionIssuePrecincts.Where(x => x.OhioPrecinctId == ballotVoterVM.OhioPrecinctId).ToList();

                if (dbIssuesPrecincts == null) { return new List<BallotIssueViewModel>(); }

                List<BallotIssueViewModel> issueVM = new List<BallotIssueViewModel>();

                foreach (var issueDTO in dbIssuesPrecincts)
                {
                    issueVM.Add(new BallotIssueViewModel(issueDTO));
                }

                return issueVM;
            }
        }




    }
}