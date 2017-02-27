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



        private void UpdateSessionWithNewControllerNameForSideBar(string controllerName)
        {
            SessionExtensions session = new SessionExtensions();
            session.UpdateVoterLocationWithNewControllerName(controllerName);
        }

    }





}