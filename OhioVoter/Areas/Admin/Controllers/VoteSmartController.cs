using OhioVoter.Models;
using OhioVoter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Areas.Admin.Controllers
{
    [Authorize]
    public class VoteSmartController : Controller
    {
        // GET: Admin/VoteSmart
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public void UpdateDatabaseWithCurrentImagePhotoUrlFromVoteSmart()
        {
            // add image url for votesmart to database
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.Candidate> candidate = context.Candidates.ToList();
                VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();

                // loop through each office
                for (int i = 0; i < candidate.Count(); i++)
                {
                    ViewModels.VoteSmart.CandidateBio votesmartCandidate = voteSmart.GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(candidate[i].VoteSmartCandidateId);
                    candidate[i].VoteSmartPhotoUrl = votesmartCandidate.Photo;
                }

                context.SaveChanges();
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public void UpdateDatabaseWithCandidateGenderFromVoteSmart()
        {
            // add gender for candidate from votesmart to database
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                List<Models.Candidate> candidate = context.Candidates.ToList();
                VoteSmartApiManagement voteSmart = new VoteSmartApiManagement();

                // loop through each office
                for (int i = 0; i < candidate.Count(); i++)
                {
                    ViewModels.VoteSmart.CandidateBio votesmartCandidate = voteSmart.GetVoteSmartMatchingCandidateFromSuppliedVoteSmartCandidateId(candidate[i].VoteSmartCandidateId);
                    string gender = votesmartCandidate.Gender;
                    if (gender == "Male")
                    {
                        candidate[i].Gender = "M";
                    }
                    else if (gender == "Female")
                    {
                        candidate[i].Gender = "F";
                    }
                    else
                    { 
                        candidate[i].Gender = string.Empty;
                    }
                }

                context.SaveChanges();
            }
        }




    }
}