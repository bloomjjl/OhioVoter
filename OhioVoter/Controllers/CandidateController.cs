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
                candidateId = 0;

            CandidateViewModel viewModel = new CandidateViewModel()
            {
                Candidate = GetCandidateSummaryViewModel((int) candidateId),
                SideBarViewModel = GetSideBarViewModel(),
                ElectionDate = GetFirstActiveElectionDate(),
                CandidateDropDownList = GetCandidateDropDownListViewModel()
            };

            return View(viewModel);
        }



        private CandidateSummary GetCandidateSummaryViewModel(int candidateId)
        {
            if (candidateId != 0)
            {
                ElectionVotingDate date = GetFirstActiveElectionDate();
                return GetCandidateSummaryInformationForSelectedCandidateId(candidateId, date);
            }
            else
            {
                return new CandidateSummary();
            }
        }



        private ViewModels.Location.SideBarViewModel GetSideBarViewModel()
        {
            LocationController location = new LocationController();
            return location.GetSideBarViewModel("Candidate");
        }



        private CandidateDropDownList GetCandidateDropDownListViewModel()
        {
            int dateId = GetFirstActiveElectionDateId(); // set default to zero to select first date found
            return GetCandidatesForDropDownList(dateId);
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
        public ActionResult Name(CandidateViewModel model)
        {
            // get selected name from model
            // first name, last name, candidate id, votesmart id
            int candidateId = model.CandidateDropDownList.SelectedCandidateId;
/*            ElectionVotingDate date = GetFirstActiveElectionDate();
            CandidateSummary candidate = GetCandidateSummaryInformationForSelectedCandidateId(candidateId, date);


            // set these two values for testing purposes
            int year = candidate.VotingDate.Year;
            string stageId = "";

            // get a list of candidates for supplied name
            List<ViewModels.VoteSmart.Candidate> VoteSmartCandidates = new List<ViewModels.VoteSmart.Candidate>();
            //            VoteSmartCandidates = GetVoteSmartCadidateViewModel(lastName, year, stageId);
            //            List<ElectionCandidate> candidates = new List<ElectionCandidate>();

            // return list of names to view
            model.Candidate = candidate;
            */
            return RedirectToAction("Index", new { candidateId = candidateId });
        }





        /// <summary>
        /// Get all the candidate information for active elections
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CandidateSummary> GetSummaryInformationForAllCandidatesForUpcomingElections()
        {
            // From Database: get list of all active election dates (ElectionVotingDate)
            // (ElectionDateId,Date)
            List<Models.ElectionVotingDate> electionDatesDb = GetActiveElectionDatesFromDatabase();

            // get election information for all dates
            List<CandidateSummary> allElectionCandidates = GetCandidateSummaryInformationForAllDates(electionDatesDb);

            // get office names for officeId
            List<Models.ElectionOffice> officeList = GetCompleteListOfPossibleOfficeNamesFromDatabase();
            allElectionCandidates = GetOfficeNamesForAllDates(allElectionCandidates, officeList);

            // get candidate/runningmate names for candidateId
            List<Models.ElectionCandidate> candidateList = GetCompleteListOfPossibleCandidateNamesFromDatabase();
            allElectionCandidates = GetCandidateNamesForAllDates(allElectionCandidates, candidateList);
            //candidates = GetRunningMateNamesForAllDates(candidates, candidateList);


            return allElectionCandidates;
        }


    




        public List<CandidateSummary> GetCandidateSummaryInformationForAllDates(List<Models.ElectionVotingDate> electionDates)
        {
            if (electionDates == null)
                return new List<CandidateSummary>();

            List<CandidateSummary> candidates = new List<CandidateSummary>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                for (int i = 0; i < electionDates.Count; i++)
                {
                    int dateId = electionDates[i].ElectionVotingDateId;
                    List<Models.ElectionVotingDateOfficeCandidate> dbAllCandidates = db.ElectionVotingDateOfficeCandidates.Where(x => x.ElectionVotingDateId == dateId).ToList();
                    
                    for (int j = 0; j < dbAllCandidates.Count; j++)
                    {
                        candidates.Add(new CandidateSummary()
                        {
                            ElectionVotingDateId = dateId,
                            VotingDate = electionDates[i].Date,
                            CandidateId = dbAllCandidates[j].CandidateId,
                            CandidateOfficeId = dbAllCandidates[j].OfficeId,
                            //CandidateFirstName = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { FirstName = x.FirstName }).ToString(),
                            //CandidateMiddleName = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { MiddleName = x.MiddleName }).ToString(),
                            //CandidateLastName = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { LastName = x.LastName }).ToString(),
                            //CandidateSuffix = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { Suffix = x.Suffix }).ToString(),
                            CertifiedCandidateId = dbAllCandidates[j].CertifiedCandidateId,
                            PartyId = dbAllCandidates[j].PartyId,
                            OfficeHolderId = dbAllCandidates[j].OfficeHolderId,
                            RunningMateId = dbAllCandidates[j].RunningMateId,
                            //RunningMateOfficeId = dbAllCandidates[j].RunningMateOfficeId,
                            //RunningMateFirstName = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { FirstName = x.FirstName }).ToString(),
                            //RunningMateMiddleName = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { MiddleName = x.MiddleName }).ToString(),
                            //RunningMateLastName = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { LastName = x.LastName }).ToString(),
                            //RunningMateSuffix = db.ElectionCandidates.Where(x => x.ElectionCandidateId == dbCandidates[j].CandidateId).Select(x => new { Suffix = x.Suffix }).ToString()
                        });
                    }
                }
            }

            return candidates;
        }



        public CandidateSummary GetCandidateSummaryInformationForSelectedCandidateId(int candidateLookUpId, ElectionVotingDate date)
        {
            // get list of all candidate/runningMates in candidateID column
            Models.ElectionVotingDateOfficeCandidate dbElectionCandidate = GetCandidateSummaryForCurrentElectionDate(candidateLookUpId, date.ElectionVotingDateId);
            Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate = GetRunningMateSummaryForCurrentElectionDate(candidateLookUpId, date.ElectionVotingDateId);
            Models.ElectionCandidate dbCandidate = GetCandidateNameForCandidateIdFromDatabase(dbElectionCandidate.CandidateId);
            Models.ElectionCandidate dbRunningMate = GetCandidateNameForCandidateIdFromDatabase(dbElectionRunningMate.RunningMateId);
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbElectionCandidate.OfficeId);

            return new CandidateSummary()
            {
                CandidateLookUpId = candidateLookUpId,

                ElectionVotingDateId = date.ElectionVotingDateId,
                VotingDate = date.Date,

                // get election information
                OfficeHolderId = dbElectionCandidate.OfficeHolderId,
                OfficeHolderName = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbElectionCandidate.OfficeHolderId),
                PartyId = dbElectionCandidate.PartyId,
                PartyName = GetPartyNameForPartyIdFromDatabase(dbElectionCandidate.PartyId.ToString()),
                CertifiedCandidateId = dbElectionCandidate.CertifiedCandidateId,

                // get candidate name 
                CandidateId = dbCandidate.ElectionCandidateId,
                VoteSmartCandidateId = dbCandidate.VoteSmartCandidateId,
                CandidateFirstName = dbCandidate.FirstName,
                CandidateMiddleName = dbCandidate.MiddleName,
                CandidateLastName = dbCandidate.LastName,
                CandidateSuffix = dbCandidate.Suffix,

                // get candidate office
                CandidateOfficeId = dbElectionCandidate.OfficeId,
                VoteSmartCandidateOfficeId = dbOffice.VoteSmartOfficeId,
                CandidateOfficeName = dbOffice.OfficeName,
                CandidateOfficeTerm = dbOffice.Term,

                // get runningmate name
                RunningMateId = dbRunningMate.ElectionCandidateId,
                VoteSmartRunningMateId = dbRunningMate.VoteSmartCandidateId,
                RunningMateFirstName = dbRunningMate.FirstName,
                RunningMateMiddleName = dbRunningMate.MiddleName,
                RunningMateLastName = dbRunningMate.LastName,
                RunningMateSuffix = dbRunningMate.Suffix,

                // get runningmage office
                RunningMateOfficeId = dbElectionCandidate.OfficeId,
                VoteSmartRunningMateOfficeId = dbOffice.VoteSmartOfficeId,
                RunningMateOfficeName = dbOffice.OfficeName,
                RunningMateOfficeTerm = dbOffice.Term
            };
        }



        public Models.ElectionVotingDateOfficeCandidate GetCandidateSummaryForCurrentElectionDate(int candidateLookUpId, int dateId)
        {
            return GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, dateId);
        }



        public Models.ElectionVotingDateOfficeCandidate GetRunningMateSummaryForCurrentElectionDate(int candidateLookUpId, int dateId)
        {
            // is candidate == runningmate??
            Models.ElectionVotingDateOfficeCandidate dbRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, dateId);
            if (dbRunningMate != null && dbRunningMate.RunningMateId.ToString() != "" && dbRunningMate.RunningMateId != 0)
            {// candidateLookup is the RunningMate (return both Candidate & RunningMate Info)
                return dbRunningMate;
            }
            else
            {// candidateLookup is the Candidate
                return new Models.ElectionVotingDateOfficeCandidate();
            }
        }




        public Models.ElectionVotingDateOfficeCandidate GetCandidateSummaryForCurrentElectionDateFromDatabase(int candidateLookUpId, int dateId)
        {
            List<Models.ElectionVotingDateOfficeCandidate> dbCandidate = new List<Models.ElectionVotingDateOfficeCandidate>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidate = db.ElectionVotingDateOfficeCandidates.Where(x => x.CandidateId == candidateLookUpId).Where(x => x.ElectionVotingDateId == dateId).ToList();
            }

            if (dbCandidate == null || dbCandidate.Count == 0)
                return new Models.ElectionVotingDateOfficeCandidate();

            return dbCandidate[0];
        }



        public Models.ElectionVotingDateOfficeCandidate GetRunningMateSummaryForCurrentElectionDateFromDatabase(int candidateLookUpId, int dateId)
        {
            List<Models.ElectionVotingDateOfficeCandidate> dbRunningMate = new List<Models.ElectionVotingDateOfficeCandidate>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbRunningMate = db.ElectionVotingDateOfficeCandidates.Where(x => x.RunningMateId == candidateLookUpId).Where(x => x.ElectionVotingDateId == dateId).ToList();
            }

            if (dbRunningMate == null || dbRunningMate.Count == 0)
                return new Models.ElectionVotingDateOfficeCandidate();

            return dbRunningMate[0];
        }





        public List<Models.ElectionOffice> GetCompleteListOfPossibleOfficeNamesFromDatabase()
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                return db.ElectionOffices.ToList();
            }
        }


        public List<Models.ElectionCandidate> GetCompleteListOfPossibleCandidateNamesFromDatabase()
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                return db.ElectionCandidates.ToList();
            }
        }



        public Models.ElectionCandidate GetCandidateNameForCandidateIdFromDatabase(int candidateId)
        {
            List<Models.ElectionCandidate> dbCandidate = new List<Models.ElectionCandidate>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbCandidate = db.ElectionCandidates.Where(x => x.ElectionCandidateId == candidateId).ToList();
            }

            if (dbCandidate == null || dbCandidate.Count == 0)
                return new Models.ElectionCandidate();

            return dbCandidate[0];
        }



        public List<CandidateSummary> GetOfficeNamesForAllDates(List<CandidateSummary> candidates, List<Models.ElectionOffice> officeList)
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



        public List<CandidateSummary> GetCandidateNamesForAllDates(List<CandidateSummary> candidates, List<Models.ElectionCandidate> candidateList)
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



        public List<CandidateSummary> GetRunningMateNamesForAllDates(List<CandidateSummary> candidates, List<Models.ElectionCandidate> candidateList)
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


        // Election Date Drop Down List

        private ElectionDateDropDownList GetActiveElectionDatesForDropDownList()
        {
            return new ElectionDateDropDownList()
            {
                Date = GetActiveElectionDateListItems()
            };
        }



        private IEnumerable<SelectListItem> GetActiveElectionDateListItems()
        {
            List<Models.ElectionVotingDate> dbDates = GetActiveElectionDatesFromDatabase();
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



        /// <summary>
        /// get a list of active election dates from database
        /// </summary>
        /// <returns></returns>
        public List<Models.ElectionVotingDate> GetActiveElectionDatesFromDatabase()
        {
            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                return db.ElectionVotingDates.Where(x => x.Active == true).OrderBy(x => x.Date).ToList();
            }
        }



        // *********************
        // Office Drop Down List
        // *********************



        public OfficeDropDownList GetOfficesForDropDownList(int dateId)
        {
            return new OfficeDropDownList()
            {
                OfficeNames = GetOfficeListItems(dateId)
            };
        }
        


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



        private Models.ElectionOffice GetOfficeInformationForOfficeId(int officeId)
        {
            List<Models.ElectionOffice> dbOffice = new List<ElectionOffice>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbOffice = db.ElectionOffices.Where(x => x.ElectionOfficeId == officeId).ToList();
            }

            if (dbOffice == null || dbOffice.Count == 0)
                return new Models.ElectionOffice();

            return dbOffice[0];
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



        public string GetPartyNameForPartyIdFromDatabase(string partyId)
        {
            List<Models.ElectionParty> dbParty = new List<Models.ElectionParty>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbParty = db.ElectionParties.Where(x => x.PartyId == partyId).ToList();
            }

            if (dbParty == null || dbParty.Count == 0)
                return "";

            return dbParty[0].PartyName;
        }



        public string GetOfficeHolderyNameForOfficeHolderIdFromDatabase(string officeHolderId)
        {
            List<Models.OfficeHolder> dbOfficeHolder = new List<Models.OfficeHolder>();

            using (OhioVoterDbContext db = new OhioVoterDbContext())
            {
                dbOfficeHolder = db.OfficeHolders.Where(x => x.OfficeHolderId == officeHolderId).ToList();
            }

            if (dbOfficeHolder == null || dbOfficeHolder.Count == 0)
                return "";

            return dbOfficeHolder[0].OfficeHolderName;
        }






        // *********************
        // Display Partial Views
        // *********************



        [ChildActionOnly]
        public ActionResult DisplayCandidateDisplayView(CandidateViewModel model)
        {
            return model.Candidate.CandidateId <= 0 ? 
                    PartialView("_CandidateLookUp", model) :
                    PartialView("_CandidateSummary", model);
        }
        


        [ChildActionOnly]
        public ActionResult DisplayCandidateOfficeView(CandidateViewModel model)
        {
            return model.Candidate.CandidateId > 0 ?
                    PartialView("_Office") :
                    PartialView("_Office");
        }




        // ***********************************************************
        // ***********************************************************
        // ***********************************************************



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



    }
}