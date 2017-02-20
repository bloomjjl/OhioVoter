﻿using OhioVoter;
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
            CandidateViewModel viewModel = new CandidateViewModel()
            {
                ControllerName = _controllerName,
                CandidateId = suppliedCandidateId
            };

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
            CandidateCompareSummaryLookUpViewModel candidateCompareLookUpSecondVM = GetCandidateCompareLookUpSecondViewModel(viewModel.CandidateFirstDisplayId, viewModel.VotingDateId, viewModel.OfficeId);
            List<SelectListItem> candidates = candidateCompareLookUpSecondVM.CandidateNames.ToList();

            // make sure candidate list found
            if (candidates == null)
            {
                viewModel.CandidateSecondCompareCount = 0;
            }
            else
            {
                viewModel.CandidateSecondCompareCount = candidates.Count;
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
                string strId = candidates[0].Value.ToString();
                if (int.TryParse(strId, out intId))
                {
                    viewModel.CandidateSecondDisplayId = int.Parse(strId);
                }
            }

            // get information for the candidateCompareDisplayViewModel
            if (viewModel.CandidateSecondCompareCount >= 1)
            {
                // load all data for compare view model
                viewModel.CandidateCompareDisplayViewModel = GetCandidateCompareDisplayViewModel(viewModel);

                return View(viewModel);
            }

            // no candidates found to compare
            return RedirectToAction("Index", "Candidate", 0);
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




        public ActionResult RemoveFirst(int? firstCandidateId, int? secondCandidateId, int? dateId, int? officeId)
        {
            // validate values provided
            int suppliedFirstCandidateId = ValidateAndReturnInteger(firstCandidateId);
            int suppliedSecondCandidateId = ValidateAndReturnInteger(secondCandidateId);
            int suppliedDateId = ValidateAndReturnInteger(dateId);
            int suppliedOfficeId = ValidateAndReturnInteger(officeId);

            // move second to first
            suppliedFirstCandidateId = suppliedSecondCandidateId;

            // clear second
            suppliedSecondCandidateId = 0;

            return RedirectToAction("Compare", "Candidate", new { firstCandidateId = suppliedFirstCandidateId, secondCandidateId = suppliedSecondCandidateId, dateId = suppliedDateId, officeId = suppliedOfficeId });
        }


        
        public ActionResult RemoveSecond(int? firstCandidateId, int? secondCandidateId, int? candidateCompareCount, int? dateId, int? officeId)
        {
            // validate values provided
            int suppliedFirstCandidateId = ValidateAndReturnInteger(firstCandidateId);
            int suppliedSecondCandidateId = ValidateAndReturnInteger(secondCandidateId);
            int suppliedCandidateCompareCount = ValidateAndReturnInteger(candidateCompareCount);
            int suppliedDateId = ValidateAndReturnInteger(dateId);
            int suppliedOfficeId = ValidateAndReturnInteger(officeId);

            // displayed candidate removed from list
            if (suppliedCandidateCompareCount > 1)
            {
                // clear candidateSecondId to display dropdownlist
                return RedirectToAction("Compare", "Candidate", new { firstCandidateId = suppliedFirstCandidateId, secondCandidateId = 0, dateId = dateId, officeId = officeId });
            }
            else
            {
                return RedirectToAction("Index", "Candidate", new { candidateId = suppliedFirstCandidateId });
            }
        }



        [ChildActionOnly]
        public ActionResult DisplayCandidateInformation(CandidateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // create empty model
                CandidateLookUpViewModel candidateLookUpVM = new CandidateLookUpViewModel()
                {
                    ControllerName = _controllerName,
                };

                return PartialView("_CandidateLookUp", candidateLookUpVM);
            }

            Models.ElectionVotingDate date = GetOldestVotingDate();

            if (model.CandidateId > 0)
            {
                // get single candidate (and runningmate) added to display model
                CandidateDisplayViewModel candidateDisplayVM = new CandidateDisplayViewModel()
                {
                    ControllerName = _controllerName,
                    CandidateLookUpId = model.CandidateId,
                    VotingDateId = date.ElectionVotingDateId,
                    VotingDate = date.Date.ToShortDateString()
                };

                candidateDisplayVM = GetCandidateDisplayViewModel(candidateDisplayVM);

                return PartialView("_CandidateDisplay", candidateDisplayVM);
            }
            else
            {
                // get list of candidates added to lookup model
                CandidateLookUpViewModel candidateLookUpVM = new CandidateLookUpViewModel()
                {
                    ControllerName = model.ControllerName,
                    VotingDate = date.Date.ToShortDateString(),
                    CandidateNames = GetCandidateListItems(date.ElectionVotingDateId)
                    //CandidateDropDownList = GetCandidatesForDropDownList(date.ElectionVotingDateId)
                };

                return PartialView("_CandidateLookUp", candidateLookUpVM);
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
            else if (viewModel.CandidateCompareSummarySecondViewModel.CandidateSecondDisplayId > 0)
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



        // ********************************************
        // update candidates displayed from Candidate List based on name provided
        // ********************************************



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

            return RedirectToAction("Index", new { candidateId = candidateId });
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
        public CandidateDisplayViewModel GetCandidateDisplayViewModel(CandidateDisplayViewModel candidateDisplayVM)
        {
            // get candidate/running mate
            List<Models.ElectionVotingDateOfficeCandidate> dbCandidates = GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(candidateDisplayVM.CandidateLookUpId);

            // get candidate lookup information
            int candidateLookUpId = candidateDisplayVM.CandidateLookUpId;
            int candidateLookUpVotingDateId = GetCandidateLookUpVotingDateId(dbCandidates, candidateLookUpId);
            int candidateLookUpOfficeId = GetCandidateLookUpOfficeId(dbCandidates, candidateLookUpId);

            // Get candidate/running mate objects for view model
            List<Models.ElectionCandidate> candidates = GetCandidateNameAndRunningMateNameFromDatabase(dbCandidates);
            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = GetCandidateAndRunningMateInformationFromVoteSmart(candidates);
            List<Models.ElectionOffice> candidateOffices = GetCandidateAndRunningMateOfficeInformation(dbCandidates);
            Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidates[0].PartyId);
            Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidates[0].OfficeHolderId);

            // load view model with objects
            candidateDisplayVM.CandidateSummaryViewModel = GetCandidateSummaryViewModel(candidateLookUpId, candidateLookUpVotingDateId, candidateLookUpOfficeId, candidates, voteSmartCandidates, candidateOffices, party, officeHolder);
            // OfficeSummaryViewModel
            candidateDisplayVM.CandidatePoliticalViewModel = GetCandidatePoliticalInformationForCandidateBioFromVoteSmart(voteSmartCandidates, candidateDisplayVM.CandidateLookUpId, candidates);
            candidateDisplayVM.CandidateCaucusViewModel = GetCandidateCaucusInformationForCandidateBioFromVoteSmart(voteSmartCandidates, candidateDisplayVM.CandidateLookUpId, candidates);
            candidateDisplayVM.CandidateProfessionalViewModel = GetCandidateProfessionalInformationForCandidateBioFromVoteSmart(voteSmartCandidates, candidateDisplayVM.CandidateLookUpId, candidates);
            candidateDisplayVM.CandidateEducationViewModel = GetCandidateEducationInformationForCandidateBioFromVoteSmart(voteSmartCandidates, candidateDisplayVM.CandidateLookUpId, candidates);
            candidateDisplayVM.CandidatePersonalViewModel = GetCandidatePersonalInformationForCandidateBioFromVoteSmart(voteSmartCandidates, candidateDisplayVM.CandidateLookUpId, candidates);
            // ContactViewModel
            candidateDisplayVM.CandidateCivicViewModel = GetCandidateCivicInformationForCandidateBioFromVoteSmart(voteSmartCandidates, candidateDisplayVM.CandidateLookUpId, candidates);
            candidateDisplayVM.CandidateAdditionalViewModel = GetCandidateAdditionalInformationForCandidateBioFromVoteSmart(voteSmartCandidates, candidateDisplayVM.CandidateLookUpId, candidates);

            return candidateDisplayVM;
        }




        public List<Models.ElectionVotingDateOfficeCandidate> GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(int candidateLookUpId)
        {
            List<Models.ElectionVotingDateOfficeCandidate> dbCandidate = new List<ElectionVotingDateOfficeCandidate>();
            Models.ElectionVotingDateOfficeCandidate dbRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId);

            if (dbRunningMate == null || dbRunningMate.CandidateId == 0)
            {// no running mate found. Must be candidate!
                // store candidate info
                dbCandidate.Add(GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId));
                if (dbCandidate == null)
                {// nothing found. create empty objects for candidate/running mate
                    dbCandidate.Add(new ElectionVotingDateOfficeCandidate());
                    dbCandidate.Add(new ElectionVotingDateOfficeCandidate());
                }
                if (dbCandidate[0].RunningMateId != 0)
                {// store running mate info
                    dbCandidate.Add(GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate[0].RunningMateId));
                }
                else
                {// create empty object for running mate
                    dbCandidate.Add(new ElectionVotingDateOfficeCandidate());
                }
            }
            else
            {// running mate == candidate lookup. 
                // store candidate for the running mate
                dbCandidate.Add(GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMate.CandidateId));
                dbCandidate.Add(GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId));
            }

            return dbCandidate;
        }



        public int GetCandidateLookUpVotingDateId(List<Models.ElectionVotingDateOfficeCandidate> dbCandidates, int candidateLookUp)
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


        public int GetCandidateLookUpOfficeId(List<Models.ElectionVotingDateOfficeCandidate> dbCandidates, int candidateLookUp)
        {
            if (dbCandidates[0] != null && dbCandidates[0].CandidateId == candidateLookUp)
            {// candidate
                return dbCandidates[0].OfficeId;
            }
            else if (dbCandidates[1] != null && dbCandidates[1].CandidateId == candidateLookUp)
            {// running mate
                return dbCandidates[1].OfficeId;
            }

            return 0;
        }



        public List<Models.ElectionCandidate> GetCandidateNameAndRunningMateNameFromDatabase(List<Models.ElectionVotingDateOfficeCandidate> dbCandidates)
        {
            // get candidate/running mate information
            List<Models.ElectionCandidate> candidates = new List<Models.ElectionCandidate>();
            candidates.Add(GetCandidateNameForCandidateIdFromDatabase(dbCandidates[0].CandidateId));
            candidates.Add(GetCandidateNameForCandidateIdFromDatabase(dbCandidates[0].RunningMateId));

            return candidates;
        }



        public List<ViewModels.VoteSmart.CandidateBio> GetCandidateAndRunningMateInformationFromVoteSmart(List<Models.ElectionCandidate> candidates)
        {
            // get candidate/running mate CandidateBio information from votesmart
            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = new List<ViewModels.VoteSmart.CandidateBio>();
            voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidates[0].VoteSmartCandidateId));
            voteSmartCandidates.Add(GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidates[1].VoteSmartCandidateId));

            return voteSmartCandidates;
        }



        public List<Models.ElectionOffice> GetCandidateAndRunningMateOfficeInformation(List<Models.ElectionVotingDateOfficeCandidate> dbCandidates)
        {
            // get candidate/running mate office information
            List<Models.ElectionOffice> candidateOffices = new List<Models.ElectionOffice>();
            candidateOffices.Add(GetOfficeInformationForOfficeId(dbCandidates[0].OfficeId));
            candidateOffices.Add(GetOfficeInformationForOfficeId(dbCandidates[1].OfficeId));

            return candidateOffices;
        }


        // ***********************************************************************



        public CandidateSummaryViewModel GetCandidateSummaryViewModel(int candidateLookUpId,
                                                                      int candidateLookUpVotingDateId,
                                                                      int candidateLookUpOfficeId,
                                                                      List<Models.ElectionCandidate> candidates,
                                                                      List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates,
                                                                      List<Models.ElectionOffice> candidateOffices,
                                                                      Models.ElectionParty party,
                                                                      Models.ElectionOfficeHolder officeHolder)
        {
            // get candidate/running mate information
            return new CandidateSummaryViewModel()
            {
                SelectedCandidateId = candidateLookUpId,
                SelectedCandidateVotingDateId = candidateLookUpVotingDateId,
                SelectedCandidateOfficeId = candidateLookUpOfficeId,
                CandidateSummary = new CandidateSummary()
                {
                    CandidateId = candidates[0].ElectionCandidateId,
                    FirstName = candidates[0].FirstName,
                    MiddleName = candidates[0].MiddleName,
                    LastName = candidates[0].LastName,
                    Suffix = candidates[0].Suffix,
                    OfficeName = candidateOffices[0].OfficeName,
                    OfficeTerm = candidateOffices[0].Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[0].Photo, voteSmartCandidates[0].Gender)
                },
                RunningMateSummary = new RunningMateSummary()
                {
                    CandidateId = candidates[1].ElectionCandidateId,
                    RunningMateId = candidates[1].ElectionCandidateId,
                    FirstName = candidates[1].FirstName,
                    MiddleName = candidates[1].MiddleName,
                    LastName = candidates[1].LastName,
                    Suffix = candidates[1].Suffix,
                    OfficeName = candidateOffices[1].OfficeName,
                    OfficeTerm = candidateOffices[1].Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[1].Photo, voteSmartCandidates[1].Gender)
                }
            };
        }

         

        public CandidatePoliticalViewModel GetCandidatePoliticalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, List<Models.ElectionCandidate> candidates)
        { 
            // get candidate/running mate information from votesmart
            return new CandidatePoliticalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidates[0].ElectionCandidateId,
                RunningMateId = candidates[1].ElectionCandidateId,
                CandidatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].Political),
                RunningMatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].Political)
            };
        }



        public CandidateCaucusViewModel GetCandidateCaucusInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, List<Models.ElectionCandidate> candidates)
        {
            // get candidate/running mate information from votesmart
            return new CandidateCaucusViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidates[0].ElectionCandidateId,
                RunningMateId = candidates[1].ElectionCandidateId,
                CandidateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].CongMembership),
                RunningMateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].CongMembership)
            };
        }



        public CandidateProfessionalViewModel GetCandidateProfessionalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, List<Models.ElectionCandidate> candidates)
        {
            // get candidate/running mate information from votesmart
            return new CandidateProfessionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidates[0].ElectionCandidateId,
                RunningMateId = candidates[1].ElectionCandidateId,
                CandidateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].Profession),
                RunningMateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].Profession)
            };
        }



        public CandidateEducationViewModel GetCandidateEducationInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, List<Models.ElectionCandidate> candidates)
        {
            // get candidate/running mate information from votesmart
            return new CandidateEducationViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidates[0].ElectionCandidateId,
                RunningMateId = candidates[1].ElectionCandidateId,
                CandidateEducationHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[0].Education),
                RunningMateEducationHistory = GetListFromStringWithLineBreaks(voteSmartCandidates[1].Education)
            };
        }



        public CandidateCivicViewModel GetCandidateCivicInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, List<Models.ElectionCandidate> candidates)
        {
            // get candidate/running mate information from votesmart
            return new CandidateCivicViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidates[0].ElectionCandidateId,
                RunningMateId = candidates[1].ElectionCandidateId,
                CandidateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartCandidates[0].OrgMembership),
                RunningMateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartCandidates[1].OrgMembership)
            };
        }



        public CandidateAdditionalViewModel GetCandidateAdditionalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, List<Models.ElectionCandidate> candidates)
        {
            // get candidate/running mate information
            return new CandidateAdditionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidates[0].ElectionCandidateId,
                RunningMateId = candidates[1].ElectionCandidateId,
                CandidateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartCandidates[0].SpecialMsg),
                RunningMateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartCandidates[1].SpecialMsg)
            };
        }



        public CandidatePersonalViewModel GetCandidatePersonalInformationForCandidateBioFromVoteSmart(List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates, int candidateLookUpId, List<Models.ElectionCandidate> candidates)
        {
            // get candidate/running mate information
            return new CandidatePersonalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidates[0].ElectionCandidateId,
                RunningMateId = candidates[1].ElectionCandidateId,
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



        public CandidateCompareDisplayViewModel GetCandidateCompareDisplayViewModel(CandidateCompareViewModel viewModel)
        {
            // load variables passed in
            CandidateCompareDisplayViewModel displayViewModel = new CandidateCompareDisplayViewModel()
            {
                ControllerName = viewModel.ControllerName,
                VotingDateId = viewModel.VotingDateId,
                VotingDate = viewModel.VotingDate,
                OfficeId = viewModel.OfficeId,
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId,
                CandidateSecondCompareCount = viewModel.CandidateSecondCompareCount
            };

            // load view model with objects
            displayViewModel.CandidateCompareSummaryViewModel = GetCandidateCompareSummaryViewModel(displayViewModel);
            // OfficeSummaryViewModel
            displayViewModel.CandidateComparePoliticalViewModel = GetCandidateComparePoliticalViewModel(displayViewModel.CandidateCompareSummaryViewModel);
            displayViewModel.CandidateCompareCaucusViewModel = GetCandidateCompareCaucusViewModel(displayViewModel.CandidateCompareSummaryViewModel);
            displayViewModel.CandidateCompareProfessionalViewModel = GetCandidateCompareProfessionalViewModel(displayViewModel.CandidateCompareSummaryViewModel);
            displayViewModel.CandidateCompareEducationViewModel = GetCandidateCompareEducationViewModel(displayViewModel.CandidateCompareSummaryViewModel);
            displayViewModel.CandidateComparePersonalViewModel = GetCandidateComparePersonalViewModel(displayViewModel.CandidateCompareSummaryViewModel);
            // ContactViewModel
            displayViewModel.CandidateCompareCivicViewModel = GetCandidateCompareCivicViewModel(displayViewModel.CandidateCompareSummaryViewModel);
            displayViewModel.CandidateCompareAdditionalViewModel = GetCandidateCompareAdditionalViewModel(displayViewModel.CandidateCompareSummaryViewModel);
            
            return displayViewModel;
        }



        // *************************************************************************************



        public CandidateCompareSummaryViewModel GetCandidateCompareSummaryViewModel(CandidateCompareDisplayViewModel viewModel)
        {
            // load variables passed in
            CandidateCompareSummaryViewModel summaryViewModel = new CandidateCompareSummaryViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId,
            };

            summaryViewModel.CandidateCompareSummaryFirstViewModel = GetCandidateCompareSummaryFirstViewModel(viewModel);
            summaryViewModel.CandidateCompareSummarySecondViewModel = GetCandidateCompareSummarySecondViewModel(viewModel);
            summaryViewModel.CandidateCompareSummaryLookUpViewModel = GetCandidateCompareLookUpSecondViewModel(viewModel.CandidateFirstDisplayId, viewModel.VotingDateId, viewModel.OfficeId);

            return summaryViewModel;
        }



        public CandidateCompareSummaryFirstViewModel GetCandidateCompareSummaryFirstViewModel(CandidateCompareDisplayViewModel viewModel)
        {
            // get candidate display information
            List<int> candidateDisplayIdList = GetCandidateDisplayIdList(viewModel);
            int candidateDisplayVotingDateId = viewModel.VotingDateId;
            int candidateDisplayOfficeId = viewModel.OfficeId;

            // get candidate/running mate
            List<Models.ElectionVotingDateOfficeCandidate> dbCandidates = GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(viewModel.CandidateFirstDisplayId);

            // Get candidate/running mate objects for view model
            List<Models.ElectionCandidate> candidates = GetCandidateNameAndRunningMateNameFromDatabase(dbCandidates);
            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = GetCandidateAndRunningMateInformationFromVoteSmart(candidates);
            List<Models.ElectionOffice> candidateOffices = GetCandidateAndRunningMateOfficeInformation(dbCandidates);
            Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidates[0].PartyId);
            Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidates[0].OfficeHolderId);

            return GetCandidateCompareSummaryFirst(candidateDisplayIdList, viewModel, candidates, voteSmartCandidates, candidateOffices, party, officeHolder);
        }



        public CandidateCompareSummarySecondViewModel GetCandidateCompareSummarySecondViewModel(CandidateCompareDisplayViewModel viewModel)
        {
            // get candidate display information
            List<int> candidateDisplayIdList = GetCandidateDisplayIdList(viewModel);
            int candidateDisplayVotingDateId = viewModel.VotingDateId;
            int candidateDisplayOfficeId = viewModel.OfficeId;

            // get candidate/running mate
            List<Models.ElectionVotingDateOfficeCandidate> dbCandidates = GetCandidateAndRunningMateForCurrentElectionDateFromDatabase(viewModel.CandidateSecondDisplayId);

            // Get candidate/running mate objects for view model
            List<Models.ElectionCandidate> candidates = GetCandidateNameAndRunningMateNameFromDatabase(dbCandidates);
            List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates = GetCandidateAndRunningMateInformationFromVoteSmart(candidates);
            List<Models.ElectionOffice> candidateOffices = GetCandidateAndRunningMateOfficeInformation(dbCandidates);
            Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidates[0].PartyId);
            Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidates[0].OfficeHolderId);

            return GetCandidateCompareSummarySecond(candidateDisplayIdList, viewModel, candidates, voteSmartCandidates, candidateOffices, party, officeHolder);
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
                                                                                        List<Models.ElectionCandidate> candidates, List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates,
                                                                                        List<Models.ElectionOffice> candidateOffices, Models.ElectionParty party,
                                                                                        Models.ElectionOfficeHolder officeHolder)
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
                    CandidateId = candidates[0].ElectionCandidateId,
                    FirstName = candidates[0].FirstName,
                    MiddleName = candidates[0].MiddleName,
                    LastName = candidates[0].LastName,
                    Suffix = candidates[0].Suffix,
                    OfficeName = candidateOffices[0].OfficeName,
                    OfficeTerm = candidateOffices[0].Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[0].Photo, voteSmartCandidates[0].Gender)
                },
                RunningMateCompareSummaryFirst = new RunningMateCompareSummaryFirst()
                {
                    CandidateId = candidates[1].ElectionCandidateId,
                    FirstName = candidates[1].FirstName,
                    MiddleName = candidates[1].MiddleName,
                    LastName = candidates[1].LastName,
                    Suffix = candidates[1].Suffix,
                    OfficeName = candidateOffices[1].OfficeName,
                    OfficeTerm = candidateOffices[1].Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[1].Photo, voteSmartCandidates[1].Gender)
                }
            };
        }



        public CandidateCompareSummarySecondViewModel GetCandidateCompareSummarySecond(List<int> candidateDisplayIdList, CandidateCompareDisplayViewModel viewModel,
                                                                                        List<Models.ElectionCandidate> candidates, List<ViewModels.VoteSmart.CandidateBio> voteSmartCandidates,
                                                                                        List<Models.ElectionOffice> candidateOffices, Models.ElectionParty party,
                                                                                        Models.ElectionOfficeHolder officeHolder)
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
                    CandidateId = candidates[0].ElectionCandidateId,
                    FirstName = candidates[0].FirstName,
                    MiddleName = candidates[0].MiddleName,
                    LastName = candidates[0].LastName,
                    Suffix = candidates[0].Suffix,
                    OfficeName = candidateOffices[0].OfficeName,
                    OfficeTerm = candidateOffices[0].Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[0].Photo, voteSmartCandidates[0].Gender),
                },
                RunningMateCompareSummarySecond = new RunningMateCompareSummarySecond()
                {
                    CandidateId = candidates[1].ElectionCandidateId,
                    FirstName = candidates[1].FirstName,
                    MiddleName = candidates[1].MiddleName,
                    LastName = candidates[1].LastName,
                    Suffix = candidates[1].Suffix,
                    OfficeName = candidateOffices[1].OfficeName,
                    OfficeTerm = candidateOffices[1].Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidates[1].Photo, voteSmartCandidates[1].Gender)
                }
            };
        }



        // ********************************************************************************************



        public CandidateComparePoliticalViewModel GetCandidateComparePoliticalViewModel(CandidateCompareSummaryViewModel viewModel)
        {
            // load variables passed in
            CandidateComparePoliticalViewModel politicalViewModel = new CandidateComparePoliticalViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId
            };

            politicalViewModel.CandidateComparePoliticalFirstViewModel = GetCandidateComparePoliticalFirstViewModel(viewModel.CandidateCompareSummaryFirstViewModel);
            politicalViewModel.CandidateComparePoliticalSecondViewModel = GetCandidateComparePoliticalSecondViewModel(viewModel.CandidateCompareSummarySecondViewModel);

            return politicalViewModel;
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



        // ********************************************************************************************



        public CandidateCompareCaucusViewModel GetCandidateCompareCaucusViewModel(CandidateCompareSummaryViewModel viewModel)
        {
            // load variables passed in
            CandidateCompareCaucusViewModel caucusViewModel = new CandidateCompareCaucusViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId
            };

            caucusViewModel.CandidateCompareCaucusFirstViewModel = GetCandidateCompareCaucusFirstViewModel(viewModel.CandidateCompareSummaryFirstViewModel);
            caucusViewModel.CandidateCompareCaucusSecondViewModel = GetCandidateCompareCaucusSecondViewModel(viewModel.CandidateCompareSummarySecondViewModel);

            return caucusViewModel;
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



        // ********************************************************************************************



        public CandidateCompareProfessionalViewModel GetCandidateCompareProfessionalViewModel(CandidateCompareSummaryViewModel viewModel)
        {
            // load variables passed in
            CandidateCompareProfessionalViewModel professionalViewModel = new CandidateCompareProfessionalViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId
            };

            professionalViewModel.CandidateCompareProfessionalFirstViewModel = GetCandidateCompareProfessionalFirstViewModel(viewModel.CandidateCompareSummaryFirstViewModel);
            professionalViewModel.CandidateCompareProfessionalSecondViewModel = GetCandidateCompareProfessionalSecondViewModel(viewModel.CandidateCompareSummarySecondViewModel);

            return professionalViewModel;
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



        // ********************************************************************************************



        public CandidateCompareEducationViewModel GetCandidateCompareEducationViewModel(CandidateCompareSummaryViewModel viewModel)
        {
            // load variables passed in
            CandidateCompareEducationViewModel educationalViewModel = new CandidateCompareEducationViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId
            };

            educationalViewModel.CandidateCompareEducationFirstViewModel= GetCandidateCompareEducationFirstViewModel(viewModel.CandidateCompareSummaryFirstViewModel);
            educationalViewModel.CandidateCompareEducationSecondViewModel = GetCandidateCompareEducationSecondViewModel(viewModel.CandidateCompareSummarySecondViewModel);

            return educationalViewModel;
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



        // ********************************************************************************************



        public CandidateComparePersonalViewModel GetCandidateComparePersonalViewModel(CandidateCompareSummaryViewModel viewModel)
        {
            // load variables passed in
            CandidateComparePersonalViewModel personalViewModel = new CandidateComparePersonalViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId
            };

            personalViewModel.CandidateComparePersonalFirstViewModel = GetCandidateComparePersonalFirstViewModel(viewModel.CandidateCompareSummaryFirstViewModel);
            personalViewModel.CandidateComparePersonalSecondViewModel = GetCandidateComparePersonalSecondViewModel(viewModel.CandidateCompareSummarySecondViewModel);

            return personalViewModel;
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



        // ********************************************************************************************



        public CandidateCompareCivicViewModel GetCandidateCompareCivicViewModel(CandidateCompareSummaryViewModel viewModel)
        {
            // load variables passed in
            CandidateCompareCivicViewModel civicViewModel = new CandidateCompareCivicViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId
            };

            civicViewModel.CandidateCompareCivicFirstViewModel = GetCandidateCompareCivicFirstViewModel(viewModel.CandidateCompareSummaryFirstViewModel);
            civicViewModel.CandidateCompareCivicSecondViewModel = GetCandidateCompareCivicSecondViewModel(viewModel.CandidateCompareSummarySecondViewModel);

            return civicViewModel;
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



        // ********************************************************************************************



        public CandidateCompareAdditionalViewModel GetCandidateCompareAdditionalViewModel(CandidateCompareSummaryViewModel viewModel)
        {
            // load variables passed in
            CandidateCompareAdditionalViewModel additionalViewModel = new CandidateCompareAdditionalViewModel()
            {
                CandidateFirstDisplayId = viewModel.CandidateFirstDisplayId,
                CandidateSecondDisplayId = viewModel.CandidateSecondDisplayId
            };

            additionalViewModel.CandidateCompareAdditionalFirstViewModel= GetCandidateCompareAdditionalFirstViewModel(viewModel.CandidateCompareSummaryFirstViewModel);
            additionalViewModel.CandidateCompareAdditionalSecondViewModel = GetCandidateCompareAdditionalSecondViewModel(viewModel.CandidateCompareSummarySecondViewModel);

            return additionalViewModel;
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



        // ********************************************************************************************



        public ViewModels.VoteSmart.CandidateBio GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(string voteSmartCandidateId)
        {
            VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();
            return voteSmart.GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(voteSmartCandidateId);
        }



        // *********************************************************************



        public string GetValidImageLocationToDisplay(string voteSmartURL, string gender)
        {
            // was a URL found at VoteSmart?
            if (voteSmartURL != null && voteSmartURL != "")
            {
                return voteSmartURL;
            }

            // if not display blank image base on the candidate's gender
            if (gender == "Female")
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



        public Models.ElectionVotingDateOfficeCandidate GetCandidateSummaryForCurrentElectionDateFromDatabase(int candidateLookUpId)
        {
            Models.ElectionVotingDateOfficeCandidate dbCandidate = new Models.ElectionVotingDateOfficeCandidate();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidate = db.ElectionVotingDateOfficeCandidates.FirstOrDefault(x => x.CandidateId == candidateLookUpId);
            }

            if (dbCandidate == null)
                return new Models.ElectionVotingDateOfficeCandidate();

            return dbCandidate;
        }



        public Models.ElectionVotingDateOfficeCandidate GetRunningMateSummaryForCurrentElectionDateFromDatabase(int candidateLookUpId)
        {
            Models.ElectionVotingDateOfficeCandidate dbRunningMate = new Models.ElectionVotingDateOfficeCandidate();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbRunningMate = db.ElectionVotingDateOfficeCandidates.FirstOrDefault(x => x.RunningMateId == candidateLookUpId);
            }

            if (dbRunningMate == null)
                return new Models.ElectionVotingDateOfficeCandidate();

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



        public Models.ElectionCandidate GetCandidateNameForCandidateIdFromDatabase(int candidateId)
        {
            Models.ElectionCandidate dbCandidate = new Models.ElectionCandidate();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidate = db.ElectionCandidates.Find(candidateId);
            }

            if (dbCandidate == null )
                return new Models.ElectionCandidate();

            return dbCandidate;
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



        /// <summary>
        /// get a list of active election dates from database
        /// </summary>
        /// <returns></returns>
        public List<Models.ElectionVotingDate> GetActiveElectionDatesInOrderByDateFromDatabase()
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                return db.ElectionVotingDates.Where(x => x.Active == true).OrderBy(x => x.Date).ToList();
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
            }

            if (dbOffice == null)
                return new Models.ElectionOffice();

            return dbOffice;
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



        private CandidateCompareDropDownList GetCandidatesToCompareForDropDownList(int candidateFirstDisplayId, int dateId, int officeId)
        {
            return new CandidateCompareDropDownList()
            {
                CandidateNames = GetCandidateCompareListItems(candidateFirstDisplayId, dateId, officeId)
            };
        }



        // *****************************************************************



        private IEnumerable<SelectListItem> GetCandidateListItems(int dateId)
        {
            if (dateId <= 0)
            {
                return new List<SelectListItem>();
            }

            List<int> dbCandidateIds = GetCandidatesForCurrentDateFromDatabase(dateId);
            List<Models.ElectionCandidate> dboCandidates = GetCandidateNamesForCandidateId(dbCandidateIds);
            List<SelectListItem> candidates = new List<SelectListItem>();

            for (int i = 0; i < dboCandidates.Count; i++)
            {
                string candidateName = string.Format("{0} {1}", dboCandidates[i].FirstName, dboCandidates[i].LastName);

                candidates.Add(new SelectListItem()
                {
                    Value = dboCandidates[i].ElectionCandidateId.ToString(),
                    Text = candidateName
                });
            }

            return candidates;
        }



        private IEnumerable<SelectListItem> GetCandidateCompareListItems(int candidateFirstDisplayId, int dateId, int officeId)
        {
            if (dateId <= 0)
                return new List<SelectListItem>();

            List<int> dbCandidateIds = GetCandidatesForCurrentDateAndOfficeFromDatabase(dateId, officeId);
            dbCandidateIds = RemoveSuppliedCandidateFromCandidateIdList(dbCandidateIds, candidateFirstDisplayId);
            List<Models.ElectionCandidate> dboCandidates = GetCandidateNamesForCandidateId(dbCandidateIds);

            List<SelectListItem> candidates = new List<SelectListItem>();

            for (int i = 0; i < dboCandidates.Count; i++)
            {
                string candidateName = string.Format("{0} {1}", dboCandidates[i].FirstName, dboCandidates[i].LastName);

                candidates.Add(new SelectListItem()
                {
                    Value = dboCandidates[i].ElectionCandidateId.ToString(),
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



        private List<int> GetCandidatesForCurrentDateFromDatabase(int dateId)
        {
            List<Models.ElectionVotingDateOfficeCandidate> dbOffices;

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbOffices = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == dateId).ToList();
            }

            if (dbOffices == null)
            {
                return new List<int>();
            }

            return dbOffices.Select(x => x.CandidateId).Distinct().ToList();
        }



        private List<int> GetCandidatesForCurrentDateAndOfficeFromDatabase(int dateId, int officeId)
        {
            List<Models.ElectionVotingDateOfficeCandidate> dbCandidates;

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidates = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == dateId)
                                                                    .Where(x => x.OfficeId == officeId)
                                                                    .ToList();
            }

            if (dbCandidates == null)
            {
                return new List<int>();
            }

            return dbCandidates.Select(x => x.CandidateId).Distinct().ToList();
        }



        // **********************************************************************



        private List<Models.ElectionCandidate> GetCandidateNamesForCandidateId(List<int> candidateIds)
        {
            List<Models.ElectionCandidate> dbCandidates = new List<ElectionCandidate>();
            List<Models.ElectionCandidate> candidates = new List<ElectionCandidate>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidates = db.ElectionCandidates.ToList();
            }

            for (int i = 0; i < candidateIds.Count; i++)
            {
                for (int j = 0; j < dbCandidates.Count; j++)
                {
                    if (candidateIds[i] == dbCandidates[j].ElectionCandidateId)
                    {
                        candidates.Add(new Models.ElectionCandidate()
                        {
                            ElectionCandidateId = dbCandidates[j].ElectionCandidateId,
                            FirstName = dbCandidates[j].FirstName,
                            LastName = dbCandidates[j].LastName
                        });

                        j = dbCandidates.Count;
                    }
                }
            }

            return candidates.OrderBy(x => x.MiddleName).OrderBy(x => x.FirstName).OrderBy(x => x.LastName).ToList();
        }



        public Models.ElectionParty GetPartyNameForPartyIdFromDatabase(string partyId)
        {
            Models.ElectionParty dbParty = new Models.ElectionParty();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbParty = db.ElectionParties.Find(partyId);
            }

            if (dbParty == null)
                return new Models.ElectionParty();

            return dbParty;
        }



        public Models.ElectionOfficeHolder GetOfficeHolderyNameForOfficeHolderIdFromDatabase(string officeHolderId)
        {
            Models.ElectionOfficeHolder dbOfficeHolder = new Models.ElectionOfficeHolder();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbOfficeHolder = db.OfficeHolders.Find(officeHolderId);
            }

            if (dbOfficeHolder == null)
                return new Models.ElectionOfficeHolder();

            return dbOfficeHolder;
        }




    }
}