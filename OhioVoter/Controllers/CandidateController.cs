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
            // validate data supplied
            if (candidateId == null || candidateId < 0)
            {
                candidateId = 0;
            }

            // update session with controller info
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);

            // get details for view model
            CandidateViewModel viewModel = new CandidateViewModel()
            {
                ControllerName = _controllerName,
                CandidateId = (int)candidateId
            };

            return View(viewModel);
        }



        public ActionResult Compare(int candidateFirstId, int? candidateSecondId, int dateId, int officeId)
        {
            // validate data supplied
            if (candidateFirstId <= 0 || dateId <= 0 || officeId <= 0)
            {
                return RedirectToAction("Index");
            }

            // make sure candidateSecondId is valid
            if (candidateSecondId == null || candidateSecondId <= 0)
            {// NO
                candidateSecondId = 0;
            }

            // update session with controller info
            UpdateSessionWithNewControllerNameForSideBar(_controllerName);
            Models.ElectionVotingDate date = GetDateInformationForDateIdFromDatabase(dateId);

            // get details for view model
            CandidateCompareViewModel viewModel = new CandidateCompareViewModel()
            {
                ControllerName = _controllerName,
                CandidateFirstDisplayId = candidateFirstId,
                CandidateSecondDisplayId = (int)candidateSecondId,
                VotingDateId = dateId,
                VotingDate = date.Date.ToShortDateString(),
                OfficeId = officeId
            };

            // get information for the second candidate(s)
            CandidateCompareSummaryLookUpViewModel candidateCompareLookUpSecondVM = GetCandidateCompareLookUpSecondViewModel(viewModel.CandidateFirstDisplayId, viewModel.VotingDateId, viewModel.OfficeId);
            List<SelectListItem> candidates = candidateCompareLookUpSecondVM.CandidateNames.ToList();

            // if only one candidate then display candidate information
            if (candidates.Count == 1)
            {
                int intId;
                string strId = candidates[0].Value.ToString();
                if (int.TryParse(strId, out intId))
                {
                    viewModel.CandidateSecondDisplayId = int.Parse(strId);
                }
            }

            // get information for the candidateCompareDisplayViewModel
            if (candidates.Count >= 1)
            {
                // load all data for compare view model
                viewModel.CandidateCompareDisplayViewModel = new CandidateCompareDisplayViewModel()
                {
                    CandidateCompareSummaryViewModel = GetCandidateCompareSummaryViewModel(viewModel, candidates.Count)
                };

                return View(viewModel);
            }

            // not candidates found to compare
            return RedirectToAction("Index");
        }




        public ActionResult RemoveFirst(int candidateFirstId, int candidateSecondId, int dateId, int officeId)
        {
            // move second to first
            candidateFirstId = candidateSecondId;

            // clear second Id
            candidateSecondId =0;

            return RedirectToAction("Compare", new { candidateFirstId = candidateFirstId, candidateSecondId = candidateSecondId, dateId = dateId, officeId = officeId });
        }



        public ActionResult RemoveSecond(int candidateFirstId, int candidateSecondId, int CandidateCount, int dateId, int officeId)
        {
            // displayed candidate removed from list
            if (CandidateCount > 1)
            {
                // clear candidateSecondId to display dropdownlist
                return RedirectToAction("Compare", new { candidateFirstId = candidateFirstId, candidateSecondId = 0, dateId = dateId, officeId = officeId });
            }
            else
            {
                return RedirectToAction("Index", "Candidate", new { candidateId = candidateFirstId });
            }
        }



        [ChildActionOnly]
        public ActionResult DisplayCandidateInformation(CandidateViewModel model)
        {
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
        public ActionResult DisplayCandidateCompareInformation(CandidateCompareSummaryViewModel viewModel)
        {

            // display information for one candidate or list of candidates
            if (viewModel.CandidateCompareSummarySecondViewModel.CandidateSecondDisplayId > 0)
            {
                return PartialView("_CandidateCompareSummarySecond", viewModel.CandidateCompareSummarySecondViewModel);
            }
            else
            {
                return PartialView("_CandidateCompareSummaryLookUp", viewModel.CandidateCompareSummaryLookUpViewModel);
            }
        }



/*        public ActionResult ChangeCandidateFirst(int candidateFirstId)
        {
            // if candidateSecond is displayed then provide a dropdown list
            // if candidateFirst is only candidate displayed than return to candidateLookUp displaying only one candidate
            // redirect to partial view

            return PartialView();
        }
*/


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



        // **************************************************************
        /*
        private CandidateSummaryViewModel GetCandidateSummaryViewModel(int candidateId)
        {
            if (candidateId != 0)
            {
                ElectionVotingDate date = GetFirstActiveElectionDate();
                CandidateSummaryViewModel candidate = GetCandidateSummaryInformationForSelectedCandidateId(candidateId, date);
                candidate = GetCandidateBiographyFromVoteSmart(candidate);
                return candidate;
            }
            else
            {
                return new CandidateSummaryViewModel();
            }
        }
        */


        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }


        /*
        private CandidateDropDownList GetCandidateDropDownListViewModel(int dateId)
        {
            return GetCandidatesForDropDownList(dateId);
        }
        */


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
                return RedirectToAction("Compare", new { candidateFirstId = model.CandidateFirstDisplayId, candidateSecondId = 0, dateId = model.VotingDateId, officeId = model.OfficeId });
            }

            // validate selectedCandidateId
            int candidateId;
            if(model.SelectedCandidateId == null)
            {
                return RedirectToAction("Compare", new { candidateFirstId = model.CandidateFirstDisplayId, candidateSecondId = 0, dateId = model.VotingDateId, officeId = model.OfficeId });
            }
            else if (int.TryParse(model.SelectedCandidateId, out candidateId))
            {
                candidateId = int.Parse(model.SelectedCandidateId);
                return RedirectToAction("Compare", new { candidateFirstId = model.CandidateFirstDisplayId, candidateSecondId = candidateId, dateId = model.VotingDateId, officeId = model.OfficeId });
            }
            else
            {
                return RedirectToAction("Index", new { candidateId = 0 });
            }
        }




        /*
        /// <summary>
        /// Get all the candidate information for active elections
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CandidateSummaryViewModel> GetSummaryInformationForAllCandidatesForUpcomingElections()
        {
            // From Database: get list of all active election dates (ElectionVotingDate)
            // (ElectionDateId,Date)
            List<Models.ElectionVotingDate> electionDatesDb = GetActiveElectionDatesFromDatabase();

            // get election information for all dates
            List<CandidateSummaryViewModel> allElectionCandidates = GetCandidateSummaryInformationForAllDates(electionDatesDb);

            // get office names for officeId
            List<Models.ElectionOffice> officeList = GetCompleteListOfPossibleOfficeNamesFromDatabase();
            allElectionCandidates = GetOfficeNamesForAllDates(allElectionCandidates, officeList);

            // get candidate/runningmate names for candidateId
            List<Models.ElectionCandidate> candidateList = GetCompleteListOfPossibleCandidateNamesFromDatabase();
            allElectionCandidates = GetCandidateNamesForAllDates(allElectionCandidates, candidateList);
            //candidates = GetRunningMateNamesForAllDates(candidates, candidateList);


            return allElectionCandidates;
        }
        */

        /*
        public List<CandidateSummaryViewModel> GetCandidateSummaryInformationForAllDates(List<Models.ElectionVotingDate> electionDates)
        {
            if (electionDates == null)
                return new List<CandidateSummaryViewModel>();

            List<CandidateSummaryViewModel> candidates = new List<CandidateSummaryViewModel>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                for (int i = 0; i < electionDates.Count; i++)
                {
                    int dateId = electionDates[i].ElectionVotingDateId;
                    List<Models.ElectionVotingDateOfficeCandidate> dbAllCandidates = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == dateId).ToList();

                    for (int j = 0; j < dbAllCandidates.Count; j++)
                    {
                        candidates.Add(new CandidateSummaryViewModel()
                        {
                            ElectionVotingDateId = dateId,
                            VotingDate = electionDates[i].Date,
                            CandidateId = dbAllCandidates[j].CandidateId,
                            CandidateOfficeId = dbAllCandidates[j].OfficeId,
                            CertifiedCandidateId = dbAllCandidates[j].CertifiedCandidateId,
                            PartyId = dbAllCandidates[j].PartyId,
                            OfficeHolderId = dbAllCandidates[j].OfficeHolderId,
                            RunningMateId = dbAllCandidates[j].RunningMateId
                        });
                    }
                }
            }
            
            return candidates;
        }
        */


        // ******************************************************************************




        public CandidateDisplayViewModel GetCandidateDisplayViewModel(CandidateDisplayViewModel candidateDisplayVM)
        {
            int candidateLookUpVotingDateId = 0;
            int candidateLookUpOfficeId = 0;

            Models.ElectionVotingDateOfficeCandidate dbCandidate;
            Models.ElectionVotingDateOfficeCandidate dbRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateDisplayVM.CandidateLookUpId);

            if (dbRunningMate == null || dbRunningMate.CandidateId == 0)
            {// no running mate found. Must be candidate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateDisplayVM.CandidateLookUpId);
                if (dbCandidate.RunningMateId != 0)
                {
                    dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
                }
                candidateLookUpVotingDateId = dbCandidate.ElectionVotingDateId;
                candidateLookUpOfficeId = dbCandidate.OfficeId;
            }
            else
            {// running mate == candidate lookup. store the candidate for the running mate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMate.CandidateId);
                dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
                candidateLookUpVotingDateId = dbRunningMate.ElectionVotingDateId;
                candidateLookUpOfficeId = dbRunningMate.OfficeId;
            }

            // Get all objects for view model
            Models.ElectionCandidate candidate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.CandidateId);
            Models.ElectionCandidate runningMate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.RunningMateId);
            ViewModels.VoteSmart.CandidateBio voteSmartCandidate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidate.VoteSmartCandidateId);
            ViewModels.VoteSmart.CandidateBio voteSmartRunningMate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMate.VoteSmartCandidateId);
            Models.ElectionOffice candidateOffice = GetOfficeInformationForOfficeId(dbCandidate.OfficeId);
            Models.ElectionOffice runningMateOffice = GetOfficeInformationForOfficeId(dbRunningMate.OfficeId);
            Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidate.PartyId);
            Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidate.OfficeHolderId);

            // load view model with objects
            candidateDisplayVM.CandidateSummaryViewModel = GetCandidateSummaryViewModel(candidateDisplayVM.CandidateLookUpId, candidateLookUpVotingDateId, candidateLookUpOfficeId, candidate, runningMate, voteSmartCandidate, voteSmartRunningMate, candidateOffice, runningMateOffice, party, officeHolder);
            // OfficeSummaryViewModel
            candidateDisplayVM.CandidatePoliticalViewModel = GetCandidatePoliticalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.CandidateCaucusViewModel = GetCandidateCaucusInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.CandidateProfessionalViewModel = GetCandidateProfessionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.CandidateEducationViewModel = GetCandidateEducationInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.CandidatePersonalViewModel = GetCandidatePersonalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            // ContactViewModel
            candidateDisplayVM.CandidateCivicViewModel = GetCandidateCivicInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.CandidateAdditionalViewModel = GetCandidateAdditionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);

            return candidateDisplayVM;
        }



        // ***********************************************************************



        public CandidateSummaryViewModel GetCandidateSummaryViewModel(int candidateLookUpId,
                                                                      int candidateLookUpVotingDateId,
                                                                      int candidateLookUpOfficeId,
                                                                      Models.ElectionCandidate candidate,
                                                                      Models.ElectionCandidate runningMate,
                                                                      ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                      ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                      Models.ElectionOffice candidateOffice,
                                                                      Models.ElectionOffice runningMateOffice,
                                                                      Models.ElectionParty party,
                                                                      Models.ElectionOfficeHolder officeHolder)
        {
            return new CandidateSummaryViewModel()
            {
                SelectedCandidateId = candidateLookUpId,
                SelectedCandidateVotingDateId = candidateLookUpVotingDateId,
                SelectedCandidateOfficeId = candidateLookUpOfficeId,
                CandidateSummary = new CandidateSummary()
                {
                    CandidateId = candidate.ElectionCandidateId,
                    FirstName = candidate.FirstName,
                    MiddleName = candidate.MiddleName,
                    LastName = candidate.LastName,
                    Suffix = candidate.Suffix,
                    OfficeName = candidateOffice.OfficeName,
                    OfficeTerm = candidateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidate.Photo, voteSmartCandidate.Gender)
                },
                RunningMateSummary = new RunningMateSummary()
                {
                    CandidateId = runningMate.ElectionCandidateId,
                    RunningMateId = runningMate.ElectionCandidateId,
                    FirstName = runningMate.FirstName,
                    MiddleName = runningMate.MiddleName,
                    LastName = runningMate.LastName,
                    Suffix = runningMate.Suffix,
                    OfficeName = runningMateOffice.OfficeName,
                    OfficeTerm = runningMateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartRunningMate.Photo, voteSmartCandidate.Gender)
                }
            };
        }



        public CandidatePoliticalViewModel GetCandidatePoliticalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                        ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                        int candidateLookUpId,
                                                                                                        int candidateId,
                                                                                                        int runningMateId)
        {
            return new CandidatePoliticalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Political),
                RunningMatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Political)
            };
        }



        public CandidateCaucusViewModel GetCandidateCaucusInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                    ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                    int candidateLookUpId,
                                                                                                    int candidateId,
                                                                                                    int runningMateId)
        {
            return new CandidateCaucusViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.CongMembership),
                RunningMateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.CongMembership)
            };
        }



        public CandidateProfessionalViewModel GetCandidateProfessionalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                              ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                              int candidateLookUpId,
                                                                                                              int candidateId,
                                                                                                              int runningMateId)
        {
            return new CandidateProfessionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Profession),
                RunningMateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Profession)
            };
        }



        public CandidateEducationViewModel GetCandidateEducationInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                        ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                        int candidateLookUpId,
                                                                                                        int candidateId,
                                                                                                        int runningMateId)
        {
            return new CandidateEducationViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateEducationHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Education),
                RunningMateEducationHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Education)
            };
        }



        public CandidateCivicViewModel GetCandidateCivicInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                int candidateLookUpId,
                                                                                                int candidateId,
                                                                                                int runningMateId)
        {
            return new CandidateCivicViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartCandidate.OrgMembership),
                RunningMateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartRunningMate.OrgMembership)
            };
        }



        public CandidateAdditionalViewModel GetCandidateAdditionalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                            ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                            int candidateLookUpId,
                                                                                                            int candidateId,
                                                                                                            int runningMateId)
        {
            return new CandidateAdditionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartCandidate.SpecialMsg),
                RunningMateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartRunningMate.SpecialMsg)
            };
        }



        public CandidatePersonalViewModel GetCandidatePersonalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                        ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                        int candidateLookUpId,
                                                                                                        int candidateId,
                                                                                                        int runningMateId)
        {
            return new CandidatePersonalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateFamily = voteSmartCandidate.Family,
                RunningMateFamily = voteSmartRunningMate.Family,
                CandidateGender = voteSmartCandidate.Gender,
                RunningMateGender = voteSmartRunningMate.Gender,
                CandidateBirthDate = voteSmartCandidate.BirthDate,
                RunningMateBirthDate = voteSmartRunningMate.BirthDate,
                CandidateBirthPlace = voteSmartCandidate.BirthPlace,
                RunningMateBirthPlace = voteSmartRunningMate.BirthPlace,
                CandidateHomeCity = voteSmartCandidate.HomeCity,
                RunningMateHomeCity = voteSmartRunningMate.HomeCity,
                CandidateHomeState = voteSmartCandidate.HomeState,
                RunningMateHomeState = voteSmartRunningMate.HomeState,
                CandidateReligion = voteSmartCandidate.Religion,
                RunningMateReligion = voteSmartRunningMate.Religion
            };
        }




        // *****************************************************************


        /*
                public CandidateCompareSummaryFirstViewModel GetCandidateFirstSummaryForCandidateCompareLookUpViewModel(int candidateDisplayId, int candiateId, int runningMateId, int votingDateId, int officeId)
                {
                    Models.ElectionVotingDateOfficeCandidate dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(candiateId);
                    Models.ElectionVotingDateOfficeCandidate dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(runningMateId);

                    // Get all objects for view model
                    Models.ElectionCandidate candidate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.CandidateId);
                    Models.ElectionCandidate runningMate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.RunningMateId);
                    ViewModels.VoteSmart.CandidateBio voteSmartCandidate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidate.VoteSmartCandidateId);
                    ViewModels.VoteSmart.CandidateBio voteSmartRunningMate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMate.VoteSmartCandidateId);
                    Models.ElectionOffice candidateOffice = GetOfficeInformationForOfficeId(dbCandidate.OfficeId);
                    Models.ElectionOffice runningMateOffice = GetOfficeInformationForOfficeId(dbRunningMate.OfficeId);
                    Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidate.PartyId);
                    Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidate.OfficeHolderId);

                    // load view model with objects
                    return GetCandidateSummaryFirstViewModel(candidateDisplayId, votingDateId, officeId, candidate, runningMate, voteSmartCandidate, voteSmartRunningMate, candidateOffice, runningMateOffice, party, officeHolder);
                }
        */


        // ****************************************************************

        /*
                public CandidateCompareLookUpSecondViewModel GetCandidateCompareLookUpViewModel(CandidateCompareLookUpSecondViewModel candidateCompareLookUpVM)
                {
                    Models.ElectionVotingDateOfficeCandidate dbCandidateFirst;
                    Models.ElectionVotingDateOfficeCandidate dbRunningMateFirst = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateCompareLookUpVM.CandidateFirstDisplayId);

                    // determine if candidate displayed is a possible runningmate and load information
                    if (dbRunningMateFirst == null || dbRunningMateFirst.CandidateId == 0)
                    {// no running mate found. Must be candidate
                        dbCandidateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateCompareLookUpVM.CandidateFirstDisplayId);
                        if (dbCandidateFirst.RunningMateId != 0)
                        {
                            dbRunningMateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateFirst.RunningMateId);
                        }
                    }
                    else
                    {// running mate == candidate displayed. store the actual candidate for the running mate
                        dbCandidateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMateFirst.CandidateId);
                        dbRunningMateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateFirst.RunningMateId);
                    }

                    // Get all objects for view model
                    Models.ElectionCandidate candidateFirst = GetCandidateNameForCandidateIdFromDatabase(dbCandidateFirst.CandidateId);
                    Models.ElectionCandidate runningMateFirst = GetCandidateNameForCandidateIdFromDatabase(dbCandidateFirst.RunningMateId);
                    ViewModels.VoteSmart.CandidateBio voteSmartCandidateFirst = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidateFirst.VoteSmartCandidateId);
                    ViewModels.VoteSmart.CandidateBio voteSmartRunningMateFirst = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMateFirst.VoteSmartCandidateId);
                    Models.ElectionOffice candidateFirstOffice = GetOfficeInformationForOfficeId(dbCandidateFirst.OfficeId);
                    Models.ElectionOffice runningMateFirstOffice = GetOfficeInformationForOfficeId(dbCandidateFirst.OfficeId);
                    Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidateFirst.PartyId);
                    Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidateFirst.OfficeHolderId);

                    // load view model with objects
                    return new CandidateCompareLookUpSecondViewModel()
                    {
                        //CandidateCompareSummaryFirstViewModel = GetCandidateSummaryFirstViewModel(candidateCompareLookUpVM.CandidateFirstDisplayId, candidateCompareLookUpVM.VotingDateId, candidateCompareLookUpVM.OfficeId, candidateFirst, runningMateFirst, voteSmartCandidateFirst, voteSmartRunningMateFirst, candidateFirstOffice, runningMateFirstOffice, party, officeHolder),
                        // OfficeSummaryViewModel
                        //CandidateFirstPoliticalViewModel = GetCandidateFirstPoliticalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstCaucusViewModel = GetCandidateFirstCaucusInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstProfessionalViewModel = GetCandidateFirstProfessionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstEducationViewModel = GetCandidateFirstEducationInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstPersonalViewModel = GetCandidateFirstPersonalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        // ContactViewModel
                        //CandidateFirstCivicViewModel = GetCandidateFirstCivicInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstAdditionalViewModel = GetCandidateFirstAdditionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                    };


                }

        */



        // *******************************************************************



        public CandidateCompareSummaryViewModel GetCandidateCompareSummaryViewModel(CandidateCompareViewModel viewModel, int candidateCount)
        {
            return new CandidateCompareSummaryViewModel()
            {
                CandidateCompareSummaryFirstViewModel = GetCandidateCompareSummaryFirstViewModel(viewModel.CandidateFirstDisplayId, viewModel.CandidateSecondDisplayId, candidateCount, viewModel.VotingDateId, viewModel.OfficeId),
                CandidateCompareSummarySecondViewModel = GetCandidateCompareSummarySecondViewModel(viewModel.CandidateFirstDisplayId, viewModel.CandidateSecondDisplayId, candidateCount, viewModel.VotingDateId, viewModel.OfficeId),
                CandidateCompareSummaryLookUpViewModel = GetCandidateCompareLookUpSecondViewModel(viewModel.CandidateFirstDisplayId, viewModel.VotingDateId, viewModel.OfficeId)
            };
        }



        public CandidateCompareSummaryFirstViewModel GetCandidateCompareSummaryFirstViewModel(int candidateFirstDisplayId, int candidatesecondDisplayId, int candidateCount , int VotingDateId, int CandidateDisplayOfficeId)
        {
            Models.ElectionVotingDateOfficeCandidate dbCandidate;
            Models.ElectionVotingDateOfficeCandidate dbRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateFirstDisplayId);

            if (dbRunningMate == null || dbRunningMate.CandidateId == 0)
            {// no running mate found. Must be candidate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateFirstDisplayId);
                if (dbCandidate.RunningMateId != 0)
                {
                    dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
                }
                VotingDateId = dbCandidate.ElectionVotingDateId;
                CandidateDisplayOfficeId = dbCandidate.OfficeId;
            }
            else
            {// running mate == candidate lookup. store the candidate for the running mate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMate.CandidateId);
                dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
                VotingDateId = dbRunningMate.ElectionVotingDateId;
                CandidateDisplayOfficeId = dbRunningMate.OfficeId;
            }

            // Get all objects for view model
            Models.ElectionCandidate candidate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.CandidateId);
            Models.ElectionCandidate runningMate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.RunningMateId);
            ViewModels.VoteSmart.CandidateBio voteSmartCandidate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidate.VoteSmartCandidateId);
            ViewModels.VoteSmart.CandidateBio voteSmartRunningMate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMate.VoteSmartCandidateId);
            Models.ElectionOffice candidateOffice = GetOfficeInformationForOfficeId(dbCandidate.OfficeId);
            Models.ElectionOffice runningMateOffice = GetOfficeInformationForOfficeId(dbRunningMate.OfficeId);
            Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidate.PartyId);
            Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidate.OfficeHolderId);

            return GetCandidateCompareSummaryFirstViewModel(candidateFirstDisplayId, candidatesecondDisplayId, candidateCount, VotingDateId, CandidateDisplayOfficeId, candidate, runningMate, voteSmartCandidate, voteSmartRunningMate, candidateOffice, runningMateOffice, party, officeHolder);
        }



        // ***********************************************************************



        public CandidateCompareSummaryFirstViewModel GetCandidateCompareSummaryFirstViewModel(int candidateFirstDisplayId, 
                                                                                                int candidatesecondDisplayId,
                                                                                                int candidateCount,
                                                                                                int VotingDateId,
                                                                                                int candidateDisplayOfficeId,
                                                                                                Models.ElectionCandidate candidate,
                                                                                                Models.ElectionCandidate runningMate,
                                                                                                ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                Models.ElectionOffice candidateOffice,
                                                                                                Models.ElectionOffice runningMateOffice,
                                                                                                Models.ElectionParty party,
                                                                                                Models.ElectionOfficeHolder officeHolder)
        {
            return new CandidateCompareSummaryFirstViewModel()
            {
                CandidateFirstDisplayId = candidateFirstDisplayId,
                CandidateSecondDisplayId = candidatesecondDisplayId,
                TotalNumberOfCandidates = candidateCount,
                VotingDateId = VotingDateId,
                OfficeId = candidateDisplayOfficeId,
                CandidateCompareSummaryFirst = new CandidateCompareSummaryFirst()
                {
                    CandidateId = candidate.ElectionCandidateId,
                    FirstName = candidate.FirstName,
                    MiddleName = candidate.MiddleName,
                    LastName = candidate.LastName,
                    Suffix = candidate.Suffix,
                    OfficeName = candidateOffice.OfficeName,
                    OfficeTerm = candidateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidate.Photo, voteSmartCandidate.Gender)
                },
                RunningMateCompareSummaryFirst = new RunningMateCompareSummaryFirst()
                {
                    CandidateId = runningMate.ElectionCandidateId,
                    FirstName = runningMate.FirstName,
                    MiddleName = runningMate.MiddleName,
                    LastName = runningMate.LastName,
                    Suffix = runningMate.Suffix,
                    OfficeName = runningMateOffice.OfficeName,
                    OfficeTerm = runningMateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartRunningMate.Photo, voteSmartCandidate.Gender)
                }
            };
        }



        // ********************************************************************************************



        public CandidateCompareSummarySecondViewModel GetCandidateCompareSummarySecondViewModel(int candidateFirstDisplayId, int candidatesecondDisplayId, int candidateCount , int VotingDateId, int CandidateDisplayOfficeId)
        {
            // make sure candidate information needs to be displayed
            if (candidatesecondDisplayId <= 0)
            {// NO information. STOP!!
                return new CandidateCompareSummarySecondViewModel();
            }

            Models.ElectionVotingDateOfficeCandidate dbCandidate;
            Models.ElectionVotingDateOfficeCandidate dbRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidatesecondDisplayId);

            if (dbRunningMate == null || dbRunningMate.CandidateId == 0)
            {// no running mate found. Must be candidate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidatesecondDisplayId);
                if (dbCandidate.RunningMateId != 0)
                {
                    dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
                }
                VotingDateId = dbCandidate.ElectionVotingDateId;
                CandidateDisplayOfficeId = dbCandidate.OfficeId;
            }
            else
            {// running mate == candidate lookup. store the candidate for the running mate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMate.CandidateId);
                dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
                VotingDateId = dbRunningMate.ElectionVotingDateId;
                CandidateDisplayOfficeId = dbRunningMate.OfficeId;
            }

            // Get all objects for view model
            Models.ElectionCandidate candidate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.CandidateId);
            Models.ElectionCandidate runningMate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.RunningMateId);
            ViewModels.VoteSmart.CandidateBio voteSmartCandidate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidate.VoteSmartCandidateId);
            ViewModels.VoteSmart.CandidateBio voteSmartRunningMate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMate.VoteSmartCandidateId);
            Models.ElectionOffice candidateOffice = GetOfficeInformationForOfficeId(dbCandidate.OfficeId);
            Models.ElectionOffice runningMateOffice = GetOfficeInformationForOfficeId(dbRunningMate.OfficeId);
            Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidate.PartyId);
            Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidate.OfficeHolderId);

            return GetCandidateCompareSummarySecondViewModel(candidateFirstDisplayId, candidatesecondDisplayId, candidateCount, VotingDateId, CandidateDisplayOfficeId, candidate, runningMate, voteSmartCandidate, voteSmartRunningMate, candidateOffice, runningMateOffice, party, officeHolder);
        }




        // ***********************************************************************



        public CandidateCompareSummarySecondViewModel GetCandidateCompareSummarySecondViewModel(int candidateFirstDisplayId, 
                                                                                                int candidatesecondDisplayId, 
                                                                                                int candidateCount,
                                                                                                int VotingDateId,
                                                                                                int candidateDisplayOfficeId,
                                                                                                Models.ElectionCandidate candidate,
                                                                                                Models.ElectionCandidate runningMate,
                                                                                                ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                Models.ElectionOffice candidateOffice,
                                                                                                Models.ElectionOffice runningMateOffice,
                                                                                                Models.ElectionParty party,
                                                                                                Models.ElectionOfficeHolder officeHolder)
        {
            return new CandidateCompareSummarySecondViewModel()
            {
                CandidateFirstDisplayId = candidateFirstDisplayId,
                CandidateSecondDisplayId = candidatesecondDisplayId,
                TotalNumberOfCandidates = candidateCount,
                VotingDateId = VotingDateId,
                OfficeId = candidateDisplayOfficeId,
                CandidateCompareSummarySecond = new CandidateCompareSummarySecond()
                {
                    CandidateId = candidate.ElectionCandidateId,
                    FirstName = candidate.FirstName,
                    MiddleName = candidate.MiddleName,
                    LastName = candidate.LastName,
                    Suffix = candidate.Suffix,
                    OfficeName = candidateOffice.OfficeName,
                    OfficeTerm = candidateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidate.Photo, voteSmartCandidate.Gender)
                },
                RunningMateCompareSummarySecond = new RunningMateCompareSummarySecond()
                {
                    CandidateId = runningMate.ElectionCandidateId,
                    FirstName = runningMate.FirstName,
                    MiddleName = runningMate.MiddleName,
                    LastName = runningMate.LastName,
                    Suffix = runningMate.Suffix,
                    OfficeName = runningMateOffice.OfficeName,
                    OfficeTerm = runningMateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartRunningMate.Photo, voteSmartCandidate.Gender)
                }
            };
        }



        // ********************************************************************************************


        /*
                public CandidateCompareDisplayViewModel GetCandidateCompareDisplayViewModel(CandidateCompareDisplayViewModel candidateCompareDisplayVM)
                {
                    // variables
                    int votingDateId = candidateCompareDisplayVM.VotingDateId;
                    int officeId = candidateCompareDisplayVM.OfficeId;
                    int candidateFirstDisplayId = candidateCompareDisplayVM.CandidateFirstDisplayId;
                    int candidateSecondDisplayId = candidateCompareDisplayVM.CandidateSecondDisplayId;
                    Models.ElectionVotingDateOfficeCandidate dbCandidateFirst;
                    Models.ElectionVotingDateOfficeCandidate dbRunningMateFirst = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateCompareDisplayVM.CandidateFirstDisplayId);
                    Models.ElectionVotingDateOfficeCandidate dbCandidateSecond;
                    Models.ElectionVotingDateOfficeCandidate dbRunningMateSecond;

                    // First CANDIDATE Candidate/RunningMate from DB
                    if (dbRunningMateFirst == null || dbRunningMateFirst.CandidateId == 0)
                    {// no running mate found. Must be candidate
                        dbCandidateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateCompareDisplayVM.CandidateFirstDisplayId);
                        if (dbCandidateFirst.RunningMateId != 0)
                        {
                            dbRunningMateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateFirst.RunningMateId);
                        }
                    }
                    else
                    {// running mate == candidate lookup. store the candidate for the running mate
                        dbCandidateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMateFirst.CandidateId);
                        dbRunningMateFirst = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateFirst.RunningMateId);
                    }

                    // First CANDIDATE Get all objects for view model
                    Models.ElectionCandidate candidateFirst = GetCandidateNameForCandidateIdFromDatabase(dbCandidateFirst.CandidateId);
                    Models.ElectionCandidate runningMateFirst = GetCandidateNameForCandidateIdFromDatabase(dbCandidateFirst.RunningMateId);
                    ViewModels.VoteSmart.CandidateBio voteSmartCandidateFirst = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidateFirst.VoteSmartCandidateId);
                    ViewModels.VoteSmart.CandidateBio voteSmartRunningMateFirst = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMateFirst.VoteSmartCandidateId);
                    Models.ElectionOffice candidateFirstOffice = GetOfficeInformationForOfficeId(dbCandidateFirst.OfficeId);
                    Models.ElectionOffice runningMateFirstOffice = GetOfficeInformationForOfficeId(dbRunningMateFirst.OfficeId);
                    Models.ElectionParty candidateFirstParty = GetPartyNameForPartyIdFromDatabase(dbCandidateFirst.PartyId);
                    Models.ElectionOfficeHolder candidateFirstOfficeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidateFirst.OfficeHolderId);

                    // Second CANDIDATE create objects variables for view model
                    Models.ElectionCandidate candidateSecond = null;
                    Models.ElectionCandidate runningMateSecond = null;
                    ViewModels.VoteSmart.CandidateBio voteSmartCandidateSecond = null;
                    ViewModels.VoteSmart.CandidateBio voteSmartRunningMateSecond = null;
                    Models.ElectionOffice candidateSecondOffice = null;
                    Models.ElectionOffice runningMateSecondOffice = null;
                    Models.ElectionParty candidateSecondParty = null;
                    Models.ElectionOfficeHolder candidateSecondOfficeHolder = null;

                    // Second CANDIDATE Candidate/RunningMate from DB if one has been selected
                    if (candidateSecondDisplayId <= 0)
                    {
                        dbCandidateSecond = new Models.ElectionVotingDateOfficeCandidate();
                        dbRunningMateSecond = new Models.ElectionVotingDateOfficeCandidate();
                    }
                    else
                    {
                        dbRunningMateSecond = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateCompareDisplayVM.CandidateSecondDisplayId);

                        if (dbRunningMateSecond == null || dbRunningMateSecond.CandidateId == 0)
                        {// no running mate found. Must be candidate
                            dbCandidateSecond = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateCompareDisplayVM.CandidateSecondDisplayId);
                            if (dbCandidateSecond.RunningMateId != 0)
                            {
                                dbRunningMateSecond = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateSecond.RunningMateId);
                            }
                        }
                        else
                        {// running mate == candidate lookup. store the candidate for the running mate
                            dbCandidateSecond = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMateSecond.CandidateId);
                            dbRunningMateSecond = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateSecond.RunningMateId);

                            // Get all objects for view model
                            candidateSecond = GetCandidateNameForCandidateIdFromDatabase(dbCandidateSecond.CandidateId);
                            runningMateSecond = GetCandidateNameForCandidateIdFromDatabase(dbCandidateSecond.RunningMateId);
                            voteSmartCandidateSecond = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidateSecond.VoteSmartCandidateId);
                            voteSmartRunningMateSecond = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMateSecond.VoteSmartCandidateId);
                            candidateSecondOffice = GetOfficeInformationForOfficeId(dbCandidateSecond.OfficeId);
                            runningMateSecondOffice = GetOfficeInformationForOfficeId(dbRunningMateSecond.OfficeId);
                            candidateSecondParty = GetPartyNameForPartyIdFromDatabase(dbCandidateSecond.PartyId);
                            candidateSecondOfficeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidateSecond.OfficeHolderId);
                        }
                    }

                    // load view model with objects
                    return new CandidateCompareDisplayViewModel()
                    {
                        CandidateCompareSummaryViewModel = GetCandidateCompareSummaryViewModel(candidateFirstDisplayId, candidateSecondDisplayId,
                                                                                                votingDateId, 
                                                                                                officeId, 
                                                                                                candidateFirst, candidateSecond,
                                                                                                runningMateFirst, runningMateSecond,
                                                                                                voteSmartCandidateFirst, voteSmartCandidateSecond,
                                                                                                voteSmartRunningMateFirst, voteSmartRunningMateSecond,
                                                                                                candidateFirstOffice, candidateSecondOffice,
                                                                                                runningMateFirstOffice, runningMateSecondOffice,
                                                                                                candidateFirstParty, candidateSecondParty,
                                                                                                candidateFirstOfficeHolder, candidateSecondOfficeHolder),
                        // OfficeSummaryViewModel
                        //CandidateFirstPoliticalViewModel = GetCandidateFirstPoliticalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstCaucusViewModel = GetCandidateFirstCaucusInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstProfessionalViewModel = GetCandidateFirstProfessionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstEducationViewModel = GetCandidateFirstEducationInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstPersonalViewModel = GetCandidateFirstPersonalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        // ContactViewModel
                        //CandidateFirstCivicViewModel = GetCandidateFirstCivicInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                        //CandidateFirstAdditionalViewModel = GetCandidateFirstAdditionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateCompareDisplayVM.CandidateFirstDisplayId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId),
                    };
                }
        */


        // ************************************************************************



        /*
                public CandidateCompareSummaryViewModel GetCandidateCompareSummaryViewModel(int candidateFirstDisplayId, int candidateSecondDisplayId,
                                                                                            int votingDateId, 
                                                                                            int officeId,
                                                                                            Models.ElectionCandidate candidateFirst, Models.ElectionCandidate candidateSecond,
                                                                                            Models.ElectionCandidate runningMateFirst, Models.ElectionCandidate runningMateSecond,
                                                                                            ViewModels.VoteSmart.CandidateBio voteSmartCandidateFirst, ViewModels.VoteSmart.CandidateBio voteSmartCandidateSecond,
                                                                                            ViewModels.VoteSmart.CandidateBio voteSmartRunningMateFirst, ViewModels.VoteSmart.CandidateBio voteSmartRunningMateSecond,
                                                                                            Models.ElectionOffice candidateFirstOffice, Models.ElectionOffice candidateSecondOffice,
                                                                                            Models.ElectionOffice runningMateFirstOffice, Models.ElectionOffice runningMateSecondOffice,
                                                                                            Models.ElectionParty candidateFirstparty, Models.ElectionParty candidateSecondparty,
                                                                                            Models.ElectionOfficeHolder candidateFirstofficeHolder, Models.ElectionOfficeHolder candidateSecondofficeHolder)
                {
                    // has a second candidate been selected to display?
                    if (candidateSecondDisplayId <= 0)
                    {// NO. provide user with list to select from
                        return new CandidateCompareSummaryViewModel()
                        {
                            CandidateCompareSummaryFirstViewModel = GetCandidateSummaryFirstViewModel(candidateFirstDisplayId, votingDateId, officeId, candidateFirst, runningMateFirst, voteSmartCandidateFirst, voteSmartRunningMateFirst, candidateFirstOffice, runningMateFirstOffice, candidateFirstparty, candidateFirstofficeHolder),
                            CandidateCompareLookUpSecondViewModel = GetCandidateSecondLookUpForCandidateCompareLookUpViewModel(candidateFirstDisplayId, votingDateId, officeId)
                        };
                    }
                    else
                    {// YES. display both candidates
                        return new CandidateCompareSummaryViewModel()
                        {
                            CandidateCompareSummaryFirstViewModel = GetCandidateSummaryFirstViewModel(candidateFirstDisplayId, votingDateId, officeId, candidateFirst, runningMateFirst, voteSmartCandidateFirst, voteSmartRunningMateFirst, candidateFirstOffice, runningMateFirstOffice, candidateFirstparty, candidateFirstofficeHolder),
                            CandidateCompareSummarySecondViewModel = GetCandidateSummarySecondViewModel(candidateFirstDisplayId, votingDateId, officeId, candidateSecond, runningMateSecond, voteSmartCandidateSecond, voteSmartRunningMateSecond, candidateSecondOffice, runningMateSecondOffice, candidateSecondparty, candidateSecondofficeHolder),
                        };
                    }
                }
        */

        /*
                public CandidateCompareSummaryFirstViewModel GetCandidateSummaryFirstViewModel(int candidateDisplayId,
                                                                                                int candidateLookUpVotingDateId,
                                                                                                int candidateLookUpOfficeId,
                                                                                                Models.ElectionCandidate candidate,
                                                                                                Models.ElectionCandidate runningMate,
                                                                                                ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                                ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                                Models.ElectionOffice candidateOffice,
                                                                                                Models.ElectionOffice runningMateOffice,
                                                                                                Models.ElectionParty party,
                                                                                                Models.ElectionOfficeHolder officeHolder)
                {
                    return new CandidateCompareSummaryFirstViewModel()
                    {
                        VotingDateId = candidateLookUpVotingDateId,

                        // identify which candidate (Candidate or RunningMate) the summary is for
                        CandidateFirstDisplayId = candidateDisplayId,
                        OfficeId = candidateLookUpOfficeId,

                        CandidateCompareSummaryFirst = new CandidateCompareSummaryFirst()
                        {
                            candidateId = candidate.ElectionCandidateId,
                            FirstName = candidate.FirstName,
                            MiddleName = candidate.MiddleName,
                            LastName = candidate.LastName,
                            Suffix = candidate.Suffix,
                            OfficeName = candidateOffice.OfficeName,
                            OfficeTerm = candidateOffice.Term,
                            PartyName = party.PartyName,
                            OfficeHolderName = officeHolder.OfficeHolderName,
                            VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidate.Photo, voteSmartCandidate.Gender)
                        },
                        RunningMateCompareSummaryFirst = new RunningMateCompareSummaryFirst()
                        {
                            RunningMateId = runningMate.ElectionCandidateId,
                            FirstName = runningMate.FirstName,
                            MiddleName = runningMate.MiddleName,
                            LastName = runningMate.LastName,
                            Suffix = runningMate.Suffix,
                            OfficeName = runningMateOffice.OfficeName,
                            OfficeTerm = runningMateOffice.Term,
                            PartyName = party.PartyName,
                            OfficeHolderName = officeHolder.OfficeHolderName,
                            VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartRunningMate.Photo, voteSmartCandidate.Gender)
                        }
                    };
                }

        */

        public CandidateCompareSummarySecondViewModel GetCandidateSummarySecondViewModel(int candidateDisplayId,
                                                                                        int candidateLookUpVotingDateId,
                                                                                        int candidateLookUpOfficeId,
                                                                                        Models.ElectionCandidate candidate,
                                                                                        Models.ElectionCandidate runningMate,
                                                                                        ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                        ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                        Models.ElectionOffice candidateOffice,
                                                                                        Models.ElectionOffice runningMateOffice,
                                                                                        Models.ElectionParty party,
                                                                                        Models.ElectionOfficeHolder officeHolder)
        {
            return new CandidateCompareSummarySecondViewModel()
            {
                VotingDateId = candidateLookUpVotingDateId,

                // identify which candidate (Candidate or RunningMate) the summary is for
                CandidateFirstDisplayId = candidateDisplayId,
                OfficeId = candidateLookUpOfficeId,

                CandidateCompareSummarySecond = new CandidateCompareSummarySecond()
                {
                    CandidateId = candidate.ElectionCandidateId,
                    FirstName = candidate.FirstName,
                    MiddleName = candidate.MiddleName,
                    LastName = candidate.LastName,
                    Suffix = candidate.Suffix,
                    OfficeName = candidateOffice.OfficeName,
                    OfficeTerm = candidateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartCandidate.Photo, voteSmartCandidate.Gender)
                },
                RunningMateCompareSummarySecond = new RunningMateCompareSummarySecond()
                {
                    CandidateId = runningMate.ElectionCandidateId,
                    FirstName = runningMate.FirstName,
                    MiddleName = runningMate.MiddleName,
                    LastName = runningMate.LastName,
                    Suffix = runningMate.Suffix,
                    OfficeName = runningMateOffice.OfficeName,
                    OfficeTerm = runningMateOffice.Term,
                    PartyName = party.PartyName,
                    OfficeHolderName = officeHolder.OfficeHolderName,
                    VoteSmartPhotoUrl = GetValidImageLocationToDisplay(voteSmartRunningMate.Photo, voteSmartCandidate.Gender)
                }
            };
        }



        public CandidateFirstPoliticalViewModel GetCandidateFirstPoliticalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                      ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                      int candidateLookUpId,
                                                                                      int candidateId,
                                                                                      int runningMateId)
        {
            return new CandidateFirstPoliticalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Political),
                RunningMatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Political)
            };
        }



        public CandidateFirstCaucusViewModel GetCandidateFirstCaucusInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                int candidateLookUpId,
                                                                                int candidateId,
                                                                                int runningMateId)
        {
            return new CandidateFirstCaucusViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.CongMembership),
                RunningMateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.CongMembership)
            };
        }



        public CandidateFirstProfessionalViewModel GetCandidateFirstProfessionalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                      ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                      int candidateLookUpId,
                                                                                      int candidateId,
                                                                                      int runningMateId)
        {
            return new CandidateFirstProfessionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Profession),
                RunningMateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Profession)
            };
        }



        public CandidateFirstEducationViewModel GetCandidateFirstEducationInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                      ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                      int candidateLookUpId,
                                                                                      int candidateId,
                                                                                      int runningMateId)
        {
            return new CandidateFirstEducationViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateEducationHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Education),
                RunningMateEducationHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Education)
            };
        }



        public CandidateFirstCivicViewModel GetCandidateFirstCivicInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                              ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                              int candidateLookUpId,
                                                                              int candidateId,
                                                                              int runningMateId)
        {
            return new CandidateFirstCivicViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartCandidate.OrgMembership),
                RunningMateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartRunningMate.OrgMembership)
            };
        }



        public CandidateFirstAdditionalViewModel GetCandidateFirstAdditionalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                        ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                        int candidateLookUpId,
                                                                                        int candidateId,
                                                                                        int runningMateId)
        {
            return new CandidateFirstAdditionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartCandidate.SpecialMsg),
                RunningMateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartRunningMate.SpecialMsg)
            };
        }



        public CandidateFirstPersonalViewModel GetCandidateFirstPersonalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                    ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                    int candidateLookUpId,
                                                                                    int candidateId,
                                                                                    int runningMateId)
        {
            return new CandidateFirstPersonalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateFamily = voteSmartCandidate.Family,
                RunningMateFamily = voteSmartRunningMate.Family,
                CandidateGender = voteSmartCandidate.Gender,
                RunningMateGender = voteSmartRunningMate.Gender,
                CandidateBirthDate = voteSmartCandidate.BirthDate,
                RunningMateBirthDate = voteSmartRunningMate.BirthDate,
                CandidateBirthPlace = voteSmartCandidate.BirthPlace,
                RunningMateBirthPlace = voteSmartRunningMate.BirthPlace,
                CandidateHomeCity = voteSmartCandidate.HomeCity,
                RunningMateHomeCity = voteSmartRunningMate.HomeCity,
                CandidateHomeState = voteSmartCandidate.HomeState,
                RunningMateHomeState = voteSmartRunningMate.HomeState,
                CandidateReligion = voteSmartCandidate.Religion,
                RunningMateReligion = voteSmartRunningMate.Religion
            };
        }



        // *****************************************************************



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
                //CandidateCompareDropDownList = GetCandidatesToCompareForDropDownList(candidateFirstDisplayId, votingDateId, officeId)
            };
        }


    /*
    public List<Models.ElectionVotingDateOfficeCandidate> GetCandidateSummaryListForCandidatesRunningForSameOfficeForCurrentElectionDateFromDatabase(int votingDateId)
    {
        List<Models.ElectionVotingDateOfficeCandidate> dbCandidate = new List<Models.ElectionVotingDateOfficeCandidate>();

        using (OhioVoterDbContext db = new OhioVoterDbContext())
        {
            dbCandidate = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == votingDateId).ToList();
        }

        if (dbCandidate == null)
            return new List<Models.ElectionVotingDateOfficeCandidate>();

        return dbCandidate;
    }
    */

    // ********************************************************************************


    /*
public List<Models.ElectionOffice> GetCompleteListOfPossibleOfficeNamesFromDatabase()
{
    using (OhioVoterDbContext db = new OhioVoterDbContext())
    {
        return db.ElectionOffices.ToList();
    }
}
*/

    /*
public List<Models.ElectionCandidate> GetCompleteListOfPossibleCandidateNamesFromDatabase()
{
    using (OhioVoterDbContext db = new OhioVoterDbContext())
    {
        return db.ElectionCandidates.ToList();
    }
}

    */


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


        /*
        public List<CandidateSummaryViewModel> GetOfficeNamesForAllDates(List<CandidateSummaryViewModel> candidates, List<Models.ElectionOffice> officeList)
        {
            for(int i = 0; i < candidates.Count; i++)
            {
                for(int j = 0; j < officeList.Count; j++)
                {
                    if(candidates[i].CandidateOfficeId == officeList[j].ElectionOfficeId)
                    {
                        candidates[i].CandidateOfficeName = officeList[j].OfficeName;
                        candidates[i].CandidateOfficeTerm = officeList[j].Term;
                        candidates[i].VoteSmartCandidateOfficeId = officeList[j].VoteSmartOfficeId;
                        j = officeList.Count;
                    }
                }
            }

            return candidates;
        }
        */

            /*
        public List<CandidateSummaryViewModel> GetCandidateNamesForAllDates(List<CandidateSummaryViewModel> candidates, List<Models.ElectionCandidate> candidateList)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                for (int j = 0; j < candidateList.Count; j++)
                {
                    if (candidates[i].CandidateId == candidateList[j].ElectionCandidateId)
                    {
                        candidates[i].CandidateFirstName = candidateList[j].FirstName;
                        candidates[i].CandidateMiddleName = candidateList[j].MiddleName;
                        candidates[i].CandidateLastName = candidateList[j].LastName;
                        candidates[i].CandidateSuffix = candidateList[j].Suffix;
                        candidates[i].VoteSmartCandidateId = candidateList[j].VoteSmartCandidateId;
                        j = candidateList.Count;
                    }
                }
            }

            return candidates;
        }
        */

            /*
        public List<CandidateSummaryViewModel> GetRunningMateNamesForAllDates(List<CandidateSummaryViewModel> candidates, List<Models.ElectionCandidate> candidateList)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                for (int j = 0; j < candidateList.Count; j++)
                {
                    if (candidates[i].RunningMateId == candidateList[j].ElectionCandidateId)
                    {
                        candidates[i].RunningMateFirstName = candidateList[j].FirstName;
                        candidates[i].RunningMateMiddleName = candidateList[j].MiddleName;
                        candidates[i].RunningMateLastName= candidateList[j].LastName;
                        candidates[i].RunningMateSuffix = candidateList[j].Suffix;
                        candidates[i].VoteSmartRunningMateId = candidateList[j].VoteSmartCandidateId;
                        j = candidateList.Count;
                    }
                }
            }

            return candidates;
        }
        */

        /*
        private int GetFirstActiveElectionDateId()
        {
            List<Models.ElectionVotingDate> dates = GetActiveElectionDatesFromDatabase();

            if (dates[0].ElectionVotingDateId == 0)
                return 0;

            return dates[0].ElectionVotingDateId;
        }



        private Models.ElectionVotingDate GetFirstActiveElectionDate()
        {
            List<Models.ElectionVotingDate> dates = GetActiveElectionDatesFromDatabase();

            if (dates == null || dates[0].ElectionVotingDateId == 0)
                return new Models.ElectionVotingDate();

            return dates[0];
        }
        */

        // Election Date Drop Down List
        /*
        private ElectionDateDropDownList GetActiveElectionDatesForDropDownList()
        {
            return new ElectionDateDropDownList()
            {
                Date = GetActiveElectionDateListItems()
            };
        }

            */



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


        /*
        private IEnumerable<SelectListItem> GetActiveElectionDateListItems()
        {
            List<Models.ElectionVotingDate> dbDates = GetActiveElectionDatesInOrderByDateFromDatabase();
            List<SelectListItem> dates = new List<SelectListItem>();

            for (int i = 0; i < dbDates.Count; i++)
            {
                dates.Add(new SelectListItem()
                {
                    Value = dbDates[i].ElectionVotingDateId.ToString(),
                    Text = dbDates[i].Date.ToShortDateString()
                });
            }

            return dates;
        }
        */


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



        // *********************
        // Office Drop Down List
        // *********************


        /*
    public OfficeDropDownList GetOfficesForDropDownList(int dateId)
    {
        return new OfficeDropDownList()
        {
            OfficeNames = GetOfficeListItems(dateId)
        };
    }

        */


        /*
    private IEnumerable<SelectListItem> GetOfficeListItems(int dateId)
    {
        if (dateId <= 0)
            return new List<SelectListItem>();

        List<int> dbOfficeIds = GetOfficesForCurrentDateFromDatabase(dateId);
        List<Models.ElectionOffice> dbOffices = GetOfficeNamesForOfficeId(dbOfficeIds);
        List <SelectListItem> offices = new List<SelectListItem>();

        for (int i = 0; i < dbOffices.Count; i++)
        {
            string OfficeName = dbOffices[i].OfficeName;

            if (dbOffices[i].Term != null && dbOffices[i].Term != "")
            {
                OfficeName = string.Format("{0} ({1})", dbOffices[i].OfficeName, dbOffices[i].Term);
            }

            offices.Add(new SelectListItem()
            {
                Value = dbOffices[i].ElectionOfficeId.ToString(),
                Text = OfficeName
            });
        }

        return offices;
    }
    */


        /*
    private List<int> GetOfficesForCurrentDateFromDatabase(int dateId)
    {
        using (OhioVoterDbContext db = new OhioVoterDbContext())
        {
            //List<Models.ElectionVotingDateOfficeCandidate> dbOffices = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == dateId).Distinct().ToList();
            List<int> dbOffices = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == dateId)
                                                                       .Select(x => x.OfficeId)
                                                                       .ToList();
            return dbOffices.Distinct().ToList();
        }
    }
    */

        /*
    private List<Models.ElectionOffice> GetOfficeNamesForOfficeId(List<int> officeIds)
    {
        List<Models.ElectionOffice> dbOffices = new List<ElectionOffice>();
        List<Models.ElectionOffice> offices = new List<ElectionOffice>();

        using (OhioVoterDbContext db = new OhioVoterDbContext())
        {
            dbOffices = db.ElectionOffices.ToList();
        }

        for (int i = 0; i < officeIds.Count; i++)
        {
            for (int j = 0; j < dbOffices.Count; j++)
            {
                if (officeIds[i] == dbOffices[j].ElectionOfficeId)
                {
                    offices.Add(new Models.ElectionOffice()
                    {
                        ElectionOfficeId = dbOffices[j].ElectionOfficeId,
                        OfficeName = dbOffices[j].OfficeName,
                        Term = dbOffices[j].Term
                    });

                    j = dbOffices.Count;
                }
            }
        }

        return offices.OrderBy(x => x.OfficeName).OrderBy(x => x.ElectionOfficeId).ToList();
    }
    */



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
                return new List<SelectListItem>();

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







        



        // ***********************************************************
        // ***********************************************************
        // ***********************************************************


            /*
        /// <summary>
        /// Get a list of candidates based on supplied last name
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public List<Models.ElectionCandidate> GetCandidatesWithMatchingSuppliedLastName(string lastName)
        {
            List<Models.ElectionCandidate> candidates = new List<ElectionCandidate>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                candidates = db.ElectionCandidates.ToList();
                candidates = candidates.Where(x => x.LastName == lastName).ToList();
            }

            return candidates;
        }
        */

        /*
        public List<Candidate> GetListOfCandidatesWithMatchingFirstAndLastNameFromVoteSmart(string firstName, string lastName, int year, string stageId)
        {
            List<Candidate> candidates = new List<Candidate>();
//            List<Candidate> candidates = GetVoteSmartCadidateViewModel(lastName, year, stageId);
            List<Candidate> filteredCandidates = new List<Candidate>();

            for(int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].FirstName == firstName)
                {
                    Candidate candidate = new Candidate()
                    {
                        ElectionCandidateId = candidates[i].ElectionCandidateId,
                        FirstName = candidates[i].FirstName,
                        LastName = candidates[i].LastName
                    };
                    filteredCandidates.Add(candidate);
                }
            }

            return candidates;
        }
        */

        
            /*
        /// <summary>
        /// get list of candidates from VoteSmart API matching supplied last name
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public List<ViewModels.VoteSmart.Candidate> GetListOfCandidatesWithMatchingLastNameFromVoteSmart(string lastName, int year, string stageId)
        {
            VoteSmartApiManagement voteSmartApi = new VoteSmartApiManagement();
            return voteSmartApi.GetVoteSmartMatchingCandidateListFromSuppliedLastNameInASpecifiedElectionYear(lastName, year, stageId);
        }
        */

            /*
        /// <summary>
        /// get list of candidates from VoteSmart API similar to supplied last name
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public List<ViewModels.VoteSmart.Candidate> GetListOfCandidatesWithSimilarLastNameFromVoteSmart(string lastName)
        {
            VoteSmartApiManagement voteSmartApi = new VoteSmartApiManagement();
            return voteSmartApi.GetVoteSmartSimilarCandidateListFromSuppliedLastName(lastName);
        }
        */


    }
}