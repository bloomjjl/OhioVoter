﻿using OhioVoter.Models;
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
            // is user in process of filling out ballot?
            if (TempData["Ballot"] != null)
            {
                // get details for view model                
                BallotViewModel ballotVM = (BallotViewModel)TempData["Ballot"];
                ballotVM.ControllerName = _controllerName;

                return View(ballotVM);
            }
            else
            {
                // display new ballot
                // update session with controller info
                UpdateSessionWithNewControllerNameForSideBar(_controllerName);

                // get details for view model
                BallotViewModel viewModel = new BallotViewModel()
                {
                    ControllerName = _controllerName
                };

                // add gender for candidate from votesmart to database
                using (OhioVoterDbContext context = new OhioVoterDbContext())
                {
                    List<Models.Candidate> candidate = context.Candidates.ToList();
                    VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();

                    context.SaveChanges();
                }

                return View(viewModel);
            }
        }


        /*
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DELETE_EmailBallot(BallotViewModel ballotVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Email Address is required");
                return RedirectToAction("Index");
            }

            string ballotEmail;

            // get date for election
            ballotEmail = string.Format("Ballot for Election Date: {0}{1}{2}", ballotVM.SelectedVotingDateId, Environment.NewLine, Environment.NewLine);

            // get each office on ballot
            for (int i = 0; i < ballotVM.BallotOfficeViewModel.Count(); i++)
            {
                // get office name
                ballotEmail = string.Format("{0}. {1}", i, ballotVM.BallotOfficeViewModel[i].OfficeName);

                // get office term if provided
                if (ballotVM.BallotOfficeViewModel[i].OfficeTerm != "")
                {
                    ballotEmail = string.Format("{0} ({1})", ballotEmail, ballotVM.BallotOfficeViewModel[i].OfficeTerm);
                }

                // get selected listed candidate(s)
                for (int j = 0; j < ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel.Count; j++)
                {
                    // check for selected listed candidate
                    if (ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel[j].IsSelected == true)
                    {
                        // add the selected candidate's name to email
                        ballotEmail = string.Format("{0}{1}", ballotEmail, Environment.NewLine, ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel[j].CandidateName);

                        // check for listed runningmate
                        if (ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel[j].RunningMateId > 0)
                        {
                            // add selected runningmate's name to email
                            ballotEmail = string.Format("{0} / {1}", ballotEmail, ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel[j].RunningMateName);
                        }
                        // add new line for next candidate
                        ballotEmail = string.Format("{0}{1}", ballotEmail, Environment.NewLine);
                    }

                    // check for selected write-in candidate
                    if (ballotVM.BallotOfficeViewModel[i].BallotwriteInCandidatesViewModel[j].IsSelected == true)
                    {
                        // add the selected candidate's name to email
                        ballotEmail = string.Format("{0}{1}", ballotEmail, Environment.NewLine, ballotVM.BallotOfficeViewModel[i].BallotwriteInCandidatesViewModel[j].CandidateName);

                        // check for listed runningmate
                        if (ballotVM.BallotOfficeViewModel[i].BallotwriteInCandidatesViewModel[j].RunningMateId > 0)
                        {
                            // add selected runningmate's name to email
                            ballotEmail = string.Format("{0} / {1}", ballotEmail, ballotVM.BallotOfficeViewModel[i].BallotwriteInCandidatesViewModel[j].RunningMateName);
                        }
                        // add new line for next candidate
                        ballotEmail = string.Format("{0}{1}", ballotEmail, Environment.NewLine);
                    }

                    // add extra line between offices
                    ballotEmail = string.Format("{0}{1}", ballotEmail, Environment.NewLine);
                }
            }

            // get each issue on ballot
            for (int i = 0; i < ballotVM.BallotOfficeViewModel.Count(); i++)
            {
                // get issue name
                ballotEmail = string.Format("{0}. {1}", i, ballotVM.BallotIssueViewModel[i].Title);

                // get selected option for issue
                for (int j = 0; j < ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel.Count; j++)
                {
                    // check for selected option
                    * if (ballotVM.BallotIssueViewModel[i].Option1. == true)
                     {

                     }
                     *

                }
            }
            // get issues
            // get selected option
            // email information

            return RedirectToAction("Index");
        }

        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailBallot(BallotViewModel ballotVM)
        {
            TempData["Ballot"] = new BallotViewModel();

            if (!ModelState.IsValid)
            {
                TempData["Ballot"] = ballotVM;
                ModelState.AddModelError("", "Please supply your email address and fill out the ballot");
                return RedirectToAction("Index", "Ballot");
            }

            BallotViewModel copyBallotVM = ballotVM;

            // has an email address been provided
            if (string.IsNullOrWhiteSpace(copyBallotVM.VoterEmailAddress))
            {
                TempData["Ballot"] = ballotVM;

                TempData["EmailBallotMessage"] = "Email Addres is required";
                ModelState.AddModelError("", "Email Address is required");
                return RedirectToAction("Index", copyBallotVM);
            }

            // validate at least one candidate/issue has been selected on ballot
            if (ValidateBallotHasSelections(copyBallotVM))
            {
                // see if user has logged in to connect saved ballot
                string message = "message";
                // validate email provided from user / user account (if logged in)

                // save ballot 

                // display SUCCESS message to user when done
                TempData["Ballot"] = ballotVM;
                TempData["EmailBallotMessage"] = "SUCCESS";
            }
            else
            {
                TempData["Ballot"] = ballotVM;
                TempData["EmailBallotMessage"] = "Ballot has not been filled out";
                ModelState.AddModelError("", "Ballot has not been filled out");
            }

            return RedirectToAction("Index", copyBallotVM);
        }



        private bool ValidateBallotHasSelections(BallotViewModel ballotVM)
        {
            // make sure ballot is provided
            if (ballotVM == null) { return false; }


            // make sure at least one office is on ballot
            if (ballotVM.BallotOfficeViewModel != null)
            {
                // check each office
                for (int i = 0; i < ballotVM.BallotOfficeViewModel.Count(); i++)
                {
                    // check listeted candidates
                    for (int j = 0; j < ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel.Count(); j++)
                    {
                        // stop checking if one is found
                        if (ballotVM.BallotOfficeViewModel[i].BallotListedCandidatesViewModel[j].IsSelected)
                        {
                            return true;
                        }
                    }

                    // check write-in candidates if there are any
                    if (ballotVM.BallotOfficeViewModel[i].BallotwriteInCandidatesViewModel != null)
                    {
                        for (int j = 0; j < ballotVM.BallotOfficeViewModel[i].BallotwriteInCandidatesViewModel.Count(); j++)
                        {
                            // stop checking if one is found
                            if (ballotVM.BallotOfficeViewModel[i].BallotwriteInCandidatesViewModel[j].IsSelected)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // make sure at least one issue is on ballot
            if (ballotVM.BallotIssueViewModel != null)
            {
                // check each issue
                for (int i = 0; i < ballotVM.BallotIssueViewModel.Count(); i++)
                {
                    // stop checking if one is found
                    if (ballotVM.BallotIssueViewModel[i].SelectedValue != null && ballotVM.BallotIssueViewModel[i].SelectedValue != "")
                    {
                        return true;
                    }
                }
            }

            return false;
        }



        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }



        [ChildActionOnly]
        public ActionResult DisplayBallotInformation(BallotViewModel ballotVM)
        {
            List<int> listSelectedCandidatesOnBallot = null;
            List<BallotIssueViewModel> listSelectedIssuesOnBallot = null;

            // Store selections on ballot if user started filling out ballot
            if (ModelState.IsValid)
            {
                // get list of selected candidates from ballot
                listSelectedCandidatesOnBallot = GetListOfSelectedCandidateIdFromBallot(ballotVM.BallotOfficeViewModel);

                // get list of selected issues from ballot
                listSelectedIssuesOnBallot = GetListOfSelectedIssueIdFromBallot(ballotVM.BallotIssueViewModel);
            }

            // validate dateId
            int intDateId = 1; // set to Nov 2016 election

            // make sure a voter provided their address
            VoterAddressViewModel voterAddressVM = GetVoterAddressFromSession();
            voterAddressVM.ControllerName = _controllerName;

            // was address found?
            bool hasLocation = ValidateSuppliedVoterAddress(voterAddressVM);
            if (hasLocation == false)
            {
                // location not available -- get voter's location information 
                ViewModels.Location.VoterLocationViewModel voterLocationVM = new ViewModels.Location.VoterLocationViewModel(voterAddressVM);
                return PartialView("_VoterLocationForm", voterLocationVM);
            }

            // make sure supplied dateId is a valid votingDateId
            int votingDateId = GetVotingDateForNextElectionFromDatabase();
            if (intDateId > votingDateId) { intDateId = votingDateId; }

            // get general ballot information
            BallotViewModel newBallotVM = new BallotViewModel(_controllerName, intDateId.ToString(), GetDropDownListOfVotingDates());

            // get voter LocationID from Session
            string voterLocationId = GetVoterLocationIdFromSession();

            if (validVoterLocationId(voterLocationId))
            {

                // get voter information from database
                BallotVoterViewModel ballotVoterVM = GetBallotInformationForVoterAddressFromDatabase(voterLocationId);

                // get office information for voter from databse
                List<BallotOfficeViewModel> collectionBallotOfficeVM = GetListOfOfficesAndCandidatesForBallot(intDateId, ballotVoterVM, listSelectedCandidatesOnBallot);

                // get issue information for voter from database
                List<BallotIssueViewModel> collectionBallotIssueVM = GetListOfIssuesForBallot(ballotVoterVM, listSelectedIssuesOnBallot);

                // display ballot
                return PartialView("_Ballot", new BallotViewModel(newBallotVM, ballotVoterVM, collectionBallotOfficeVM, collectionBallotIssueVM));
            }

            // proof of concept for Hamilton County
            // Not a registered voter in Hamilton County - can not display ballot
            return PartialView("_Ballot", new BallotViewModel());
        }



        public string GetVoterLocationIdFromSession()
        {// Tests not generated because of session testing null reference error
            SessionExtensions session = new SessionExtensions();
            return session.GetVoterLocationId();
        }



        public bool validVoterLocationId(string voterLocationId)
        {
            if (voterLocationId == null || voterLocationId == "" || voterLocationId == "0")
            {
                return false;
            }

            int intVoterLocationId = GetIntegerFromStringValue(voterLocationId);

            if (intVoterLocationId > 0)
            {
                return true;
            }

            return false;
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



        public VoterAddressViewModel GetVoterAddressFromSession()
        {
            // get voter location from session
            Services.SessionExtensions session = new Services.SessionExtensions();
            ViewModels.Location.VoterLocationViewModel locationVM = session.GetVoterLocationFromSession();
            return new VoterAddressViewModel(locationVM);
        }



        public bool ValidateSuppliedVoterAddress(VoterAddressViewModel voterAddressVM)
        {
            // validate voter location
            Controllers.LocationController location = new Controllers.LocationController();
            ViewModels.Location.VoterLocationViewModel locationVM = new ViewModels.Location.VoterLocationViewModel(voterAddressVM);

            return location.ValidateVoterLocation(locationVM);
        }



        public BallotVoterViewModel GetBallotInformationForVoterAddressFromDatabase(string voterLocationId)
        {
            // convert string values to integer
            //int intStreetNumber = GetIntegerFromStringValue(voterAddressVM.StreetNumber);
            int intLocationId = GetIntegerFromStringValue(voterLocationId);

            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                // only checking for ballots in hamilton County, OHIO for proof of concept
                // voter address already checked
                // use voterId?
                // check most precise address
                Models.HamiltonOhioVoter hamiltonOhioVoterDTO = context.HamiltonOhioVoters.FirstOrDefault(x => x.Id == intLocationId);

                if (hamiltonOhioVoterDTO == null) { return new BallotVoterViewModel(); }

                return new BallotVoterViewModel(hamiltonOhioVoterDTO);
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



        public int GetIntegerFromStringValue(string strValue)
        {
            int intValue = 0;
            if (int.TryParse(strValue, out intValue))
            {
                intValue = int.Parse(strValue);
            }

            return intValue;
        }



        public List<BallotOfficeViewModel> GetListOfOfficesAndCandidatesForBallot(int dateId, BallotVoterViewModel ballotVoterVM,  List<int> selectedCandidatesList)
        {
            // get office information for voter from databse
            List<BallotOfficeViewModel> ballotOfficeVM = GetListOfOfficesForBallotVoterFromDatabase(ballotVoterVM, dateId);

            // remove offices for runningmate and combine office name listed for both candidate / runningmate
            ballotOfficeVM = RemoveOfficesForRunningMates(ballotOfficeVM);

            // get candidate information for each office from database
            ballotOfficeVM = GetListOfCandidatesForBallotOfficeFromDatabase(ballotOfficeVM);

            // get runningmate information from database
            ballotOfficeVM = GetRunningMateInformationForBallotOfficeFromDatabase(ballotOfficeVM);

            // update ballot with selected candidates
            if (selectedCandidatesList != null)
            {
                ballotOfficeVM = UpdateCandidatesThatHaveBeenSelectedOnBallot(ballotOfficeVM, selectedCandidatesList);
            }

            // make sure images are provided for all candidates/runningmates
            return GetImagesForAllCandidatesAndRunningMates(ballotOfficeVM);


            // get candidate images from VoteSmart and return View Model
            //return GetCandidateImagesFromVoteSmart(collectionBallotOfficeWithRunningmateVM);
        }



        public List<BallotOfficeViewModel> GetListOfOfficesForBallotVoterFromDatabase(BallotVoterViewModel ballotVoterVM, int dateId)
        {
            List<BallotOfficeViewModel> officeVM = new List<BallotOfficeViewModel>();

            // get list of all offices for current election date
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.ElectionOffice> dbOffices = context.ElectionOffices.Where(x => x.ElectionVotingDateId == dateId)
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



        public List<BallotOfficeViewModel> RemoveOfficesForRunningMates(List<BallotOfficeViewModel> ballotOfficesVM)
        {
            int presidentOfficeId = 1;
            int vicePresidentOfficeId = 2;

            int presidentOfficeIndex = -1;
            int vicePresidentOfficeIndex = -1;

            // get office ID for President and Vice President
            for (int i = 0; i < ballotOfficesVM.Count(); i++)
            {
                int currentOfficeId = ballotOfficesVM[i].OfficeId;

                // get indexes for offices
                if (currentOfficeId == presidentOfficeId)
                {
                    presidentOfficeIndex = i;
                }
                else if (currentOfficeId == vicePresidentOfficeId)
                {
                    vicePresidentOfficeIndex = i;
                }

                // do not display offices for runningmates
                if (presidentOfficeIndex != -1 && vicePresidentOfficeIndex != -1)
                {
                    // combine offices for "President / Vice President"
                    ballotOfficesVM[presidentOfficeIndex].OfficeName = String.Format("{0} / {1}", ballotOfficesVM[presidentOfficeIndex].OfficeName, ballotOfficesVM[vicePresidentOfficeIndex].OfficeName);

                    // remove runningmate office
                    ballotOfficesVM.RemoveAt(vicePresidentOfficeIndex);

                    // stop checking
                    i = ballotOfficesVM.Count();
                }
            }

            return ballotOfficesVM;

        }


        /*
        public BallotViewModel GetCandidatesAndIssuesSelectedOnBallot(BallotViewModel ballotSelectionsVM)
        {
            // get list of selected candidates from ballot
            List<int> listSelectedCandidatesOnBallot = GetListOfSelectedCandidateIdFromBallot(ballotSelectionsVM.BallotOfficeViewModel);

            // get list of selected issues from ballot
            List<BallotIssueViewModel> listSelectedIssuesOnBallot = GetListOfSelectedIssueIdFromBallot(ballotSelectionsVM.BallotIssueViewModel);


            *
            // get candidate information for each office from database
            List<BallotOfficeViewModel> collectionBallotOfficeWithCandidateVM = GetListOfCandidatesForBallotOfficeFromDatabase(ballotOfficesVM);
            *
            // update candidates that have been selected on ballot
            List<BallotOfficeViewModel> collectionBallotOfficeWithSelectedCandidateVM = UpdateCandidatesThatHaveBeenSelectedOnBallot(collectionBallotOfficeWithCandidateVM, listSelectedCandidatesOnBallot);
            *
            // get runningmate information from database
            List<BallotOfficeViewModel> collectionBallotOfficeWithCandidateAndRunningmateVM = GetRunningMateInformationForBallotOfficeFromDatabase(collectionBallotOfficeWithSelectedCandidateVM);

            // make sure images are provided for all candidates/runningmates
            return GetImagesForAllCandidatesAndRunningMates(collectionBallotOfficeWithCandidateAndRunningmateVM);
            *
        }
        */


        public List<int> GetListOfSelectedCandidateIdFromBallot(List<BallotOfficeViewModel> ballotOfficesVM)
        {
            if (ballotOfficesVM == null) { return new List<int>(); }

            List<int> listSelectedCandidateId = new List<int>();

            // check each office on ballot
            for (int i = 0; i < ballotOfficesVM.Count(); i++)
            {
                // make sure there are listed candidates for office
                if (ballotOfficesVM[i].BallotListedCandidatesViewModel != null)
                {
                    // check each candidate
                    for (int j = 0; j < ballotOfficesVM[i].BallotListedCandidatesViewModel.Count(); j++)
                    {
                        // add candidates selected on ballot
                        if (ballotOfficesVM[i].BallotListedCandidatesViewModel[j].IsSelected == true)
                        {
                            listSelectedCandidateId.Add(ballotOfficesVM[i].BallotListedCandidatesViewModel[j].CandidateId);
                        }
                    }
                }

                // make sure there are write-in candidates for office
                if (ballotOfficesVM[i].BallotwriteInCandidatesViewModel != null)
                {
                    // check each candidate
                    for (int j = 0; j < ballotOfficesVM[i].BallotwriteInCandidatesViewModel.Count(); j++)
                    {
                        // add candidates selected on ballot
                        if (ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j].IsSelected == true)
                        {
                            listSelectedCandidateId.Add(ballotOfficesVM[i].BallotwriteInCandidatesViewModel[j].CandidateId);
                        }
                    }
                }
            }

            return listSelectedCandidateId; 
        }


        
        public List<BallotOfficeViewModel> GetListOfCandidatesForBallotOfficeFromDatabase(List<BallotOfficeViewModel> ballotOfficesVM)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                // get office ID
                for (int i = 0; i < ballotOfficesVM.Count(); i++)
                {
                    int currentOfficeId = ballotOfficesVM[i].ElectionOfficeId;
                    string currentOfficeName = ballotOfficesVM[i].OfficeName;
                    string currentOfficeTerm = ballotOfficesVM[i].OfficeTerm;

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

            return ballotOfficesVM;
        }



         private List<BallotOfficeViewModel> UpdateCandidatesThatHaveBeenSelectedOnBallot(List<BallotOfficeViewModel> collectionBallotOfficeWithCandidateVM, List<int> listSelectedCandidatesOnBallot)
        {
            if (collectionBallotOfficeWithCandidateVM == null ) { return new List<BallotOfficeViewModel>(); }

            if (listSelectedCandidatesOnBallot == null) { return collectionBallotOfficeWithCandidateVM; }

            // check each office on ballot
            for (int i = 0; i < collectionBallotOfficeWithCandidateVM.Count(); i++)
            {
                collectionBallotOfficeWithCandidateVM[i].HasSelectedCandidate = false;

                // make sure there are listed candidates for office
                if (collectionBallotOfficeWithCandidateVM[i].BallotListedCandidatesViewModel != null)
                {
                    // check each candidate
                    for (int j = 0; j < collectionBallotOfficeWithCandidateVM[i].BallotListedCandidatesViewModel.Count(); j++)
                    {
                        // update selected candidates on ballot
                        var selectedCandidate = listSelectedCandidatesOnBallot.FirstOrDefault(x => x == collectionBallotOfficeWithCandidateVM[i].BallotListedCandidatesViewModel[j].CandidateId);
                        if (selectedCandidate > 0)
                        {
                            collectionBallotOfficeWithCandidateVM[i].HasSelectedCandidate = true;
                            collectionBallotOfficeWithCandidateVM[i].BallotListedCandidatesViewModel[j].IsSelected = true;
                        }
                    }
                }

                // make sure there are write-in candidates for office
                if (collectionBallotOfficeWithCandidateVM[i].BallotwriteInCandidatesViewModel != null)
                {
                    // check each candidate
                    for (int j = 0; j < collectionBallotOfficeWithCandidateVM[i].BallotwriteInCandidatesViewModel.Count(); j++)
                    {
                        // update selected candidates on ballot
                        var selectedCandidate = listSelectedCandidatesOnBallot.FirstOrDefault(x => x == collectionBallotOfficeWithCandidateVM[i].BallotwriteInCandidatesViewModel[j].CandidateId);
                        if (selectedCandidate > 0)
                        {
                            collectionBallotOfficeWithCandidateVM[i].HasSelectedCandidate = true;
                            collectionBallotOfficeWithCandidateVM[i].BallotwriteInCandidatesViewModel[j].IsSelected = true;
                        }
                    }
                }
            }

            return collectionBallotOfficeWithCandidateVM;
        }


        /*
        private List<BallotIssueViewModel> GetIssueInformationFromIssueIdOnBallot(List<BallotIssueViewModel> BallotIssueVM, int dateId)
        {
            // get list of selected candidates from ballot
            List<BallotIssueViewModel> listSelectedIssuesOnBallot = GetListOfSelectedIssueIdFromBallot(BallotIssueVM);

            // get candidate information for each office from database
            List<BallotIssueViewModel> collectionBallotOfficeWithCandidateVM = GetListOfIssuesForBallotFromDatabase(BallotIssueVM);

            // update candidates that have been selected on ballot
            return UpdateIssuesThatHaveBeenSelectedOnBallot(collectionBallotOfficeWithCandidateVM, listSelectedIssuesOnBallot);
        }
        */


        private List<BallotIssueViewModel> GetListOfSelectedIssueIdFromBallot(List<BallotIssueViewModel> BallotIssueVM)
        {
            if (BallotIssueVM == null) { return new List<BallotIssueViewModel>(); }

            List<BallotIssueViewModel> listSelectedIssues = new List<BallotIssueViewModel>();

                // make sure there are issues on ballot
                if (BallotIssueVM != null)
                {
                    // check each issue
                    for (int i = 0; i < BallotIssueVM.Count(); i++)
                    {
                        // add issue if option selected on ballot
                        if (BallotIssueVM[i].SelectedValue != null && BallotIssueVM[i].SelectedValue != "")
                        {
                            listSelectedIssues.Add(BallotIssueVM[i]);
                        }
                    }
                }

            return listSelectedIssues;
        }




        /*
        private List<BallotIssueViewModel> GetListOfIssuesForBallotFromDatabase(List<BallotIssueViewModel> ballotIssueVM)
        {
            if (ballotIssueVM == null) { return new List<BallotIssueViewModel>(); }

            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<BallotIssueViewModel> issueVM = new List<BallotIssueViewModel>();

                for (int i = 0; i < ballotIssueVM.Count(); i++)
                {
                    List<Models.ElectionIssuePrecinct> dbIssuePrecinct = context.ElectionIssuePrecincts.ToList();
                    Models.ElectionIssuePrecinct issueDTO = dbIssuePrecinct.FirstOrDefault(x => x.ElectionIssue.Id == ballotIssueVM[i].ElectionIssueId);

                    if (issueDTO != null)
                    {
                        issueVM.Add(new BallotIssueViewModel(issueDTO));
                    }
                }

                return issueVM;
            }
        }

    */

        public string GetCandidateImageFromDatabase(int candidateId)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                Models.Candidate candidateDTO = context.Candidates.FirstOrDefault(x => x.Id == candidateId);

                // candidate not found
                if (candidateDTO == null) { return string.Empty; }

                // return photo location
                return candidateDTO.Photo;
            }
        }



        public List<BallotOfficeViewModel> GetRunningMateInformationForBallotOfficeFromDatabase(List<BallotOfficeViewModel> ballotOfficesVM)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                // get list of all candidates
                List<Models.Candidate> dbCandidates = context.Candidates.ToList();

                // add only the runningmate candidates to their offices
                // only checking for President/Vice President runningmate
                for (int i = 0; i < ballotOfficesVM.Count; i++)
                {
                    // find Listed Candidates where office = President
                    if (ballotOfficesVM[i].ElectionOfficeId == 1)
                    {
                        for (int j = 0; j < ballotOfficesVM[i].BallotListedCandidatesViewModel.Count; j++)
                        {
                            // get VP information
                            Models.Candidate candidateDTO = dbCandidates.FirstOrDefault(x => x.Id == ballotOfficesVM[i].BallotListedCandidatesViewModel[j].RunningMateId);

                            ballotOfficesVM[i].BallotListedCandidatesViewModel[j] = new BallotCandidateViewModel(ballotOfficesVM[i].BallotListedCandidatesViewModel[j], candidateDTO);
                        }
                    }

                    // find WriteIn Candidates where office = President
                    if (ballotOfficesVM[i].ElectionOfficeId == 1)
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



        public List<BallotOfficeViewModel> GetSelectedCandidatesOnBallot(List<BallotOfficeViewModel> ballotOfficeVM, List<BallotOfficeViewModel> selectedCanidatesOfficeVM)
        {
            if (ballotOfficeVM == null) { return new List<BallotOfficeViewModel>(); }

            if (selectedCanidatesOfficeVM == null) { return ballotOfficeVM; }

            List<BallotOfficeViewModel> ballotOfficeWithSelectedCanidatesVM = new List<BallotOfficeViewModel>();

            // check each office for selected candidates
            for (int i = 0; i < selectedCanidatesOfficeVM.Count(); i++)
            {
                var selectedCandidates = selectedCanidatesOfficeVM[i].BallotListedCandidatesViewModel.Where(x => x.IsSelected == true).ToList();
                foreach (var candidate in selectedCandidates)
                {
                    bool isFound = false;

                    // check listed candidates
                    for (int j = 0; j < ballotOfficeVM[i].BallotListedCandidatesViewModel.Count(); j++)
                    {
                        if (ballotOfficeVM[i].BallotListedCandidatesViewModel[j].CandidateId == candidate.CandidateId)
                        {
                            // update candidate and stop looking
                            ballotOfficeVM[i].BallotListedCandidatesViewModel[j].IsSelected = true;
                            isFound = true;
                            j = ballotOfficeVM[i].BallotListedCandidatesViewModel.Count();
                        }
                    }

                    // Keep checking?
                    if (isFound == false && ballotOfficeVM[i].BallotwriteInCandidatesViewModel != null)
                    {
                        // check write-in candidates
                        for (int j = 0; j < ballotOfficeVM[i].BallotwriteInCandidatesViewModel.Count(); j++)
                        {
                            if (ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].CandidateId == candidate.CandidateId)
                            {
                                // update candidate and stop looking
                                ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].IsSelected = true;
                                isFound = true;
                                j = ballotOfficeVM[i].BallotwriteInCandidatesViewModel.Count();
                            }
                        }
                    }

                }
            }

            return ballotOfficeWithSelectedCanidatesVM;
        }



        public List<BallotOfficeViewModel> GetImagesForAllCandidatesAndRunningMates(List<BallotOfficeViewModel> ballotOfficeVM)
        {
            if(ballotOfficeVM == null)
            {
                return new List<BallotOfficeViewModel>();
            }
            
            // check all offices
            for (int i = 0; i < ballotOfficeVM.Count(); i++)
            {
                // get silhouettes for listed candidates/runningmates missing images
                for (int j = 0; j < ballotOfficeVM[i].BallotListedCandidatesViewModel.Count(); j++)
                {
                    // check listed candidate
                    if (ballotOfficeVM[i].BallotListedCandidatesViewModel[j].CandidatePhoto == null || ballotOfficeVM[i].BallotListedCandidatesViewModel[j].CandidatePhoto == "")
                    {
                        ballotOfficeVM[i].BallotListedCandidatesViewModel[j].CandidatePhoto = GetImageSilhouetteForGender(ballotOfficeVM[i].BallotListedCandidatesViewModel[j].CandidateGender);
                    }

                    // is there a listed runningmate?
                    if (ballotOfficeVM[i].BallotListedCandidatesViewModel[j].RunningMateFirstName != null)
                    {
                        // check listed runningmate
                        if (ballotOfficeVM[i].BallotListedCandidatesViewModel[j].RunningMatePhoto == null || ballotOfficeVM[i].BallotListedCandidatesViewModel[j].RunningMatePhoto == "")
                        {
                            ballotOfficeVM[i].BallotListedCandidatesViewModel[j].RunningMatePhoto = GetImageSilhouetteForGender(ballotOfficeVM[i].BallotListedCandidatesViewModel[j].RunningMateGender);
                        }
                    }
                }

                // get silhouettes for write-in candidates/runningmates missing images
                for (int j = 0; j < ballotOfficeVM[i].BallotwriteInCandidatesViewModel.Count(); j++)
                {
                    // check write-in candidate
                    if (ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].CandidatePhoto == null || ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].CandidatePhoto == "")
                    {
                        ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].CandidatePhoto = GetImageSilhouetteForGender(ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].CandidateGender);
                    }

                    // is there a write-in runningmate?
                    if (ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].RunningMateFirstName != null)
                    {
                        // check write-in runningmate
                        if (ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].RunningMatePhoto == null || ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].RunningMatePhoto == "")
                        {
                            ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].RunningMatePhoto = GetImageSilhouetteForGender(ballotOfficeVM[i].BallotwriteInCandidatesViewModel[j].RunningMateGender);
                        }
                    }
                }
            }

            return ballotOfficeVM;
        }




        public string GetImageUrlForCandidate(string imageUrl, string gender)
        {
            if (imageUrl == null || imageUrl == "")
            {
                return GetImageSilhouetteForGender(gender);
            }
            else
            {
                return imageUrl;
            }
        }



        public string GetImageSilhouetteForGender(string gender)
        {
            // gender image
            if (gender == "F")
            {
                return "~/Content/images/image_female.png";
            }
            else
            {
                return "~/Content/images/image_male.png";
            }
        }




        public List<BallotIssueViewModel> GetListOfIssuesForBallot(BallotVoterViewModel ballotVoterVM, List<BallotIssueViewModel> selectedIssuesList)
        {
            if (ballotVoterVM == null) { return new List<BallotIssueViewModel>(); }

            List<BallotIssueViewModel> issuesVM = GetListOfIssuesForBallotFromDatabase(ballotVoterVM);

            return UpdateIssuesThatHaveBeenSelectedOnBallot(issuesVM, selectedIssuesList);

        }



        public List<BallotIssueViewModel> GetListOfIssuesForBallotFromDatabase(BallotVoterViewModel ballotVoterVM)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                // get all issues for current precinct
                List<Models.ElectionIssuePrecinct> dbIssuesPrecincts = context.ElectionIssuePrecincts.Where(x => x.OhioPrecinctId == ballotVoterVM.OhioPrecinctId).ToList();

                if (dbIssuesPrecincts == null) { return new List<BallotIssueViewModel>(); }

                List<BallotIssueViewModel> issueVM = new List<BallotIssueViewModel>();

                foreach (var issueDTO in dbIssuesPrecincts)
                {
                    // Check if issue is for precinct with mutliple school districts
                    if (issueDTO.ElectionIssue.SchoolCode != null && issueDTO.ElectionIssue.SchoolCode != "")
                    {
                        if (issueDTO.ElectionIssue.SchoolCode == ballotVoterVM.SchoolOfficeCode)
                        {
                            issueVM.Add(new BallotIssueViewModel(issueDTO));
                        }
                    }
                    else
                    {
                        // Issue is for everyone at precinct
                        issueVM.Add(new BallotIssueViewModel(issueDTO));
                    }
                }

                return issueVM;
            }
        }



        private List<BallotIssueViewModel> UpdateIssuesThatHaveBeenSelectedOnBallot(List<BallotIssueViewModel> IssueVM, List<BallotIssueViewModel> selectedIssuesList)
        {
            if (IssueVM == null) { return new List<BallotIssueViewModel>(); }

            if (selectedIssuesList == null) { return IssueVM; }

            // check each Issue on ballot
            for (int i = 0; i < IssueVM.Count(); i++)
            {
                // get information for current selected issue
                var selectedIssue = selectedIssuesList.FirstOrDefault(x => x.ElectionIssueId == IssueVM[i].ElectionIssueId);

                // update selected issues on ballot
                if (selectedIssue != null)
                {
                    // check values to determine if one is checked
                    if (selectedIssue.SelectedValue == "")
                    {
                        IssueVM[i].Option1Checked = "";
                        IssueVM[i].Option2Checked = "";
                        IssueVM[i].OptionChecked = 0;
                    }
                    else if (selectedIssue.SelectedValue == IssueVM[i].Option1Value)
                    {
                        IssueVM[i].Option1Checked = "checked";
                        IssueVM[i].Option2Checked = "";
                        IssueVM[i].OptionChecked = 1;
                    }
                    else if (selectedIssue.SelectedValue == IssueVM[i].Option2Value)
                    {
                        IssueVM[i].Option1Checked = "";
                        IssueVM[i].Option2Checked = "checked";
                        IssueVM[i].OptionChecked = 2;
                    }
                }
            }

            return IssueVM;
        }




    }
}