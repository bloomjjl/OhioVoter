using OhioVoter;
using OhioVoter.Models;
using OhioVoter.Services;
using OhioVoter.ViewModels;
using OhioVoter.ViewModels.Candidate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Controllers
{
    public class CandidateController : Controller
    {
        private static string _controllerName = "Candidate";



        // GET: Candidate
        public ActionResult Index(int? candidateId)
        {
            // validate values provided
            int suppliedCandidateId = ValidateAndReturnInteger(candidateId);

            // update session with controller info
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);

            // get details for view model
            CandidateViewModel viewModel = new CandidateViewModel(_controllerName, suppliedCandidateId);

            return View(viewModel);
        }



        public ActionResult Compare(int? firstCandidateId, int? secondCandidateId, int? dateId, int? officeId)
        {
            // validate values provided
            int suppliedFirstCandidateId = ValidateAndReturnInteger(firstCandidateId);
            int suppliedSecondCandidateId = ValidateAndReturnInteger(secondCandidateId);
            int suppliedDateId = ValidateAndReturnInteger(dateId);
            int suppliedOfficeId = ValidateAndReturnInteger(officeId);

            // values must be provided
            if (suppliedFirstCandidateId <= 0 || suppliedDateId <= 0 || suppliedOfficeId <= 0)
            {
                return RedirectToAction("Index", "Candidate", 0);
            }

            // update session with controller info
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);
            Models.ElectionVotingDate date = GetDateInformationForDateIdFromDatabase(suppliedDateId);

            // get details for view model
            CandidateCompareViewModel viewModel = new CandidateCompareViewModel()
            {
                ControllerName = _controllerName,
                CandidateFirstDisplayId = suppliedFirstCandidateId,
                CandidateSecondDisplayId = suppliedSecondCandidateId,
                VotingDateId = suppliedDateId,
                VotingDate = date.Date.ToShortDateString(),
                OfficeId = suppliedOfficeId
            };

            // get information for the second candidate(s)
            IEnumerable<SelectListItem> candidates = GetCandidateCompareListItems(suppliedFirstCandidateId, suppliedDateId, suppliedOfficeId);
            
            // make sure candidate list found
            if (candidates == null)
            {
                viewModel.CandidateSecondCompareCount = 0;
            }
            else
            {
                viewModel.CandidateSecondCompareCount = candidates.Count();
            }

            // if no additional candidates to display
            if (viewModel.CandidateSecondCompareCount == 0)
            {
                // no additional candidates found to display
                // John Barnes is only candidate running for office
                // TODO: add error message "no other candidates are running for this office"
                return RedirectToAction("Index", "Candidate", new { candidateId = suppliedFirstCandidateId });
            }

            // if only one candidate then display candidate information
            if (viewModel.CandidateSecondCompareCount == 1)
            {
                int intId;
                string strId = candidates.ElementAt(0).Value.ToString();
                if (int.TryParse(strId, out intId))
                {
                    // load all data for compare view model
                    viewModel.CandidateSecondDisplayId = int.Parse(strId);
                    viewModel.CandidateCompareDisplayViewModel = GetCandidateCompareDisplayViewModel(viewModel);
                }
                return View(viewModel);
            }

            // get information for the candidateCompareDisplayViewModel
            if (viewModel.CandidateSecondCompareCount > 1)
            {
                //string selectedCandidateId = "0";
                /*viewModel.CandidateCompareDisplayViewModel.CandidateCompareSummaryViewModel.CandidateCompareSummaryLookUpViewModel candidateCompareLookUpSecondVM = new CandidateCompareSummaryLookUpViewModel(viewModel.CandidateFirstDisplayId,
                                                                                                                                   viewModel.VotingDateId,
                                                                                                                                   viewModel.OfficeId,
                                                                                                                                   candidates,
                                                                                                                                   selectedCandidateId);
                */
                // load all data for compare lookUp view model
                viewModel.CandidateCompareDisplayViewModel = GetCandidateCompareDisplayViewModel(viewModel);

                //return PartialView("_CandidateCompareSummaryLookUp", candidateCompareLookUpSecondVM);
                return View(viewModel);
            }

            // no candidates found to compare
            return RedirectToAction("Index", "Candidate", 0);
        }



        public int GetIntegerValueFromStringValue(string strValue)
        {
            int intValue;

            if (strValue == null) { return 0; }
            else if (int.TryParse(strValue, out intValue)) { return int.Parse(strValue); }
            else { return 0; }
        }



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




        public ActionResult RemoveFirst(int? firstCandidateId, int? secondCandidateId, int? candidateCompareCount, int? dateId, int? officeId)
        {
            // validate values provided
            int suppliedFirstCandidateId = ValidateAndReturnInteger(firstCandidateId);
            int suppliedSecondCandidateId = ValidateAndReturnInteger(secondCandidateId);
            int suppliedCandidateCompareCount = ValidateAndReturnInteger(candidateCompareCount);
            int suppliedDateId = ValidateAndReturnInteger(dateId);
            int suppliedOfficeId = ValidateAndReturnInteger(officeId);

            // first candidate removed
            if (suppliedCandidateCompareCount > 1)
            {
                // clear message
                TempData["CandidateMessage"] = string.Empty;

                // move secondCandidateId to first and clear candidateSecondId to display dropdownlist
                return RedirectToAction("Compare", "Candidate", new { firstCandidateId = suppliedSecondCandidateId, secondCandidateId = 0, dateId = suppliedDateId, officeId = suppliedOfficeId });
            }
            else if (suppliedCandidateCompareCount == 1)
            {
                // store message to display to user
                TempData["CandidateMessage"] = "Only two candidates are running for this office";

                // display secondCandidateId
                return RedirectToAction("Index", "Candidate", new { candidateId = suppliedSecondCandidateId });
            }
            else
            {
                // store message to display to user
                TempData["CandidateMessage"] = "No other candidates to display";

                // display secondCandidateId
                return RedirectToAction("Index", "Candidate", new { candidateId = suppliedSecondCandidateId });
            }
        }


        public ActionResult RemoveSecond(int? firstCandidateId, int? secondCandidateId, int? candidateCompareCount, int? dateId, int? officeId)
        {
            // validate values provided
            int suppliedFirstCandidateId = ValidateAndReturnInteger(firstCandidateId);
            int suppliedSecondCandidateId = ValidateAndReturnInteger(secondCandidateId);
            int suppliedCandidateCompareCount = ValidateAndReturnInteger(candidateCompareCount);
            int suppliedDateId = ValidateAndReturnInteger(dateId);
            int suppliedOfficeId = ValidateAndReturnInteger(officeId);

            // second candidate removed
            if (suppliedCandidateCompareCount > 1)
            {
                // clear message
                TempData["CandidateMessage"] = string.Empty;

                // clear candidateSecondId to display dropdownlist
                return RedirectToAction("Compare", "Candidate", new { firstCandidateId = suppliedFirstCandidateId, secondCandidateId = 0, dateId = dateId, officeId = officeId });
            }
            else if (suppliedCandidateCompareCount == 1)
            {
                // store message to display to user
                TempData["CandidateMessage"] = "Only two candidates are running for this office";

                // display firstCandidateId
                return RedirectToAction("Index", "Candidate", new { candidateId = suppliedFirstCandidateId });
            }
            else
            {
                // store message to display to user
                TempData["CandidateMessage"] = "No other candidates to display";

                // display firstCandidateId
                return RedirectToAction("Index", "Candidate", new { candidateId = suppliedFirstCandidateId });
            }
        }



        [ChildActionOnly]
        public ActionResult DisplayCandidateInformation(CandidateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // create empty model
                CandidateLookUpViewModel candidateLookUpVM = new CandidateLookUpViewModel(_controllerName);
                return PartialView("_CandidateLookUp", candidateLookUpVM);
            }

            // display selected candidate
            if (model.CandidateId > 0)
            {
                CandidateDisplayViewModel candidateDisplayVM = GetCandidateDisplayViewModel(model.CandidateId);
                return PartialView("_CandidateDisplay", candidateDisplayVM);
            }
            
            // Election Date to display
            Models.ElectionVotingDate date = GetOldestVotingDate();

            // get list of all offices for current election
            int electionOfficeId = 0;
            IEnumerable<SelectListItem> electionOfficeSelectList = GetElectionOfficeListItems(date.Id, electionOfficeId);

            // only display candidates if office selected or candidate name has been provided
            //List<CandidateListViewModel> candidateListVM = new List<CandidateListViewModel>();
            // get list of all candidates for current election
            string candidateLookUpName = string.Empty;
            List<CandidateListViewModel> candidateListVM = GetListOfCandidatesForCurrentElectionOffice(date.Id, electionOfficeId, candidateLookUpName);

            // performance
            // 1275 - 1314 ms
            

            return PartialView("_CandidateLookUp", new CandidateLookUpViewModel(model.ControllerName, date.Date.ToShortDateString(), candidateListVM, electionOfficeSelectList)); 
        }



        public ActionResult UpdateCandidateLookUpList (string electionOfficeId, string candidateLookUpName)
        {
            // validate input values
            int intElectionOfficeId = GetIntegerValueFromStringValue(electionOfficeId);
            if(string.IsNullOrWhiteSpace(candidateLookUpName)) { candidateLookUpName = ""; }

            // Election Date to display
            Models.ElectionVotingDate date = GetOldestVotingDate();

            // sort list of candidates by office name?
            List<CandidateListViewModel> candidateListVM = GetListOfCandidatesForCurrentElectionOffice(date.Id, intElectionOfficeId, candidateLookUpName);

            // performance
            // 1282ms

            if (Request.IsAjaxRequest())
            {
                if (candidateListVM.Count > 0)
                {
                    return PartialView("_CandidateList", candidateListVM);
                }
                else
                {
                    return PartialView("_CandidateListEmpty");
                }
            }
            else
            {
                return View("Index");
            }
        }



        public List<CandidateListViewModel> GetListOfCandidatesForCurrentElectionOffice(int electionDateId, int electionOfficeId, string candidateLookUpName)
        {
            if (electionOfficeId == 0 && string.IsNullOrEmpty(candidateLookUpName))
            {
                // no office provided
                // display all candidates for current election
                return GetListOfCandidatesForCurrentElectionFromDatabase(electionDateId, candidateLookUpName);

                // improve performance by displaying smaller list of candidates
                //return new List<CandidateListViewModel>();
            }
            else if (electionOfficeId == 0)
            {
                // display all candidates based on name provided
                return GetListOfCandidatesForCurrentElectionFromDatabase(electionDateId, candidateLookUpName);
            }
            else
            {
                // only display candidates for supplied office
                return GetListOfCandidatesForElectionOfficeFromDatabase(electionOfficeId, candidateLookUpName);
            }
        }



        public List<CandidateListViewModel> GetListOfCandidatesForCurrentElectionFromDatabase(int dateId, string candidateLookUpName)
        {
            // validate input values
            if (dateId <= 0) { return new List<CandidateListViewModel>(); }

            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<ElectionCandidate> dbCandidates = null;

               

                // is a candidate being looked up?
                if (!string.IsNullOrEmpty(candidateLookUpName))
                {
                    // remove candidates from list if don't contain matching string in first or last name
                    candidateLookUpName = candidateLookUpName.ToLower();

                    dbCandidates = context.ElectionCandidates.Include("Candidate").Include("ElectionOffice").Include("Party").Include("ElectionOffice.Office")
                                                            .Where(x => x.ElectionVotingDateId == dateId && (x.Candidate.FirstName + " " + x.Candidate.LastName).Contains(candidateLookUpName))
                                                            .OrderBy(x => x.Candidate.FirstName)
                                                            .OrderBy(x => x.Candidate.LastName)
                                                            .ToList();
                } else
                {
                     // get info from database
                dbCandidates = context.ElectionCandidates.Include("Candidate").Include("ElectionOffice").Include("Party").Include("ElectionOffice.Office")
                                                            .Where(x => x.ElectionVotingDateId == dateId)
                                                            .OrderBy(x => x.Candidate.FirstName)
                                                            .OrderBy(x => x.Candidate.LastName)
                                                            .ToList();
                }

                if (dbCandidates == null || dbCandidates.Count == 0) { return new List<CandidateListViewModel>(); }

                // convert to ViewModel                
                List<CandidateListViewModel> candidateVM = new List<CandidateListViewModel>();

                for (int i = 0; i < dbCandidates.Count(); i++)
                {
                    if (string.IsNullOrEmpty(dbCandidates[i].Candidate.VoteSmartPhotoUrl))
                    {
                        dbCandidates[i].Candidate.VoteSmartPhotoUrl = GetGenderImageLocationToDisplay(dbCandidates[i].Candidate.Gender);
                    }

                    candidateVM.Add(new CandidateListViewModel(dbCandidates[i]));
                }
                

                return candidateVM;
            }
        }



        public List<CandidateListViewModel> GetListOfCandidatesForElectionOfficeFromDatabase(int electionOfficeId, string candidateLookUpName)
        {
            // validate supplied values
            int intOfficeId = GetIntegerValueFromStringValue(electionOfficeId.ToString());

            if (intOfficeId <= 0 && string.IsNullOrEmpty(candidateLookUpName)) { return new List<CandidateListViewModel>(); }

            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<ElectionCandidate> dbCandidates = null;

                // get info from database
                dbCandidates = context.ElectionCandidates.Where(x => x.ElectionOfficeId == intOfficeId)
                                                            .OrderBy(x => x.Candidate.FirstName)
                                                            .OrderBy(x => x.Candidate.LastName)
                                                            .ToList();

                // is a candidate being looked up?
                if (!string.IsNullOrEmpty(candidateLookUpName))
                {
                    // remove candidates from list if don't contain matching string in first or last name
                    candidateLookUpName = candidateLookUpName.ToLower();

                    dbCandidates = dbCandidates.Where(c => candidateLookUpName.Any(ec => c.Candidate.CandidateFirstLastName.ToLower().Contains(candidateLookUpName)))
                                               .ToList();
                }

                if (dbCandidates == null || dbCandidates.Count == 0) { return new List<CandidateListViewModel>(); }

                // convert to ViewModel
                List<CandidateListViewModel> candidateVM = 
                    dbCandidates.Select(candidateDTO => new CandidateListViewModel(candidateDTO))
                    .ToList();

                // make sure photo stored
                for (int i = 0; i < candidateVM.Count(); i++)
                {
                    if (string.IsNullOrEmpty(candidateVM[i].VoteSmartPhotoUrl))
                    {
                        candidateVM[i].VoteSmartPhotoUrl = candidateVM[i].GenderPhotoUrl;
                    }
                }

                /*
                foreach (var candidateDTO in dbCandidates)
                {
                    candidateVM.Add(new CandidateListViewModel(candidateDTO));

                    if (candidateDTO.Candidate.VoteSmartPhotoUrl == null || candidateDTO.Candidate.VoteSmartPhotoUrl == "")
                    {
                        candidateDTO.Candidate.VoteSmartPhotoUrl = GetGenderImageLocationToDisplay(candidateDTO.Candidate.Gender);
                    }

                    candidateVM.Add(new CandidateListViewModel(candidateDTO));
                }
                */

                return candidateVM;
            }
        }



        [ChildActionOnly]
        public ActionResult DisplayCandidateCompareSummarySecond(CandidateCompareSummaryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // display???
                return PartialView("_CandidateCompareSummaryLookUp", viewModel.CandidateCompareSummaryLookUpViewModel);
            }

            // display information for one candidate or list of candidates
            if (viewModel.CandidateCompareSummarySecondViewModel == null)
            {// no second candidate to display
                // display drop down list for user to select one
                return PartialView("_CandidateCompareSummaryLookUp", viewModel.CandidateCompareSummaryLookUpViewModel);
            }
            else if (viewModel.CandidateSecondDisplayId > 0)
            {// display second candidate
                // display candidate information
                return PartialView("_CandidateCompareSummarySecond", viewModel.CandidateCompareSummarySecondViewModel);
            }
            else
            {// candidate has not been selected yet
                // display drop down list for user to select one
                return PartialView("_CandidateCompareSummaryLookUp", viewModel.CandidateCompareSummaryLookUpViewModel);
            }
        }



        public ActionResult DisplaySecondCandidate(CandidateCompareSummaryLookUpViewModel viewModel)
        {
            // validate selectedCandidateId
            int intId;
            if (int.TryParse(viewModel.SelectedCandidateId, out intId))
            {
                return PartialView("_CandidateCompareSecondLookUp", viewModel);
            }

            // get SummaryViewModel

            // display selected candidate information
            return PartialView("_CandidateCompareSecondSummary", viewModel);
        }



        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }


        /*
        /// <summary>
        /// Update page based on supplied candidate last name
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Name(CandidateLookUpViewModel model)
        //public ActionResult Name(string selectedCandidateId)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", new { candidateId = 0 });
            }

            // get selected OfficeID
            // get list of candidates for selected OfficeId
            // display new list of candidates

            *
            // validate selectededCandidateId
            int candidateId;
            if (int.TryParse(model.SelectedCandidateId, out candidateId))
            {
                candidateId = int.Parse(model.SelectedCandidateId);
            }
            else
            {
                candidateId = 0;
            }
            *

            return RedirectToAction("Index", new { candidateId = candidateId });
        }
        */



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CandidateLookUp(CandidateLookUpViewModel candidateLookUpVM)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", new { candidateId = 0 });
            }

            // make sure view model does not have a candidate selected
            int candidateId = 0;

            return View("Index", new CandidateViewModel(_controllerName, candidateId, candidateLookUpVM));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompareName(CandidateCompareSummaryLookUpViewModel model)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Compare", new { firstCandidateId = model.CandidateFirstDisplayId, secondCandidateId = 0, dateId = model.VotingDateId, officeId = model.OfficeId });
            }

            // validate selectedCandidateId
            int candidateId;
            if (model.SelectedCandidateId == null)
            {
                return RedirectToAction("Compare", new { firstCandidateId = model.CandidateFirstDisplayId, secondCandidateId = 0, dateId = model.VotingDateId, officeId = model.OfficeId });
            }
            else if (int.TryParse(model.SelectedCandidateId, out candidateId))
            {
                candidateId = int.Parse(model.SelectedCandidateId);
                return RedirectToAction("Compare", new { firstCandidateId = model.CandidateFirstDisplayId, secondCandidateId = candidateId, dateId = model.VotingDateId, officeId = model.OfficeId });
            }
            else
            {
                return RedirectToAction("Index", new { candidateId = 0 });
            }
        }



        // ******************************************************************************



        /// <summary>
        /// Get the candidate (& running mate) information to display
        /// </summary>
        /// <param name="candidateDisplayVM"></param>
        /// <returns></returns>
        public CandidateDisplayViewModel GetCandidateDisplayViewModel(int candidateLookUpId)
        {
            int runningMateId = 0;
            string voteSmartCandidateId = "0";
            string voteSmartRunningMateId = "0";

            int votingdateId = GetCurrentVotingDateIdfromDatabase();

            // Get candidate/running mate objects for view model
            CandidateSummaryViewModel summaryVM = GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(candidateLookUpId, votingdateId);
            if (summaryVM == null || summaryVM.CandidateSummary == null) { return new CandidateDisplayViewModel(); }
            int candidateId = ValidateAndReturnInteger(summaryVM.CandidateSummary.CandidateId);

            if (summaryVM.CandidateCount == 1)
            {
                TempData["CandidateMessage"] = "Only one candidate is running for this office.";
            }
            else
            {
                //TempData["CandidateMessage"] = string.Empty;
            }

            if (summaryVM.RunningMateSummary == null)
            {
                summaryVM.RunningMateSummary = new RunningMateSummary()
                {
                    CandidateId = 0,
                    VoteSmartCandidateId = "0"
                };
            }
            else
            {
                runningMateId = summaryVM.RunningMateSummary.CandidateId;
            }

            // validate votesmart candidate objects
            if (!string.IsNullOrEmpty(summaryVM.CandidateSummary.VoteSmartCandidateId))
            {
                voteSmartCandidateId = summaryVM.CandidateSummary.VoteSmartCandidateId;
            }
            if (!string.IsNullOrEmpty(summaryVM.RunningMateSummary.VoteSmartCandidateId))
            {
                voteSmartRunningMateId = summaryVM.RunningMateSummary.VoteSmartCandidateId;
            }

            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = GetCandidateAndRunningMateInformationFromVoteSmart(voteSmartCandidateId, voteSmartRunningMateId);
            
            return new CandidateDisplayViewModel()
            {
                CandidateSummaryViewModel = summaryVM,
                CandidatePoliticalViewModel = new CandidatePoliticalViewModel(GetListFromStringWithLineBreaks(voteSmartCandidates[0].Political), GetListFromStringWithLineBreaks(voteSmartCandidates[1].Political), candidateLookUpId, candidateId, runningMateId),
                CandidateCaucusViewModel = new CandidateCaucusViewModel(GetListFromStringWithLineBreaks(voteSmartCandidates[0].CongMembership), GetListFromStringWithLineBreaks(voteSmartCandidates[1].CongMembership), candidateLookUpId, candidateId, runningMateId),
                CandidateProfessionalViewModel = new CandidateProfessionalViewModel(GetListFromStringWithLineBreaks(voteSmartCandidates[0].Profession), GetListFromStringWithLineBreaks(voteSmartCandidates[1].Profession), candidateLookUpId, candidateId, runningMateId),
                CandidateEducationViewModel = new CandidateEducationViewModel(GetListFromStringWithLineBreaks(voteSmartCandidates[0].Education), GetListFromStringWithLineBreaks(voteSmartCandidates[1].Education), candidateLookUpId, candidateId, runningMateId),
                CandidatePersonalViewModel = new CandidatePersonalViewModel(voteSmartCandidates, candidateLookUpId, candidateId, runningMateId),
                CandidateCivicViewModel = new CandidateCivicViewModel(GetListFromStringWithLineBreaks(voteSmartCandidates[0].OrgMembership), GetListFromStringWithLineBreaks(voteSmartCandidates[1].OrgMembership), candidateLookUpId, candidateId, runningMateId),
                CandidateAdditionalViewModel = new CandidateAdditionalViewModel(GetListFromStringWithLineBreaks(voteSmartCandidates[0].SpecialMsg), GetListFromStringWithLineBreaks(voteSmartCandidates[1].SpecialMsg), candidateLookUpId, candidateId, runningMateId)
            };
        }


        public int GetNumberOfCandidatesRunningForCurrentElectionOfficeFromDatabase(int candidateId, int votingDateId)
        {
            // validate parameters
            int intCandidateId = ValidateAndReturnInteger(candidateId);
            int intVotingDateId = ValidateAndReturnInteger(votingDateId);

            using (Models.OhioVoterDbContext context = new Models.OhioVoterDbContext())
            {
                // get list of offices for election
                List<Models.ElectionCandidate> dbCandidates = context.ElectionCandidates.Where(x => x.ElectionVotingDateId == intVotingDateId).ToList();
                if (dbCandidates == null) { return 0; }

                // get office for candidate
                Models.ElectionCandidate officeDTO = dbCandidates.FirstOrDefault(x => x.CandidateId == intCandidateId);
                if (officeDTO == null) { return 0; }

                // count number of candidates running for office
                List<Models.ElectionCandidate> candidateDTO = dbCandidates.Where(x => x.ElectionOfficeId == officeDTO.ElectionOfficeId).ToList();
                if (candidateDTO == null) { return 0; }

                return candidateDTO.Count;
            }
        }



        public CandidateSummaryViewModel GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(int candidateLookUpId, int votingDateId)
        {
            // validate parameters
            int intCandidateLookUpId = ValidateAndReturnInteger(candidateLookUpId);

            // get number of candidates running for same election office as candidateId
            int candidateCount = GetNumberOfCandidatesRunningForCurrentElectionOfficeFromDatabase(intCandidateLookUpId, votingDateId);

            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                CandidateSummaryViewModel candidateSummaryVM = new CandidateSummaryViewModel();

                // get candidate LookUp info from database
                Models.ElectionCandidate electionRunningMateLookUpDTO = context.ElectionCandidates.FirstOrDefault(x => x.RunningMateId == intCandidateLookUpId);

                // is candidate LookUp a runningmate?
                if (electionRunningMateLookUpDTO != null)
                {
                    // candidateLookUp == runningmate
                    Models.ElectionCandidate electionCandidateDTO = electionRunningMateLookUpDTO;
                    // get runningmate info from database
                    Models.ElectionCandidate electionRunningMateDTO = context.ElectionCandidates.FirstOrDefault(x => x.CandidateId == intCandidateLookUpId);
                    // convert to ViewModel
                    candidateSummaryVM = new CandidateSummaryViewModel(electionRunningMateDTO, candidateCount, electionCandidateDTO, electionRunningMateDTO);
                }
                else
                {
                    // get candidate/runningmate info
                    Models.ElectionCandidate electionCandidateLookUpDTO = context.ElectionCandidates.FirstOrDefault(x => x.CandidateId == intCandidateLookUpId);
                    if (electionCandidateLookUpDTO == null)
                    {
                        // candidateLookUp == NOT FOUND
                        return new CandidateSummaryViewModel();
                    }
                    else
                    {
                        // candidateLookUp == candidate 
                        if (electionCandidateLookUpDTO.RunningMateId != 0)
                        {
                            // get runningmate info from datase
                            Models.ElectionCandidate electionRunningMateDTO = context.ElectionCandidates.FirstOrDefault(x => x.CandidateId == electionCandidateLookUpDTO.RunningMateId);
                            // convert to ViewModel
                            candidateSummaryVM = new CandidateSummaryViewModel(electionCandidateLookUpDTO, candidateCount, electionCandidateLookUpDTO, electionRunningMateDTO);
                        }
                        else
                        {
                            // NO runningmate
                            // convert to ViewModel
                            candidateSummaryVM = new CandidateSummaryViewModel(electionCandidateLookUpDTO, candidateCount, electionCandidateLookUpDTO);
                        }
                    }
                }

                // make sure image provided for candidate(s)
                if (string.IsNullOrEmpty(candidateSummaryVM.CandidateSummary.VoteSmartPhotoUrl))
                {
                    candidateSummaryVM.CandidateSummary.VoteSmartPhotoUrl = candidateSummaryVM.CandidateSummary.GenderPhotUrl;
                }
                if (candidateSummaryVM.RunningMateSummary != null && string.IsNullOrEmpty(candidateSummaryVM.RunningMateSummary.VoteSmartPhotoUrl))
                {
                    candidateSummaryVM.RunningMateSummary.VoteSmartPhotoUrl = candidateSummaryVM.RunningMateSummary.GenderPhotUrl;
                }

                return candidateSummaryVM;
            }
        }



        public int GetCandidateLookUpVotingDateId(List<Models.ElectionCandidate> dbCandidates, int candidateLookUp)
        {
            if (dbCandidates[0] != null && dbCandidates[0].CandidateId == candidateLookUp)
            {// candidate
                return dbCandidates[0].ElectionVotingDateId;
            }
            else if (dbCandidates[1] != null && dbCandidates[1].CandidateId == candidateLookUp)
            {// running mate
                return dbCandidates[1].ElectionVotingDateId;
            }

            return 0;
        }


        public int GetCandidateLookUpOfficeId(List<Models.ElectionCandidate> dbCandidates, int candidateLookUp)
        {
            if (dbCandidates[0] != null && dbCandidates[0].CandidateId == candidateLookUp)
            {// candidate
                return dbCandidates[0].ElectionOfficeId;
            }
            else if (dbCandidates[1] != null && dbCandidates[1].CandidateId == candidateLookUp)
            {// running mate
                return dbCandidates[1].ElectionOfficeId;
            }

            return 0;
        }



        public List<ViewModels.VoteSmart.CandidateBio> GetCandidateAndRunningMateInformationFromVoteSmart(string voteSmartCandidateId, string voteSmartRunningMateId)
        {
            // get candidate/running mate CandidateBio information from votesmart
            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = new List<ViewModels.VoteSmart.CandidateBio>();

            // validate candidate 
            if (string.IsNullOrEmpty(voteSmartCandidateId))
            {
                // STOP'
                // no candidate found
                return new List<ViewModels.VoteSmart.CandidateBio>();
            }
            else
            {
                voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(voteSmartCandidateId));
            }

            // check for runningmate
            if (!string.IsNullOrEmpty(voteSmartRunningMateId))
            {
                voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(voteSmartRunningMateId));
            }

            return voteSmartCandidates;
        }



        public List<ViewModels.VoteSmart.CandidateBio> GetCompareFirstCandidateAndRunningMateInformationFromVoteSmart(CandidateCompareSummaryFirstViewModel summaryVM)
        {
            // validate votesmart candidate objects
            string voteSmartCandidateId = "0";
            string voteSmartRunningMateId = "0";

            if (!string.IsNullOrEmpty(summaryVM.CandidateCompareSummaryFirst.VoteSmartCandidateId))
            {
                voteSmartCandidateId = summaryVM.CandidateCompareSummaryFirst.VoteSmartCandidateId;
            }
            if (!string.IsNullOrEmpty(summaryVM.RunningMateCompareSummaryFirst.VoteSmartCandidateId))
            {
                voteSmartRunningMateId = summaryVM.RunningMateCompareSummaryFirst.VoteSmartCandidateId;
            }

            // get candidate/running mate CandidateBio information from votesmart
            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = new List<ViewModels.VoteSmart.CandidateBio>();

            // validate candidate 
            if (string.IsNullOrEmpty(voteSmartCandidateId))
            {
                // STOP'
                // no candidate found
                return new List<ViewModels.VoteSmart.CandidateBio>();
            }
            else
            {
                voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(voteSmartCandidateId));
                if (string.IsNullOrEmpty(voteSmartCandidates[0].Photo))
                {
                    voteSmartCandidates[0].Photo = GetGenderImageLocationToDisplay(summaryVM.CandidateCompareSummaryFirst.Gender);
                }
            }

            // check for runningmate
            if (!string.IsNullOrEmpty(voteSmartRunningMateId))
            {
                voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(voteSmartRunningMateId));
                if (string.IsNullOrEmpty(voteSmartCandidates[1].Photo))
                {
                    voteSmartCandidates[1].Photo = GetGenderImageLocationToDisplay(summaryVM.RunningMateCompareSummaryFirst.Gender);
                }
            }

            return voteSmartCandidates;

        }



        public List<ViewModels.VoteSmart.CandidateBio> GetCompareSecondCandidateAndRunningMateInformationFromVoteSmart(CandidateCompareSummarySecondViewModel summaryVM)
        {
            if (summaryVM.CandidateCompareSummarySecond == null)
            {
                return new List<ViewModels.VoteSmart.CandidateBio>();
            }
            
            // validate votesmart candidate objects
            string voteSmartCandidateId = "0";
            string voteSmartRunningMateId = "0";

            if (!string.IsNullOrEmpty(summaryVM.CandidateCompareSummarySecond.VoteSmartCandidateId))
            {
                voteSmartCandidateId = summaryVM.CandidateCompareSummarySecond.VoteSmartCandidateId;
            }
            if (!string.IsNullOrEmpty(summaryVM.RunningMateCompareSummarySecond.VoteSmartCandidateId))
            {
                voteSmartRunningMateId = summaryVM.RunningMateCompareSummarySecond.VoteSmartCandidateId;
            }

            // get candidate/running mate CandidateBio information from votesmart
            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = new List<ViewModels.VoteSmart.CandidateBio>();

            // validate candidate 
            if (string.IsNullOrEmpty(voteSmartCandidateId))
            {
                // STOP'
                // no candidate found
                return new List<ViewModels.VoteSmart.CandidateBio>();
            }
            else
            {
                voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(voteSmartCandidateId));
                if (string.IsNullOrEmpty(voteSmartCandidates[0].Photo))
                {
                    voteSmartCandidates[0].Photo = GetGenderImageLocationToDisplay(summaryVM.CandidateCompareSummarySecond.Gender);
                }
            }

            // check for runningmate
            if (!string.IsNullOrEmpty(voteSmartRunningMateId))
            {
                voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(voteSmartRunningMateId));
                if (string.IsNullOrEmpty(voteSmartCandidates[1].Photo))
                {
                    voteSmartCandidates[1].Photo = GetGenderImageLocationToDisplay(summaryVM.RunningMateCompareSummarySecond.Gender);
                }
            }

            return voteSmartCandidates;

        }



        public List<Models.ElectionOffice> GetCandidateAndRunningMateElectionOfficeInformation(int candidateOfficeId, int runningMateOfficeId)
        {
            // get candidate/running mate office information
            List<Models.ElectionOffice> candidateOffices = new List<Models.ElectionOffice>();
            candidateOffices.Add(GetOfficeInformationForOfficeId(candidateOfficeId));
            candidateOffices.Add(GetOfficeInformationForOfficeId(runningMateOfficeId));

            return candidateOffices;
        }




        public string GetOfficeNameForOfficeIdFromDatabase(int officeId)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                Models.Office dbOffice = context.Offices.FirstOrDefault(x => x.Id == officeId);
                if (dbOffice == null)
                {
                    return string.Empty;
                }

                return dbOffice.OfficeName;
            }
        }

        // ***********************************************************************


/*
        public CandidateSummaryViewModel GetCandidateSummaryViewModel(int candidateLookUpId,
                                                                      int candidateLookUpVotingDateId,
                                                                      int candidateLookUpOfficeId,
                                                                      List<Models.Candidate> candidates,
                                                                      List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates,
                                                                      List<Models.ElectionOffice> candidateOffices,
                                                                      Models.Party party,
                                                                      Models.OfficeHolder officeHolder)
        {
            // get candidate/running mate information
            return new CandidateSummaryViewModel()
            {
                SelectedCandidateId = candidateLookUpId,
                SelectedCandidateVotingDateId = candidateLookUpVotingDateId,
                SelectedCandidateOfficeId = candidateLookUpOfficeId,
                CandidateSummary = new CandidateSummary()
                {
                    CandidateId = candidates[0].Id,
                    FirstName = candidates[0].FirstName,
                    MiddleName = candidates[0].MiddleName,
                    LastName = candidates[0].LastName,
                    Suffix = candidates[0].Suffix,
                    OfficeName = GetOfficeNameForOfficeIdFromDatabase(candidateOffices[0].OfficeId),
                    OfficeTerm = candidateOffices[0].OfficeTerm,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.Description,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[0].Photo, voteSmartCandidates[0].Gender)
                },
                RunningMateSummary = new RunningMateSummary()
                {
                    CandidateId = candidates[1].Id,
                    RunningMateId = candidates[1].Id,
                    FirstName = candidates[1].FirstName,
                    MiddleName = candidates[1].MiddleName,
                    LastName = candidates[1].LastName,
                    Suffix = candidates[1].Suffix,
                    OfficeName = GetOfficeNameForOfficeIdFromDatabase(candidateOffices[1].OfficeId),
                    OfficeTerm = candidateOffices[1].OfficeTerm,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.Description,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[1].Photo, voteSmartCandidates[1].Gender)
                }
            };
        }
        */
         

        public CandidatePoliticalViewModel GetCandidatePoliticalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        { 
            // get candidate/running mate information from votesmart
            return new CandidatePoliticalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].Political),
                RunningMatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].Political)
            };
        }



        public CandidateCaucusViewModel GetCandidateCaucusInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        {
            // get candidate/running mate information from votesmart
            return new CandidateCaucusViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].CongMembership),
                RunningMateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].CongMembership)
            };
        }



        public CandidateProfessionalViewModel GetCandidateProfessionalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        {
            // get candidate/running mate information from votesmart
            return new CandidateProfessionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].Profession),
                RunningMateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].Profession)
            };
        }



        public CandidateEducationViewModel GetCandidateEducationInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        {
            // get candidate/running mate information from votesmart
            return new CandidateEducationViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateEducationHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].Education),
                RunningMateEducationHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].Education)
            };
        }



        public CandidateCivicViewModel GetCandidateCivicInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        {
            // get candidate/running mate information from votesmart
            return new CandidateCivicViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartCandidates[0].OrgMembership),
                RunningMateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartCandidates[1].OrgMembership)
            };
        }



        public CandidateAdditionalViewModel GetCandidateAdditionalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        {
            // get candidate/running mate information
            return new CandidateAdditionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartCandidates[0].SpecialMsg),
                RunningMateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartCandidates[1].SpecialMsg)
            };
        }



        public CandidatePersonalViewModel GetCandidatePersonalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, int candidateId, int runningMateId)
        {
            // get candidate/running mate information
            return new CandidatePersonalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateFamily = voteSmartCandidates[0].Family,
                RunningMateFamily = voteSmartCandidates[1].Family,
                CandidateGender = voteSmartCandidates[0].Gender,
                RunningMateGender = voteSmartCandidates[1].Gender,
                CandidateBirthDate = voteSmartCandidates[0].BirthDate,
                RunningMateBirthDate = voteSmartCandidates[1].BirthDate,
                CandidateBirthPlace = voteSmartCandidates[0].BirthPlace,
                RunningMateBirthPlace = voteSmartCandidates[1].BirthPlace,
                CandidateHomeCity = voteSmartCandidates[0].HomeCity,
                RunningMateHomeCity = voteSmartCandidates[1].HomeCity,
                CandidateHomeState = voteSmartCandidates[0].HomeState,
                RunningMateHomeState = voteSmartCandidates[1].HomeState,
                CandidateReligion = voteSmartCandidates[0].Religion,
                RunningMateReligion = voteSmartCandidates[1].Religion
            };
        }



        // *****************************************************************



        public CandidateCompareDisplayViewModel GetCandidateCompareDisplayViewModel(CandidateCompareViewModel candidateCompareVM)
        {
            // load variables passed in
            CandidateCompareDisplayViewModel displayViewModel = new CandidateCompareDisplayViewModel(candidateCompareVM);

            // load summary view models with objects
            CandidateCompareSummaryViewModel compareSummaryVM = GetCandidateCompareSummaryViewModel(displayViewModel, candidateCompareVM.CandidateSecondCompareCount);

            // get votesmart info for both groups of candidates/runningmates
            List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary = GetCompareFirstCandidateAndRunningMateInformationFromVoteSmart(compareSummaryVM.CandidateCompareSummaryFirstViewModel);
            List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary = GetCompareSecondCandidateAndRunningMateInformationFromVoteSmart(compareSummaryVM.CandidateCompareSummarySecondViewModel);

            compareSummaryVM.CandidateCompareSummaryFirstViewModel.CandidateCompareSummaryFirst.VoteSmartCandidateId = firstVoteSmartCandidateSummary[0].CandidateId;
            compareSummaryVM.CandidateCompareSummaryFirstViewModel.CandidateCompareSummaryFirst.VoteSmartPhotoUrl = firstVoteSmartCandidateSummary[0].Photo;
            compareSummaryVM.CandidateCompareSummaryFirstViewModel.RunningMateCompareSummaryFirst.VoteSmartCandidateId = firstVoteSmartCandidateSummary[1].CandidateId;
            compareSummaryVM.CandidateCompareSummaryFirstViewModel.RunningMateCompareSummaryFirst.VoteSmartPhotoUrl = firstVoteSmartCandidateSummary[1].Photo;
            if (compareSummaryVM.CandidateCompareSummarySecondViewModel.CandidateCompareSummarySecond != null)
            {
                compareSummaryVM.CandidateCompareSummarySecondViewModel.CandidateCompareSummarySecond.VoteSmartCandidateId = secondVoteSmartCandidateSummary[0].CandidateId;
                compareSummaryVM.CandidateCompareSummarySecondViewModel.CandidateCompareSummarySecond.VoteSmartPhotoUrl = secondVoteSmartCandidateSummary[0].Photo;
                compareSummaryVM.CandidateCompareSummarySecondViewModel.RunningMateCompareSummarySecond.VoteSmartCandidateId = secondVoteSmartCandidateSummary[1].CandidateId;
                compareSummaryVM.CandidateCompareSummarySecondViewModel.RunningMateCompareSummarySecond.VoteSmartPhotoUrl = secondVoteSmartCandidateSummary[1].Photo;
            }

            // OfficeSummaryViewModel
            displayViewModel.CandidateCompareSummaryViewModel = compareSummaryVM;
            displayViewModel.CandidateComparePoliticalViewModel = GetCandidateComparePoliticalViewModel(compareSummaryVM, firstVoteSmartCandidateSummary, secondVoteSmartCandidateSummary);
            displayViewModel.CandidateCompareCaucusViewModel = GetCandidateCompareCaucusViewModel(compareSummaryVM, firstVoteSmartCandidateSummary, secondVoteSmartCandidateSummary);
            displayViewModel.CandidateCompareProfessionalViewModel = GetCandidateCompareProfessionalViewModel(compareSummaryVM, firstVoteSmartCandidateSummary, secondVoteSmartCandidateSummary);
            displayViewModel.CandidateCompareEducationViewModel = GetCandidateCompareEducationViewModel(compareSummaryVM, firstVoteSmartCandidateSummary, secondVoteSmartCandidateSummary);
            displayViewModel.CandidateComparePersonalViewModel = GetCandidateComparePersonalViewModel(compareSummaryVM, firstVoteSmartCandidateSummary, secondVoteSmartCandidateSummary);
            // ContactViewModel
            displayViewModel.CandidateCompareCivicViewModel = GetCandidateCompareCivicViewModel(compareSummaryVM, firstVoteSmartCandidateSummary, secondVoteSmartCandidateSummary);
            displayViewModel.CandidateCompareAdditionalViewModel = GetCandidateCompareAdditionalViewModel(compareSummaryVM, firstVoteSmartCandidateSummary, secondVoteSmartCandidateSummary);
            
            return displayViewModel;
        }



        // *************************************************************************************



        public CandidateCompareSummaryViewModel GetCandidateCompareSummaryViewModel(CandidateCompareDisplayViewModel viewModel, int totalNumberOfCandidates)
        {
            // load variables passed in
            CandidateCompareSummaryViewModel summaryViewModel = new CandidateCompareSummaryViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId,
                VotingDate = viewModel.VotingDate 
            };

            summaryViewModel.CandidateCompareSummaryFirstViewModel = GetCandidateCompareSummaryFirstViewModel(viewModel, totalNumberOfCandidates);
            summaryViewModel.CandidateCompareSummarySecondViewModel = GetCandidateCompareSummarySecondViewModel(viewModel, totalNumberOfCandidates);
            summaryViewModel.CandidateCompareSummaryLookUpViewModel = GetCandidateCompareLookUpSecondViewModel(viewModel.CandidateFirstDisplayId, viewModel.VotingDateId, viewModel.OfficeId);

            return summaryViewModel;
        }



        public CandidateCompareSummaryFirstViewModel GetCandidateCompareSummaryFirstViewModel(CandidateCompareDisplayViewModel viewModel, int totalNumberOfCandidates)
        {
            // Get candidate/running mate objects for view model
            CandidateSummaryViewModel summaryVM = GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(viewModel.CandidateFirstDisplayId, viewModel.VotingDateId);
            if (summaryVM == null || summaryVM.CandidateSummary == null) { return new CandidateCompareSummaryFirstViewModel(); }

            int candidateId = ValidateAndReturnInteger(summaryVM.CandidateSummary.CandidateId);

            if (summaryVM.RunningMateSummary == null)
            {
                summaryVM.RunningMateSummary = new RunningMateSummary()
                {
                    CandidateId = 0,
                    VoteSmartCandidateId = "0"
                };
            }
            
            return new CandidateCompareSummaryFirstViewModel(summaryVM, totalNumberOfCandidates, viewModel.CandidateSecondDisplayId);
        }



        public CandidateCompareSummarySecondViewModel GetCandidateCompareSummarySecondViewModel(CandidateCompareDisplayViewModel viewModel, int totalNumberOfCandidates)
        {
            // make sure second candidate has been selected
            if (viewModel.CandidateSecondDisplayId <= 0)
            {
                return new CandidateCompareSummarySecondViewModel(viewModel, totalNumberOfCandidates);
            }

            // Get candidate/running mate objects for view model
            CandidateSummaryViewModel summaryVM = GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(viewModel.CandidateSecondDisplayId, viewModel.VotingDateId);

            if (summaryVM == null || summaryVM.CandidateSummary == null)
            {
                return new CandidateCompareSummarySecondViewModel(viewModel, totalNumberOfCandidates);
            }

            int candidateId = ValidateAndReturnInteger(summaryVM.CandidateSummary.CandidateId);

            if (summaryVM.RunningMateSummary == null)
            {
                summaryVM.RunningMateSummary = new RunningMateSummary()
                {
                    CandidateId = 0,
                    VoteSmartCandidateId = "0"
                };
            }

            return new CandidateCompareSummarySecondViewModel(summaryVM, viewModel.CandidateFirstDisplayId, viewModel.CandidateSecondDisplayId, totalNumberOfCandidates);
        }



        public List<int> GetCandidateDisplayIdList (CandidateCompareDisplayViewModel viewModel)
        {
            // get Id for first candidate (or running mate) displayed 
            // AND second candidate (or running mate) displayed
            List<int> displayIDList = new List<int>();
            displayIDList.Add(viewModel.CandidateFirstDisplayId);
            displayIDList.Add(viewModel.CandidateSecondDisplayId);

            return displayIDList;
        }
        public CandidateCompareSummaryFirstViewModel GetCandidateCompareSummaryFirst(List<int> candidateDisplayIdList, CandidateCompareDisplayViewModel viewModel,
                                                                                        List<Models.Candidate> candidates, List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates,
                                                                                        List<Models.ElectionOffice> candidateOffices, Models.Party party,
                                                                                        Models.OfficeHolder officeHolder)
        {
            return new CandidateCompareSummaryFirstViewModel()
            {
                CandidateFirstDisplayId = candidateDisplayIdList[0],
                CandidateSecondDisplayId = candidateDisplayIdList[1],
                TotalNumberOfCandidates = viewModel.CandidateSecondCompareCount,
                VotingDateId = viewModel.VotingDateId,
                OfficeId = viewModel.OfficeId,
                voteSmartCandidates = voteSmartCandidates,

                CandidateCompareSummaryFirst = new CandidateCompareSummaryFirst()
                {
                    CandidateId = candidates[0].Id,
                    FirstName = candidates[0].FirstName,
                    MiddleName = candidates[0].MiddleName,
                    LastName = candidates[0].LastName,
                    Suffix = candidates[0].Suffix,
                    OfficeName = GetOfficeNameForOfficeIdFromDatabase(candidateOffices[0].OfficeId),
                    OfficeTerm = candidateOffices[0].OfficeTerm,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.Description,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[0].Photo, voteSmartCandidates[0].Gender)
                },
                RunningMateCompareSummaryFirst = new RunningMateCompareSummaryFirst()
                {
                    CandidateId = candidates[1].Id,
                    FirstName = candidates[1].FirstName,
                    MiddleName = candidates[1].MiddleName,
                    LastName = candidates[1].LastName,
                    Suffix = candidates[1].Suffix,
                    OfficeName = GetOfficeNameForOfficeIdFromDatabase(candidateOffices[1].OfficeId),
                    OfficeTerm = candidateOffices[1].OfficeTerm,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.Description,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[1].Photo, voteSmartCandidates[1].Gender)
                }
            };
        }
        public CandidateCompareSummarySecondViewModel GetCandidateCompareSummarySecond(List<int> candidateDisplayIdList, CandidateCompareDisplayViewModel viewModel,
                                                                                        List<Models.Candidate> candidates, List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates,
                                                                                        List<Models.ElectionOffice> candidateOffices, Models.Party party,
                                                                                        Models.OfficeHolder officeHolder)
        {
            return new CandidateCompareSummarySecondViewModel()
            {
                CandidateFirstDisplayId = candidateDisplayIdList[0],
                CandidateSecondDisplayId = candidateDisplayIdList[1],
                TotalNumberOfCandidates = viewModel.CandidateSecondCompareCount,
                VotingDateId = viewModel.VotingDateId,
                OfficeId = viewModel.OfficeId,
                voteSmartCandidates = voteSmartCandidates,

                CandidateCompareSummarySecond = new CandidateCompareSummarySecond()
                {
                    CandidateId = candidates[0].Id,
                    FirstName = candidates[0].FirstName,
                    MiddleName = candidates[0].MiddleName,
                    LastName = candidates[0].LastName,
                    Suffix = candidates[0].Suffix,
                    OfficeName = GetOfficeNameForOfficeIdFromDatabase(candidateOffices[0].OfficeId),
                    OfficeTerm = candidateOffices[0].OfficeTerm,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.Description,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[0].Photo, voteSmartCandidates[0].Gender),
                },
                RunningMateCompareSummarySecond = new RunningMateCompareSummarySecond()
                {
                    CandidateId = candidates[1].Id,
                    FirstName = candidates[1].FirstName,
                    MiddleName = candidates[1].MiddleName,
                    LastName = candidates[1].LastName,
                    Suffix = candidates[1].Suffix,
                    OfficeName = GetOfficeNameForOfficeIdFromDatabase(candidateOffices[1].OfficeId),
                    OfficeTerm = candidateOffices[1].OfficeTerm,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.Description,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[1].Photo, voteSmartCandidates[1].Gender)
                }
            };
        }



        // ********************************************************************************************



        public CandidateComparePoliticalViewModel GetCandidateComparePoliticalViewModel(CandidateCompareSummaryViewModel compareSummaryVM, List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary, List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary)
        {
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0 && secondVoteSmartCandidateSummary != null && secondVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateComparePoliticalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateComparePoliticalFirstViewModel = new CandidateComparePoliticalFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].Political), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].Political), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateComparePoliticalSecondViewModel = new CandidateComparePoliticalSecondViewModel(GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[0].Political), GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[1].Political), compareSummaryVM.CandidateCompareSummarySecondViewModel),
                };
            }
            else if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateComparePoliticalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateComparePoliticalFirstViewModel = new CandidateComparePoliticalFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].Political), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].Political), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateComparePoliticalSecondViewModel = new CandidateComparePoliticalSecondViewModel()
                };
            }

            return new CandidateComparePoliticalViewModel()
            {
                CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                CandidateComparePoliticalFirstViewModel = new CandidateComparePoliticalFirstViewModel(),
                CandidateComparePoliticalSecondViewModel = new CandidateComparePoliticalSecondViewModel()
            };
        }



        public CandidateComparePoliticalFirstViewModel GetCandidateComparePoliticalFirstViewModel(CandidateCompareSummaryFirstViewModel viewModel)
        {
            return new CandidateComparePoliticalFirstViewModel()
            {
                CandidateDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateId = viewModel.CandidateCompareSummaryFirst.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummaryFirst.CandidateId,
                CandidatePoliticalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].Political),
                RunningMatePoliticalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].Political)
            };
        }
        public CandidateComparePoliticalSecondViewModel GetCandidateComparePoliticalSecondViewModel(CandidateCompareSummarySecondViewModel viewModel)
        {
            return new CandidateComparePoliticalSecondViewModel()
            {
                CandidateDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateId = viewModel.CandidateCompareSummarySecond.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummarySecond.CandidateId,
                CandidatePoliticalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].Political),
                RunningMatePoliticalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].Political)
            };
        }



        public CandidateCompareCaucusViewModel GetCandidateCompareCaucusViewModel(CandidateCompareSummaryViewModel compareSummaryVM, List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary, List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary)
        {
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0 && secondVoteSmartCandidateSummary != null && secondVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareCaucusViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareCaucusFirstViewModel = new CandidateCompareCaucusFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].CongMembership), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].CongMembership), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareCaucusSecondViewModel = new CandidateCompareCaucusSecondViewModel(GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[0].CongMembership), GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[1].CongMembership), compareSummaryVM.CandidateCompareSummarySecondViewModel),
                };
            }
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareCaucusViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareCaucusFirstViewModel = new CandidateCompareCaucusFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].CongMembership), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].CongMembership), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareCaucusSecondViewModel = new CandidateCompareCaucusSecondViewModel()
                };
            }

            return new CandidateCompareCaucusViewModel()
            {
                CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                CandidateCompareCaucusFirstViewModel = new CandidateCompareCaucusFirstViewModel(),
                CandidateCompareCaucusSecondViewModel = new CandidateCompareCaucusSecondViewModel()
            };
        }



        public CandidateCompareCaucusFirstViewModel GetCandidateCompareCaucusFirstViewModel(CandidateCompareSummaryFirstViewModel viewModel)
        {
            return new CandidateCompareCaucusFirstViewModel()
            {
                CandidateDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateId = viewModel.CandidateCompareSummaryFirst.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummaryFirst.CandidateId,
                CandidateCaucusHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].CongMembership),
                RunningMateCaucusHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].CongMembership)
            };
        }
        public CandidateCompareCaucusSecondViewModel GetCandidateCompareCaucusSecondViewModel(CandidateCompareSummarySecondViewModel viewModel)
        {
            return new CandidateCompareCaucusSecondViewModel()
            {
                CandidateDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateId = viewModel.CandidateCompareSummarySecond.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummarySecond.CandidateId,
                CandidateCaucusHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].CongMembership),
                RunningMateCaucusHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].CongMembership)
            };
        }



        public CandidateCompareProfessionalViewModel GetCandidateCompareProfessionalViewModel(CandidateCompareSummaryViewModel compareSummaryVM, List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary, List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary)
        {
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0 && secondVoteSmartCandidateSummary != null && secondVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareProfessionalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareProfessionalFirstViewModel = new CandidateCompareProfessionalFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].Profession), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].Profession), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareProfessionalSecondViewModel = new CandidateCompareProfessionalSecondViewModel(GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[0].Profession), GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[1].Profession), compareSummaryVM.CandidateCompareSummarySecondViewModel),
                };
            }
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareProfessionalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareProfessionalFirstViewModel = new CandidateCompareProfessionalFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].Profession), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].Profession), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareProfessionalSecondViewModel = new CandidateCompareProfessionalSecondViewModel()
                };
            }

            return new CandidateCompareProfessionalViewModel()
            {
                CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                CandidateCompareProfessionalFirstViewModel = new CandidateCompareProfessionalFirstViewModel(),
                CandidateCompareProfessionalSecondViewModel = new CandidateCompareProfessionalSecondViewModel()
            };
        }



        public CandidateCompareProfessionalFirstViewModel GetCandidateCompareProfessionalFirstViewModel(CandidateCompareSummaryFirstViewModel viewModel)
        {
            return new CandidateCompareProfessionalFirstViewModel()
            {
                CandidateDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateId = viewModel.CandidateCompareSummaryFirst.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummaryFirst.CandidateId,
                CandidateProfessionalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].Profession),
                RunningMateProfessionalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].Profession)
            };
        }
        public CandidateCompareProfessionalSecondViewModel GetCandidateCompareProfessionalSecondViewModel(CandidateCompareSummarySecondViewModel viewModel)
        {
            return new CandidateCompareProfessionalSecondViewModel()
            {
                CandidateDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateId = viewModel.CandidateCompareSummarySecond.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummarySecond.CandidateId,
                CandidateProfessionalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].Profession),
                RunningMateProfessionalHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].Profession)
            };
        }



        public CandidateCompareEducationViewModel GetCandidateCompareEducationViewModel(CandidateCompareSummaryViewModel compareSummaryVM, List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary, List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary)
        {
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0 && secondVoteSmartCandidateSummary != null && secondVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareEducationViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareEducationFirstViewModel = new CandidateCompareEducationFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].Education), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].Education), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareEducationSecondViewModel = new CandidateCompareEducationSecondViewModel(GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[0].Education), GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[1].Education), compareSummaryVM.CandidateCompareSummarySecondViewModel),
                };
            }
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareEducationViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareEducationFirstViewModel = new CandidateCompareEducationFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].Education), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].Education), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareEducationSecondViewModel = new CandidateCompareEducationSecondViewModel()
                };
            }

            return new CandidateCompareEducationViewModel()
            {
                CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                CandidateCompareEducationFirstViewModel = new CandidateCompareEducationFirstViewModel(),
                CandidateCompareEducationSecondViewModel = new CandidateCompareEducationSecondViewModel()
            };
        }



        public CandidateCompareEducationFirstViewModel GetCandidateCompareEducationFirstViewModel(CandidateCompareSummaryFirstViewModel viewModel)
        {
            return new CandidateCompareEducationFirstViewModel()
            {
                CandidateDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateId = viewModel.CandidateCompareSummaryFirst.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummaryFirst.CandidateId,
                CandidateEducationHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].Education),
                RunningMateEducationHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].Education)
            };
        }
        public CandidateCompareEducationSecondViewModel GetCandidateCompareEducationSecondViewModel(CandidateCompareSummarySecondViewModel viewModel)
        {
            return new CandidateCompareEducationSecondViewModel()
            {
                CandidateDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateId = viewModel.CandidateCompareSummarySecond.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummarySecond.CandidateId,
                CandidateEducationHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].Education),
                RunningMateEducationHistory = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].Education)
            };
        }



        public CandidateComparePersonalViewModel GetCandidateComparePersonalViewModel(CandidateCompareSummaryViewModel compareSummaryVM, List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary, List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary)
        {
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0 && secondVoteSmartCandidateSummary != null && secondVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateComparePersonalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateComparePersonalFirstViewModel = new CandidateComparePersonalFirstViewModel(firstVoteSmartCandidateSummary[0], firstVoteSmartCandidateSummary[1], compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateComparePersonalSecondViewModel = new CandidateComparePersonalSecondViewModel(secondVoteSmartCandidateSummary[0], secondVoteSmartCandidateSummary[1], compareSummaryVM.CandidateCompareSummarySecondViewModel),
                };
            }
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateComparePersonalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateComparePersonalFirstViewModel = new CandidateComparePersonalFirstViewModel(firstVoteSmartCandidateSummary[0], firstVoteSmartCandidateSummary[1], compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateComparePersonalSecondViewModel = new CandidateComparePersonalSecondViewModel()
                };
            }

            return new CandidateComparePersonalViewModel()
            {
                CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                CandidateComparePersonalFirstViewModel = new CandidateComparePersonalFirstViewModel(),
                CandidateComparePersonalSecondViewModel = new CandidateComparePersonalSecondViewModel()
            };
        }



        public CandidateComparePersonalFirstViewModel GetCandidateComparePersonalFirstViewModel(CandidateCompareSummaryFirstViewModel viewModel)
        {
            return new CandidateComparePersonalFirstViewModel()
            {
                CandidateDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateId = viewModel.CandidateCompareSummaryFirst.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummaryFirst.CandidateId,

                CandidateFamily = viewModel.voteSmartCandidates[0].Family,
                RunningMateFamily = viewModel.voteSmartCandidates[1].Family,
                CandidateGender = viewModel.voteSmartCandidates[0].Gender,
                RunningMateGender = viewModel.voteSmartCandidates[1].Gender,
                CandidateBirthDate = viewModel.voteSmartCandidates[0].BirthDate,
                RunningMateBirthDate = viewModel.voteSmartCandidates[1].BirthDate,
                CandidateBirthPlace = viewModel.voteSmartCandidates[0].BirthPlace,
                RunningMateBirthPlace = viewModel.voteSmartCandidates[1].BirthPlace,
                CandidateHomeCity = viewModel.voteSmartCandidates[0].HomeCity,
                RunningMateHomeCity = viewModel.voteSmartCandidates[1].HomeCity,
                CandidateHomeState = viewModel.voteSmartCandidates[0].HomeState,
                RunningMateHomeState = viewModel.voteSmartCandidates[1].HomeState,
                CandidateReligion = viewModel.voteSmartCandidates[0].Religion,
                RunningMateReligion = viewModel.voteSmartCandidates[1].Religion
            };
        }
        public CandidateComparePersonalSecondViewModel GetCandidateComparePersonalSecondViewModel(CandidateCompareSummarySecondViewModel viewModel)
        {
            return new CandidateComparePersonalSecondViewModel()
            {
                CandidateDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateId = viewModel.CandidateCompareSummarySecond.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummarySecond.CandidateId,

                CandidateFamily = viewModel.voteSmartCandidates[0].Family,
                RunningMateFamily = viewModel.voteSmartCandidates[1].Family,
                CandidateGender = viewModel.voteSmartCandidates[0].Gender,
                RunningMateGender = viewModel.voteSmartCandidates[1].Gender,
                CandidateBirthDate = viewModel.voteSmartCandidates[0].BirthDate,
                RunningMateBirthDate = viewModel.voteSmartCandidates[1].BirthDate,
                CandidateBirthPlace = viewModel.voteSmartCandidates[0].BirthPlace,
                RunningMateBirthPlace = viewModel.voteSmartCandidates[1].BirthPlace,
                CandidateHomeCity = viewModel.voteSmartCandidates[0].HomeCity,
                RunningMateHomeCity = viewModel.voteSmartCandidates[1].HomeCity,
                CandidateHomeState = viewModel.voteSmartCandidates[0].HomeState,
                RunningMateHomeState = viewModel.voteSmartCandidates[1].HomeState,
                CandidateReligion = viewModel.voteSmartCandidates[0].Religion,
                RunningMateReligion = viewModel.voteSmartCandidates[1].Religion
            };
        }



        public CandidateCompareCivicViewModel GetCandidateCompareCivicViewModel(CandidateCompareSummaryViewModel compareSummaryVM, List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary, List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary)
        {
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0 && secondVoteSmartCandidateSummary != null && secondVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareCivicViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareCivicFirstViewModel = new CandidateCompareCivicFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].OrgMembership), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].OrgMembership), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareCivicSecondViewModel = new CandidateCompareCivicSecondViewModel(GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[0].OrgMembership), GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[1].OrgMembership), compareSummaryVM.CandidateCompareSummarySecondViewModel),
                };
            }
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareCivicViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareCivicFirstViewModel = new CandidateCompareCivicFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].OrgMembership), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].OrgMembership), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareCivicSecondViewModel = new CandidateCompareCivicSecondViewModel()
                };
            }

            return new CandidateCompareCivicViewModel()
            {
                CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                CandidateCompareCivicFirstViewModel = new CandidateCompareCivicFirstViewModel(),
                CandidateCompareCivicSecondViewModel = new CandidateCompareCivicSecondViewModel()
            };
        }



        public CandidateCompareCivicFirstViewModel GetCandidateCompareCivicFirstViewModel(CandidateCompareSummaryFirstViewModel viewModel)
        {
            return new CandidateCompareCivicFirstViewModel()
            {
                CandidateDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateId = viewModel.CandidateCompareSummaryFirst.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummaryFirst.CandidateId,
                CandidateCivicMemberships = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].OrgMembership),
                RunningMateCivicMemberships = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].OrgMembership)
            };
        }
        public CandidateCompareCivicSecondViewModel GetCandidateCompareCivicSecondViewModel(CandidateCompareSummarySecondViewModel viewModel)
        {
            return new CandidateCompareCivicSecondViewModel()
            {
                CandidateDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateId = viewModel.CandidateCompareSummarySecond.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummarySecond.CandidateId,
                CandidateCivicMemberships = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].OrgMembership),
                RunningMateCivicMemberships = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].OrgMembership)
            };
        }



        public CandidateCompareAdditionalViewModel GetCandidateCompareAdditionalViewModel(CandidateCompareSummaryViewModel compareSummaryVM, List<ViewModels.VoteSmart.CandidateBio> firstVoteSmartCandidateSummary, List<ViewModels.VoteSmart.CandidateBio> secondVoteSmartCandidateSummary)
        {
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0 && secondVoteSmartCandidateSummary != null && secondVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareAdditionalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareAdditionalFirstViewModel = new CandidateCompareAdditionalFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].SpecialMsg), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].SpecialMsg), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareAdditionalSecondViewModel = new CandidateCompareAdditionalSecondViewModel(GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[0].SpecialMsg), GetListFromStringWithLineBreaks(secondVoteSmartCandidateSummary[1].SpecialMsg), compareSummaryVM.CandidateCompareSummarySecondViewModel),
                };
            }
            if (firstVoteSmartCandidateSummary != null && firstVoteSmartCandidateSummary.Count > 0)
            {
                return new CandidateCompareAdditionalViewModel()
                {
                    CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                    CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                    CandidateCompareAdditionalFirstViewModel = new CandidateCompareAdditionalFirstViewModel(GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[0].SpecialMsg), GetListFromStringWithLineBreaks(firstVoteSmartCandidateSummary[1].SpecialMsg), compareSummaryVM.CandidateCompareSummaryFirstViewModel),
                    CandidateCompareAdditionalSecondViewModel = new CandidateCompareAdditionalSecondViewModel()
                };
            }

            return new CandidateCompareAdditionalViewModel()
            {
                CandidateFirstDisplayId = compareSummaryVM.CandidateFirstDisplayId,
                CandidateSecondDisplayId = compareSummaryVM.CandidateSecondDisplayId,
                CandidateCompareAdditionalFirstViewModel = new CandidateCompareAdditionalFirstViewModel(),
                CandidateCompareAdditionalSecondViewModel = new CandidateCompareAdditionalSecondViewModel()
            };
        }



        public CandidateCompareAdditionalFirstViewModel GetCandidateCompareAdditionalFirstViewModel(CandidateCompareSummaryFirstViewModel viewModel)
        {
            return new CandidateCompareAdditionalFirstViewModel()
            {
                CandidateDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateId = viewModel.CandidateCompareSummaryFirst.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummaryFirst.CandidateId,
                CandidateAdditionalInformation = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].SpecialMsg),
                RunningMateAdditionalInformation = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].SpecialMsg)
            };
        }
        public CandidateCompareAdditionalSecondViewModel GetCandidateCompareAdditionalSecondViewModel(CandidateCompareSummarySecondViewModel viewModel)
        {
            return new CandidateCompareAdditionalSecondViewModel()
            {
                CandidateDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateId = viewModel.CandidateCompareSummarySecond.CandidateId,
                RunningMateId = viewModel.RunningMateCompareSummarySecond.CandidateId,
                CandidateAdditionalInformation = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[0].SpecialMsg),
                RunningMateAdditionalInformation = GetListFromStringWithLineBreaks(viewModel.voteSmartCandidates[1].SpecialMsg)
            };
        }



        public ViewModels.VoteSmart.CandidateBio GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(string voteSmartCandidateId)
        {
            VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();
            return voteSmart.GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(voteSmartCandidateId);
        }



        // *********************************************************************



        public string GetValidImageLocationToDisplay(string thirdPartyImageUrl, string gender)
        {
            if (string.IsNullOrEmpty(thirdPartyImageUrl)) { return GetGenderImageLocationToDisplay(gender); }

            return thirdPartyImageUrl;
        }


        public string GetGenderImageLocationToDisplay(string gender)
        {
            if (gender == "F" || gender == "Female")
            {
                return "~/Content/images/image_female.png";
            }
            else
            {
                return "~/Content/images/image_male.png";
            }
        }



        public List<string> GetListFromStringWithLineBreaks(string stringWithLineBreaks)
        {
            List<string> stringList = new List<string>();
            string[] lines = stringWithLineBreaks.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (var x in lines)
            {
                stringList.Add(x);
            }

            if (stringList.Count == 0)
            {
                stringList.Add("No information on file.");
            }
            else if (stringList.Count == 1 && stringList[0] == "")
            {
                stringList[0] = "No information on file.";
            }

            return stringList;
        }



        // *************************************************************************



        public Models.ElectionCandidate GetCandidateSummaryForCurrentElectionDateFromDatabase(int candidateLookUpId)
        {
            Models.ElectionCandidate dbCandidate = new Models.ElectionCandidate();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidate = db.ElectionCandidates.FirstOrDefault(x => x.CandidateId == candidateLookUpId);
            }

            if (dbCandidate == null)
                return new Models.ElectionCandidate();

            return dbCandidate;
        }



        public Models.ElectionCandidate GetRunningMateSummaryForCurrentElectionDateFromDatabase(int candidateLookUpId)
        {
            Models.ElectionCandidate dbRunningMate = new Models.ElectionCandidate();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbRunningMate = db.ElectionCandidates.FirstOrDefault(x => x.RunningMateId == candidateLookUpId);
            }

            if (dbRunningMate == null)
                return new Models.ElectionCandidate();

            return dbRunningMate;
        }



        // *************************************************************


        
        public CandidateCompareSummaryLookUpViewModel GetCandidateCompareLookUpSecondViewModel(int candidateFirstDisplayId, int votingDateId, int officeId)
        {
            return new CandidateCompareSummaryLookUpViewModel()
            {
                CandidateFirstDisplayId = candidateFirstDisplayId,
                VotingDateId = votingDateId,
                OfficeId = officeId,
                CandidateNames = GetCandidateCompareListItems(candidateFirstDisplayId, votingDateId, officeId)
            };
        }



        /// <summary>
        /// get the name for candidate for supplied candidate ID
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        public CandidateSummary GetCandidateSummaryForCandidateIdFromDatabase(int candidateId)
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                Models.Candidate candidateDTO = db.Candidates.Find(candidateId);

                if (candidateDTO == null) { return new CandidateSummary(); }

                return new CandidateSummary(candidateDTO);
            }
        }


        public RunningMateSummary GetRunningMateSummaryForCandidateIdFromDatabase(int candidateId)
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                Models.Candidate candidateDTO = db.Candidates.Find(candidateId);

                if (candidateDTO == null) { return new RunningMateSummary(); }

                return new RunningMateSummary(candidateDTO);
            }
        }



        private Models.ElectionVotingDate GetOldestVotingDate()
        {
            List<Models.ElectionVotingDate> dbDates = GetActiveElectionDatesInOrderByDateFromDatabase();

            Models.ElectionVotingDate date = dbDates.FirstOrDefault(x => x.Active == true);

            if (date == null)
            {
                return new Models.ElectionVotingDate();
            }
            else
            {
                return date;
            }

        }


        private int GetCurrentVotingDateIdfromDatabase()
        {
            using (Models.OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.ElectionVotingDate> dbDates = context.ElectionVotingDates.Where(x => x.Active == true).OrderBy(x => x.Date).ToList();
                if (dbDates == null) { return 0; }

                if (dbDates.Count == 0) { return 0; }

                return dbDates[0].Id;
            }
        }



        /// <summary>
        /// get a list of active election dates from database
        /// </summary>
        /// <returns></returns>
        public List<Models.ElectionVotingDate> GetActiveElectionDatesInOrderByDateFromDatabase()
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                List<Models.ElectionVotingDate> electionVotingDateDTO = db.ElectionVotingDates.Where(x => x.Active == true).OrderBy(x => x.Date).ToList();
                
                if (electionVotingDateDTO == null)
                {
                    return new List<ElectionVotingDate>();
                }

                return electionVotingDateDTO;
            }
        }




        public Models.ElectionVotingDate GetDateInformationForDateIdFromDatabase(int dateId)
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                return db.ElectionVotingDates.Find(dateId);
            }
        }





        private Models.ElectionOffice GetOfficeInformationForOfficeId(int officeId)
        {
            Models.ElectionOffice dbOffice = new ElectionOffice();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbOffice = db.ElectionOffices.Find(officeId);

                if (dbOffice == null)
                    return new Models.ElectionOffice();

                return dbOffice;
            }
        }






        // ************************
        // Candidate Drop Down List
        // ************************



        private CandidateDropDownList GetCandidatesForDropDownList(int dateId)
        {
            return new CandidateDropDownList()
            {
                CandidateNames = GetCandidateListItems(dateId)
            };
        }


        /*
        private CandidateCompareDropDownList GetCandidatesToCompareForDropDownList(int candidateFirstDisplayId, int dateId, int officeId)
        {
            return new CandidateCompareDropDownList()
            {
                CandidateNames = GetCandidateCompareListItems(candidateFirstDisplayId, dateId, officeId)
            };
        }
        */


        // *****************************************************************



        private IEnumerable<SelectListItem> GetCandidateListItems(int dateId)
        {
            if (dateId <= 0)
            {
                return new List<SelectListItem>();
            }

            List<int> dbCandidateIds = GetCandidatesForCurrentDateFromDatabase(dateId);
            List<Models.Candidate> dboCandidates = GetCandidateNamesForCandidateIdFromDatabase(dbCandidateIds);
            List<SelectListItem> candidates = new List<SelectListItem>();

            for (int i = 0; i < dboCandidates.Count; i++)
            {
                string candidateName = string.Format("{0} {1}", dboCandidates[i].FirstName, dboCandidates[i].LastName);

                candidates.Add(new SelectListItem()
                {
                    Value = dboCandidates[i].Id.ToString(),
                    Text = candidateName
                });
            }

            return candidates;
        }



        private IEnumerable<SelectListItem> GetCandidateCompareListItems(int candidateFirstDisplayId, int dateId, int officeId)
        {
            if (dateId <= 0 || officeId <= 0 || candidateFirstDisplayId <= 0)
                return new List<SelectListItem>();

            List<int> dbCandidateIds = GetCandidatesForCurrentDateAndOfficeFromDatabase(dateId, officeId);
            dbCandidateIds = RemoveSuppliedCandidateFromCandidateIdList(dbCandidateIds, candidateFirstDisplayId);
            List<Models.Candidate> dboCandidates = GetCandidateNamesForCandidateIdFromDatabase(dbCandidateIds);

            List<SelectListItem> candidates = new List<SelectListItem>();

            for (int i = 0; i < dboCandidates.Count; i++)
            {
                string candidateName = string.Format("{0} {1}", dboCandidates[i].FirstName, dboCandidates[i].LastName);

                candidates.Add(new SelectListItem()
                {
                    Value = dboCandidates[i].Id.ToString(),
                    Text = candidateName
                });
            }

            return candidates;
        }



        public List<int> RemoveSuppliedCandidateFromCandidateIdList(List<int> candidateIdList, int candidateId)
        {
            var itemToRemove = candidateIdList.Single(r => r == candidateId);
            candidateIdList.Remove(itemToRemove);
            return candidateIdList;
        }



        // ****************************************************************



        private IEnumerable<SelectListItem> GetElectionOfficeListItems(int dateId, int selectedOfficeId)
        {
            // validate input values
            if (dateId <= 0) { return new List<SelectListItem>(); }

            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                // create select list object
                List<SelectListItem> electionOffices = new List<SelectListItem>();

                // get list of offices with candidates for election
                List<ElectionCandidate> dbElectionCandidateOffices = context.ElectionCandidates.Include("ElectionOffice.Office").Where(x => x.ElectionVotingDateId == dateId)
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



        // ****************************************************************



        private List<int> GetCandidatesForCurrentDateFromDatabase(int dateId)
        {
            List<Models.ElectionCandidate> dbOffices;

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbOffices = db.ElectionCandidates.Where(x => x.ElectionVotingDateId == dateId).ToList();
            }

            if (dbOffices == null)
            {
                return new List<int>();
            }

            return dbOffices.Select(x => x.CandidateId).Distinct().ToList();
        }



        private List<int> GetCandidatesForCurrentDateAndOfficeFromDatabase(int dateId, int officeId)
        {
            List<Models.ElectionCandidate> dbCandidates;

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidates = db.ElectionCandidates.Where(x => x.ElectionVotingDateId == dateId)
                                                                    .Where(x => x.ElectionOfficeId == officeId)
                                                                    .ToList();
            }

            if (dbCandidates == null)
            {
                return new List<int>();
            }

            return dbCandidates.Select(x => x.CandidateId).Distinct().ToList();
        }



        // **********************************************************************


        /// <summary>
        /// get list of candidate names for supplied ID(s) from database
        /// </summary>
        /// <param name="candidateIds"></param>
        /// <returns></returns>
        private List<Models.Candidate> GetCandidateNamesForCandidateIdFromDatabase(List<int> candidateIds)
        {
            List<Models.Candidate> dbCandidates = new List<Models.Candidate>();
            List<Models.Candidate> candidatesDTO = new List<Models.Candidate>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidates = db.Candidates.ToList();
            }

            for (int i = 0; i < candidateIds.Count; i++)
            {
                for (int j = 0; j < dbCandidates.Count; j++)
                {
                    if (candidateIds[i] == dbCandidates[j].Id)
                    {
                        candidatesDTO.Add(new Models.Candidate()
                        {
                            Id = dbCandidates[j].Id,
                            FirstName = dbCandidates[j].FirstName,
                            LastName = dbCandidates[j].LastName
                        });

                        j = dbCandidates.Count;
                    }
                }
            }

            return candidatesDTO.OrderBy(x => x.MiddleName).OrderBy(x => x.FirstName).OrderBy(x => x.LastName).ToList();
        }



        public Models.Party GetPartyNameForPartyIdFromDatabase(int partyId)
        {
            Models.Party dbParty = new Models.Party();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbParty = db.Parties.Find(partyId);
            }

            if (dbParty == null)
                return new Models.Party();

            return dbParty;
        }



        public Models.OfficeHolder GetOfficeHolderyNameForOfficeHolderIdFromDatabase(string officeHolderId)
        {
            Models.OfficeHolder dbOfficeHolder = new Models.OfficeHolder();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbOfficeHolder = db.OfficeHolders.Find(officeHolderId);
            }

            if (dbOfficeHolder == null)
                return new Models.OfficeHolder();

            return dbOfficeHolder;
        }




    }
}