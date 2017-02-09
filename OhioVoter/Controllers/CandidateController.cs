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

            string controllerName = "Candidate";
            UpdateSessionWithNewControllerNameForSideBar(controllerName);

            CandidateViewModel viewModel = new CandidateViewModel()
            {
                ControllerName = controllerName,
                Candidate = GetCandidateSummaryViewModel((int)candidateId),
                ElectionDate = GetFirstActiveElectionDate(),
                CandidateDropDownList = GetCandidateDropDownListViewModel(),
                // Office
            };

            return View(viewModel);
        }



        private CandidateSummary GetCandidateSummaryViewModel(int candidateId)
        {
            if (candidateId != 0)
            {
                ElectionVotingDate date = GetFirstActiveElectionDate();
                CandidateSummary candidate = GetCandidateSummaryInformationForSelectedCandidateId(candidateId, date);
                candidate = GetCandidateBiographyFromVoteSmart(candidate);
                return candidate;
            }
            else
            {
                return new CandidateSummary();
            }
        }



        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
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
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { candidateId = 0 });

            int candidateId = model.CandidateDropDownList.SelectedCandidateId;

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
        


        // ******************************************************************************



        /// <summary>
        /// look up candidate information for selected candidate and current election based on supplied candidateId
        /// </summary>
        /// <param name="candidateLookUpId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public CandidateSummary GetCandidateSummaryInformationForSelectedCandidateId(int candidateLookUpId, ElectionVotingDate date)
        {
            // get candidateLookUp information
            Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp = GetCandidateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, date.ElectionVotingDateId);

            // add supplied information to candidate object
            CandidateSummary candidate = new CandidateSummary()
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



        // *************************************************************************



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



        // ****************************************************
        


        public CandidateSummary GetCandidateInformationForRunningMateId(int candidateLookUpId, int dateId, Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp, CandidateSummary candidate)
        {
            Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate = GetRunningMateSummaryForCurrentElectionDateFromDatabase(candidateLookUpId, dateId);

            candidate = GetCandidateElectionSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetCandidateNameSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetCandidateOfficeSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetRunningMateNameSummaryForRunningMateId(candidate, dbElectionRunningMate);
            candidate = GetRunningMateOfficeSummaryForRunningMateId(candidate, dbCandidateLookUp, dbElectionRunningMate);

            return candidate;
        }



        public CandidateSummary GetCandidateInformationForCandidateId(int dateId, Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp, CandidateSummary candidate)
        {
            Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbCandidateLookUp.RunningMateId, dateId);

            candidate = GetCandidateElectionSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetCandidateNameSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetCandidateOfficeSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetRunningMateNameSummaryForCandidateId(candidate, dbCandidateLookUp);
            candidate = GetRunningMateOfficeSummaryForCandidateId(dateId, candidate, dbElectionRunningMate);

            return candidate;
        }



        // ****************************************************************************



        public CandidateSummary GetCandidateElectionSummaryForRunningMateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            candidate.OfficeHolderId = dbElectionRunningMate.OfficeHolderId;
            candidate.OfficeHolderName = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbElectionRunningMate.OfficeHolderId);
            candidate.PartyId = dbElectionRunningMate.PartyId;
            candidate.PartyName = GetPartyNameForPartyIdFromDatabase(dbElectionRunningMate.PartyId.ToString());
            candidate.CertifiedCandidateId = dbElectionRunningMate.CertifiedCandidateId;

            return candidate;
        }



        public CandidateSummary GetCandidateNameSummaryForRunningMateId(CandidateSummary candidate,  Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
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



        public CandidateSummary GetCandidateOfficeSummaryForRunningMateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbElectionRunningMate.OfficeId);

            candidate.CandidateOfficeId = dbElectionRunningMate.OfficeId;
            candidate.VoteSmartCandidateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.CandidateOfficeName = dbOffice.OfficeName;
            candidate.CandidateOfficeTerm = dbOffice.Term;

            return candidate;
        }



        public CandidateSummary GetRunningMateNameSummaryForRunningMateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
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



        public CandidateSummary GetRunningMateOfficeSummaryForRunningMateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbCandidateLookUp, Models.ElectionVotingDateOfficeCandidate dbElectionRunningMate)
        {
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbCandidateLookUp.OfficeId);

            candidate.RunningMateOfficeId = dbCandidateLookUp.OfficeId;
            candidate.VoteSmartRunningMateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.RunningMateOfficeName = dbOffice.OfficeName;
            candidate.RunningMateOfficeTerm = dbOffice.Term;

            return candidate;
        }



        // ******************************************************************************



        public CandidateSummary GetCandidateElectionSummaryForCandidateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            candidate.OfficeHolderId = dbElectionCandidate.OfficeHolderId;
            candidate.OfficeHolderName = GetOfficeHolderyNameForOfficeHolderIdFromDatabase(dbElectionCandidate.OfficeHolderId);
            candidate.PartyId = dbElectionCandidate.PartyId;
            candidate.PartyName = GetPartyNameForPartyIdFromDatabase(dbElectionCandidate.PartyId.ToString());
            candidate.CertifiedCandidateId = dbElectionCandidate.CertifiedCandidateId;

            return candidate;
        }



        public CandidateSummary GetCandidateNameSummaryForCandidateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
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



        public CandidateSummary GetCandidateOfficeSummaryForCandidateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbElectionCandidate.OfficeId);

            candidate.CandidateOfficeId = dbElectionCandidate.OfficeId;
            candidate.VoteSmartCandidateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.CandidateOfficeName = dbOffice.OfficeName;
            candidate.CandidateOfficeTerm = dbOffice.Term;

            return candidate;
        }



        public CandidateSummary GetRunningMateNameSummaryForCandidateId(CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
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



        public CandidateSummary GetRunningMateOfficeSummaryForCandidateId(int dateId, CandidateSummary candidate, Models.ElectionVotingDateOfficeCandidate dbElectionCandidate)
        {
            Models.ElectionVotingDateOfficeCandidate dbRunningMateLookUp = GetCandidateSummaryForCurrentElectionDateFromDatabase(dbElectionCandidate.CandidateId, dateId);
            Models.ElectionOffice dbOffice = GetOfficeInformationForOfficeId(dbRunningMateLookUp.OfficeId);

            candidate.RunningMateOfficeId = dbRunningMateLookUp.OfficeId;
            candidate.VoteSmartRunningMateOfficeId = dbOffice.VoteSmartOfficeId;
            candidate.RunningMateOfficeName = dbOffice.OfficeName;
            candidate.RunningMateOfficeTerm = dbOffice.Term;

            return candidate;
        }



        // ******************************************************************************


        
        public CandidateSummary GetCandidateBiographyFromVoteSmart(CandidateSummary candidate)
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



        public CandidateSummary UpdateCandidateSummaryWithCandidateBiographyFromVoteSmart(CandidateSummary candidate, ViewModels.VoteSmart.CandidateBio candidateBio)
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



        public CandidateSummary UpdateCandidateSummaryWithRunningMateBiographyFromVoteSmart(CandidateSummary candidate, ViewModels.VoteSmart.CandidateBio runningMateBio)
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
                stringList.Add(stringWithLineBreaks);

            return stringList;
        }
        



        // ********************************************************************************



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
        public ActionResult DisplayCandidateInformation(CandidateViewModel model)
        {
            if (model.Candidate.CandidateId > 0)
                return PartialView("_CandidateDisplay", model);

            return PartialView("_CandidateLookUp", model);
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