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
        // GET: Candidate
        public ActionResult Index(int? candidateId = 0)
        {
            if (candidateId == null || candidateId < 0)
            {
                candidateId = 0;
            }

            string controllerName = "Candidate";
            UpdateSessionWithNewControllerNameForSideBar(controllerName);

            CandidateViewModel viewModel = new CandidateViewModel()
            {
                ControllerName = controllerName,
                CandidateId = (int) candidateId
            };

            return View(viewModel);
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
                    ControllerName = model.ControllerName,
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
                    VotingDate = date.Date.ToShortTimeString(),
                    CandidateDropDownList = GetCandidatesForDropDownList(date.ElectionVotingDateId)
                };

                return PartialView("_CandidateLookUp", candidateLookUpVM);
            }
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
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { candidateId = 0 });

            int candidateId = model.CandidateDropDownList.SelectedCandidateId;

            return RedirectToAction("Index", new { candidateId = candidateId });
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
            Models.ElectionVotingDateOfficeCandidate dbCandidate;
            Models.ElectionVotingDateOfficeCandidate dbRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateDisplayVM.CandidateLookUpId);
            if(dbRunningMate == null || dbRunningMate.CandidateId == 0)
            {// no running mate found. Must be candidate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateDisplayVM.CandidateLookUpId);
                if(dbCandidate.RunningMateId != 0)
                {
                    dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
                }
            }
            else
            {// running mate found. store the candidate for the running mate
                dbCandidate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbRunningMate.CandidateId);
                dbRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidate.RunningMateId);
            }

            // Gett all objects for view model
            Models.ElectionCandidate candidate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.CandidateId);
            Models.ElectionCandidate runningMate = GetCandidateNameForCandidateIdFromDatabase(dbCandidate.RunningMateId);
            ViewModels.VoteSmart.CandidateBio voteSmartCandidate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(candidate.VoteSmartCandidateId);
            ViewModels.VoteSmart.CandidateBio voteSmartRunningMate = GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(runningMate.VoteSmartCandidateId);
            Models.ElectionOffice candidateOffice = GetOfficeInformationForOfficeId(dbCandidate.OfficeId);
            Models.ElectionOffice runningMateOffice = GetOfficeInformationForOfficeId(dbRunningMate.OfficeId);
            Models.ElectionParty party = GetPartyNameForPartyIdFromDatabase(dbCandidate.PartyId);
            Models.ElectionOfficeHolder officeHolder = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbCandidate.OfficeHolderId);

            // load view model with objects
            candidateDisplayVM.CandidateSummaryViewModel = GetCandidateSummaryViewModel(candidate, runningMate, voteSmartCandidate, voteSmartRunningMate, candidateOffice, runningMateOffice, party, officeHolder);
            // OfficeSummaryViewModel
            candidateDisplayVM.PoliticalViewModel = GetPoliticalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.CaucusViewModel = GetCaucusInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.ProfessionalViewModel = GetProfessionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.EducationViewModel = GetEducationInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.PersonalViewModel = GetPersonalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            // ContactViewModel
            candidateDisplayVM.CivicViewModel = GetCivicInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);
            candidateDisplayVM.AdditionalViewModel = GetAdditionalInformationForCandidateBioFromVoteSmart(voteSmartCandidate, voteSmartRunningMate, candidateDisplayVM.CandidateLookUpId, candidate.ElectionCandidateId, runningMate.ElectionCandidateId);

            return candidateDisplayVM;
        }



        // ***********************************************************************



        public CandidateSummaryViewModel GetCandidateSummaryViewModel(Models.ElectionCandidate candidate,
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
                CandidateSummary = new CandidateSummary()
                {
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



        public ViewModels.VoteSmart.CandidateBio GetCandidateInformationForVoteSmartCandidateIdFromVoteSmart(string voteSmartCandidateId)
        {
            VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();
            return voteSmart.GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(voteSmartCandidateId);
        }



        public PoliticalViewModel GetPoliticalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                      ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                      int candidateLookUpId,
                                                                                      int candidateId,
                                                                                      int runningMateId)
        {
            return new PoliticalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Political),
                RunningMatePoliticalHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Political)
            };
        }



        public CaucusViewModel GetCaucusInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                int candidateLookUpId,
                                                                                int candidateId,
                                                                                int runningMateId)
        {
            return new CaucusViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.CongMembership),
                RunningMateCaucusHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.CongMembership)
            };
        }



        public ProfessionalViewModel GetProfessionalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                      ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                      int candidateLookUpId,
                                                                                      int candidateId,
                                                                                      int runningMateId)
        {
            return new ProfessionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Profession),
                RunningMateProfessionalHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Profession)
            };
        }



        public EducationViewModel GetEducationInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                      ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                      int candidateLookUpId,
                                                                                      int candidateId,
                                                                                      int runningMateId)
        {
            return new EducationViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateEducationHistory = GetListFromStringWithLineBreaks(voteSmartCandidate.Education),
                RunningMateEducationHistory = GetListFromStringWithLineBreaks(voteSmartRunningMate.Education)
            };
        }



        public CivicViewModel GetCivicInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                              ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                              int candidateLookUpId,
                                                                              int candidateId,
                                                                              int runningMateId)
        {
            return new CivicViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartCandidate.OrgMembership),
                RunningMateCivicMemberships = GetListFromStringWithLineBreaks(voteSmartRunningMate.OrgMembership)
            };
        }



        public AdditionalViewModel GetAdditionalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                        ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                        int candidateLookUpId,
                                                                                        int candidateId,
                                                                                        int runningMateId)
        {
            return new AdditionalViewModel()
            {
                CandidateLookUpId = candidateLookUpId,
                CandidateId = candidateId,
                RunningMateId = runningMateId,
                CandidateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartCandidate.SpecialMsg),
                RunningMateAdditionalInformation = GetListFromStringWithLineBreaks(voteSmartRunningMate.SpecialMsg)
            };
        }



        public PersonalViewModel GetPersonalInformationForCandidateBioFromVoteSmart(ViewModels.VoteSmart.CandidateBio voteSmartCandidate,
                                                                                    ViewModels.VoteSmart.CandidateBio voteSmartRunningMate,
                                                                                    int candidateLookUpId,
                                                                                    int candidateId,
                                                                                    int runningMateId)
        {
            return new PersonalViewModel()
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



        public string GetValidImageLocationToDisplay(string voteSmartURL, string gender)
        {
            if (voteSmartURL != null && voteSmartURL != "")
                return voteSmartURL;

            if (gender == "Female")
                return "~/Content/images/image_female.png";

            return "~/Content/images/image_male.png";
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





        // ***************************************************************************

        /*
    public CandidateDisplayViewModel GetCandidateInformationForCandidateSummary(CandidateDisplayViewModel candidate)
    {
        // get candidateLookUp information
        Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, date.ElectionVotingDateId);

        // add supplied information to candidate object
        CandidateSummaryViewModel candidate = new CandidateSummaryViewModel()
        {
            CandidateLookUpId = candidateLookUpId,
            ElectionVotingDateId = date.ElectionVotingDateId,
            VotingDate = date.Date
        };

        // determine the type of candidate that is being looked up
        Models.ElectionVotingDateOfficeCandidate dbRunningMateLookUp = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, date.ElectionVotingDateId);

        if (candidateLookUpId == dbRunningMateLookUp.RunningMateId)
        {// candidateLookUp is a running mate
            candidate = GetCandidateInformationForRunningMateId(candidateLookUpId, date.ElectionVotingDateId, dbCandidateLookUp, candidate);
        }
        else
        {// candidateLookUp is a candidate
            candidate = GetCandidateInformationForCandidateId(date.ElectionVotingDateId, dbCandidateLookUp, candidate);
        }

        return candidate;
    }
    */

        /*
    /// <summary>
    /// look up candidate information for selected candidate and current election based on supplied candidateId
    /// </summary>
    /// <param name="candidateLookUpId"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    public CandidateSummaryViewModel GetCandidateSummaryInformationForSelectedCandidateId(int candidateLookUpId, ElectionVotingDate date)
    {
        // get candidateLookUp information
        Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, date.ElectionVotingDateId);

        // add supplied information to candidate object
        CandidateSummaryViewModel candidate = new CandidateSummaryViewModel()
        {
            CandidateLookUpId = candidateLookUpId,
            ElectionVotingDateId = date.ElectionVotingDateId,
            VotingDate = date.Date
        };

        // determine the type of candidate that is being looked up
        Models.ElectionVotingDateOfficeCandidate dbRunningMateLookUp = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, date.ElectionVotingDateId);

        if (candidateLookUpId == dbRunningMateLookUp.RunningMateId)
        {// candidateLookUp is a running mate
            candidate = GetCandidateInformationForRunningMateId(candidateLookUpId, date.ElectionVotingDateId, dbCandidateLookUp, candidate);
        }
        else
        {// candidateLookUp is a candidate
            candidate = GetCandidateInformationForCandidateId(date.ElectionVotingDateId, dbCandidateLookUp, candidate);
        }

        return candidate;
    }
    */


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

            if (dbRunningMate == null )
                return new Models.ElectionVotingDateOfficeCandidate();

            return dbRunningMate;
        }



        // ****************************************************
        

            /*
        public CandidateSummaryViewModel GetCandidateInformationForRunningMateId(int candidateLookUpId, int dateId, Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp, CandidateSummaryViewModel candidate)
        {
            Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, dateId);

            candidate = GetCandidateElectionSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetCandidateNameSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetCandidateOfficeSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetRunningMateNameSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetRunningMateOfficeSummaryForRunningMateId(candidate, dbCandidateLookUp, dbElectionRunningMate);

            return candidate;
        }
        */

        /*
        public CandidateSummaryViewModel GetCandidateInformationForCandidateId(int dateId, Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp, CandidateSummaryViewModel candidate)
        {
            Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateLookUp.RunningMateId, dateId);

            candidate = GetCandidateElectionSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetCandidateNameSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetCandidateOfficeSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetRunningMateNameSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetRunningMateOfficeSummaryForCandidateId(dateId, candidate, dbElectionRunningMate);

            return candidate;
        }
        */


        // ****************************************************************************


            /*
        public CandidateSummaryViewModel GetCandidateElectionSummaryForRunningMateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            candidate.OfficeHolderId = dbElectionRunningMate.OfficeHolderId;
            candidate.OfficeHolderName = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbElectionRunningMate.OfficeHolderId);
            candidate.PartyId = dbElectionRunningMate.PartyId;
            candidate.PartyName = GetPartyNameForPartyIdFromDatabase(dbElectionRunningMate.PartyId.ToString());
            candidate.CertifiedCandidateId = dbElectionRunningMate.CertifiedCandidateId;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetCandidateNameSummaryForRunningMateId(CandidateSummaryViewModel candidate,  Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            Models.ElectionCandidate dbCandidate = GetCandidateNameForCandidateIdFromDatabase(dbElectionRunningMate.CandidateId);

            candidate.CandidateId = dbCandidate.ElectionCandidateId;
            candidate.VoteSmartCandidateId = dbCandidate.VoteSmartCandidateId;
            candidate.CandidateFirstName = dbCandidate.FirstName;
            candidate.CandidateMiddleName = dbCandidate.MiddleName;
            candidate.CandidateLastName = dbCandidate.LastName;
            candidate.CandidateSuffix = dbCandidate.Suffix;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetCandidateOfficeSummaryForRunningMateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbElectionRunningMate.OfficeId);

            candidate.CandidateOfficeId = dbElectionRunningMate.OfficeId;
            candidate.VoteSmartCandidateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.CandidateOfficeName = dbOffice.OfficeName;
            candidate.CandidateOfficeTerm = dbOffice.Term;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetRunningMateNameSummaryForRunningMateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            Models.ElectionCandidate dbRunningMate = GetCandidateNameForCandidateIdFromDatabase(dbElectionRunningMate.RunningMateId);

            candidate.RunningMateId = dbRunningMate.ElectionCandidateId;
            candidate.VoteSmartRunningMateId = dbRunningMate.VoteSmartCandidateId;
            candidate.RunningMateFirstName = dbRunningMate.FirstName;
            candidate.RunningMateMiddleName = dbRunningMate.MiddleName;
            candidate.RunningMateLastName = dbRunningMate.LastName;
            candidate.RunningMateSuffix = dbRunningMate.Suffix;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetRunningMateOfficeSummaryForRunningMateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbCandidateLookUp.OfficeId);

            candidate.RunningMateOfficeId = dbCandidateLookUp.OfficeId;
            candidate.VoteSmartRunningMateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.RunningMateOfficeName = dbOffice.OfficeName;
            candidate.RunningMateOfficeTerm = dbOffice.Term;

            return candidate;
        }
        */


        // ******************************************************************************


            /*
        public CandidateSummaryViewModel GetCandidateElectionSummaryForCandidateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            candidate.OfficeHolderId = dbElectionCandidate.OfficeHolderId;
            candidate.OfficeHolderName = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbElectionCandidate.OfficeHolderId);
            candidate.PartyId = dbElectionCandidate.PartyId;
            candidate.PartyName = GetPartyNameForPartyIdFromDatabase(dbElectionCandidate.PartyId.ToString());
            candidate.CertifiedCandidateId = dbElectionCandidate.CertifiedCandidateId;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetCandidateNameSummaryForCandidateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            Models.ElectionCandidate dbCandidate = GetCandidateNameForCandidateIdFromDatabase(dbElectionCandidate.CandidateId);

            candidate.CandidateId = dbCandidate.ElectionCandidateId;
            candidate.VoteSmartCandidateId = dbCandidate.VoteSmartCandidateId;
            candidate.CandidateFirstName = dbCandidate.FirstName;
            candidate.CandidateMiddleName = dbCandidate.MiddleName;
            candidate.CandidateLastName = dbCandidate.LastName;
            candidate.CandidateSuffix = dbCandidate.Suffix;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetCandidateOfficeSummaryForCandidateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbElectionCandidate.OfficeId);

            candidate.CandidateOfficeId = dbElectionCandidate.OfficeId;
            candidate.VoteSmartCandidateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.CandidateOfficeName = dbOffice.OfficeName;
            candidate.CandidateOfficeTerm = dbOffice.Term;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetRunningMateNameSummaryForCandidateId(CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            Models.ElectionCandidate dbRunningMate = GetCandidateNameForCandidateIdFromDatabase(dbElectionCandidate.RunningMateId);

            candidate.RunningMateId = dbRunningMate.ElectionCandidateId;
            candidate.VoteSmartRunningMateId = dbRunningMate.VoteSmartCandidateId;
            candidate.RunningMateFirstName = dbRunningMate.FirstName;
            candidate.RunningMateMiddleName = dbRunningMate.MiddleName;
            candidate.RunningMateLastName = dbRunningMate.LastName;
            candidate.RunningMateSuffix = dbRunningMate.Suffix;

            return candidate;
        }
        */

            /*
        public CandidateSummaryViewModel GetRunningMateOfficeSummaryForCandidateId(int dateId, CandidateSummaryViewModel candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            Models.ElectionVotingDateOfficeCandidate dbRunningMateLookUp = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbElectionCandidate.CandidateId, dateId);
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbRunningMateLookUp.OfficeId);

            candidate.RunningMateOfficeId = dbRunningMateLookUp.OfficeId;
            candidate.VoteSmartRunningMateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.RunningMateOfficeName = dbOffice.OfficeName;
            candidate.RunningMateOfficeTerm = dbOffice.Term;

            return candidate;
        }
        */


        // ******************************************************************************


        /*
        public CandidateSummaryViewModel GetCandidateBiographyFromVoteSmart(CandidateSummaryViewModel candidate)
        {
            VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();
            ViewModels.VoteSmart.CandidateBio candidateBio = voteSmart.GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(candidate.VoteSmartCandidateId);
            candidate = UpdateCandidateSummaryWithCandidateBiographyFromVoteSmart(candidate, candidateBio);

            if (candidate.RunningMateId > 0)
            {
                ViewModels.VoteSmart.CandidateBio runningMateBio = voteSmart.GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(candidate.VoteSmartRunningMateId);
                candidate = UpdateCandidateSummaryWithRunningMateBiographyFromVoteSmart(candidate, runningMateBio);
            }

            return candidate;
         }
         */

            /*
        public CandidateSummaryViewModel UpdateCandidateSummaryWithCandidateBiographyFromVoteSmart(CandidateSummaryViewModel candidate, ViewModels.VoteSmart.CandidateBio candidateBio)
        {
            candidate.OpenSecretsCandidateId = candidateBio.CrpId;
            candidate.VoteSmartCandidateNickName = candidateBio.NickName;
            candidate.VoteSmartCandidateMiddleName = candidateBio.MiddleName;
            candidate.VoteSmartCandidatePreferredName = candidateBio.PreferredName;
            candidate.VoteSmartCandidateBirthDate = candidateBio.BirthDate;
            candidate.VoteSmartCandidateBirthPlace = candidateBio.BirthPlace;
            candidate.VoteSmartCandidatePronunciation = candidateBio.Pronunciation;
            candidate.VoteSmartCandidateGender = candidateBio.Gender;
            candidate.VoteSmartCandidatePhotoUrl = GetValidImageLocationToDisplay(candidateBio.Photo, candidateBio.Gender);
            candidate.VoteSmartCandidateFamily = GetListFromStringWithLineBreaks(candidateBio.Family);
            candidate.VoteSmartCandidateHomeCity = candidateBio.HomeCity;
            candidate.VoteSmartCandidateHomeState = candidateBio.HomeState;
            candidate.VoteSmartCandidateEducation = GetListFromStringWithLineBreaks(candidateBio.Education);
            candidate.VoteSmartCandidateProfession = GetListFromStringWithLineBreaks(candidateBio.Profession);
            candidate.VoteSmartCandidatePolitical = GetListFromStringWithLineBreaks(candidateBio.Political);
            candidate.VoteSmartCandidateReligion = candidateBio.Religion;
            candidate.VoteSmartCandidateCongMembership = GetListFromStringWithLineBreaks(candidateBio.CongMembership);
            candidate.VoteSmartCandidateOrgMembership = GetListFromStringWithLineBreaks(candidateBio.OrgMembership);
            candidate.VoteSmartCandidateSpecialMsg = GetListFromStringWithLineBreaks(candidateBio.SpecialMsg);

            return candidate;
        }
        */


            /*
        public CandidateSummaryViewModel UpdateCandidateSummaryWithRunningMateBiographyFromVoteSmart(CandidateSummaryViewModel candidate, ViewModels.VoteSmart.CandidateBio runningMateBio)
        {
            candidate.OpenSecretsRunningMateId = runningMateBio.CrpId;
            candidate.VoteSmartRunningMateNickName = runningMateBio.NickName;
            candidate.VoteSmartRunningMateMiddleName = runningMateBio.MiddleName;
            candidate.VoteSmartRunningMatePreferredName = runningMateBio.PreferredName;
            candidate.VoteSmartRunningMateBirthDate = runningMateBio.BirthDate;
            candidate.VoteSmartRunningMateBirthPlace = runningMateBio.BirthPlace;
            candidate.VoteSmartRunningMatePronunciation = runningMateBio.Pronunciation;
            candidate.VoteSmartRunningMateGender = runningMateBio.Gender;
            candidate.VoteSmartRunningMatePhotoUrl = GetValidImageLocationToDisplay(runningMateBio.Photo, runningMateBio.Gender);
            candidate.VoteSmartRunningMateFamily = GetListFromStringWithLineBreaks(runningMateBio.Family);
            candidate.VoteSmartRunningMateHomeCity = runningMateBio.HomeCity;
            candidate.VoteSmartRunningMateHomeState = runningMateBio.HomeState;
            candidate.VoteSmartRunningMateEducation = GetListFromStringWithLineBreaks(runningMateBio.Education);
            candidate.VoteSmartRunningMateProfession = GetListFromStringWithLineBreaks(runningMateBio.Profession);
            candidate.VoteSmartRunningMatePolitical = GetListFromStringWithLineBreaks(runningMateBio.Political);
            candidate.VoteSmartRunningMateReligion = runningMateBio.Religion;
            candidate.VoteSmartRunningMateCongMembership = GetListFromStringWithLineBreaks(runningMateBio.CongMembership);
            candidate.VoteSmartRunningMateOrgMembership = GetListFromStringWithLineBreaks(runningMateBio.OrgMembership);
            candidate.VoteSmartRunningMateSpecialMsg = GetListFromStringWithLineBreaks(runningMateBio.SpecialMsg);

            return candidate;
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


        private List<int> GetCandidatesForCurrentDateFromDatabase(int dateId)
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                List<Models.ElectionVotingDateOfficeCandidate> dbOffices = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == dateId).ToList();
                return dbOffices.Select(x => x.CandidateId).Distinct().ToList();
            }
        }



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