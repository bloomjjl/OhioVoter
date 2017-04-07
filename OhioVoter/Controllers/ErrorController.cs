using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhioVoter.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            // get error information
            ApplicationException errorApplication = new ApplicationException();
            string errorMessage = errorApplication.Message.ToString();
            string errorType = errorApplication.GetType().ToString();

            // send email with details
            //bool emailSent = SendErrorEmail(errorMessage, errorType);

            // clear error
            Server.ClearError();

            // statusCode = "400" path = "Error" responseMode = "Redirect" />
            // statusCode = "401" prefixLanguageFilePath = "" path = "Error" responseMode = "ExecuteURL" />
            Response.StatusCode = 200;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_GenericMessage");
            }

            return View();
        }



        public ActionResult NotFound()
        {
            // get error information
            ApplicationException errorApplication = new ApplicationException();
            string errorMessage = errorApplication.Message.ToString();
            string errorType = errorApplication.GetType().ToString();

            // send email with details
            //bool emailSent = SendErrorEmail(errorMessage, errorType);

            // clear error
            Server.ClearError();

            // redirect = "~/Error/NotFound" statusCode = "404" />
            // statusCode = "404" subStatusCode = "0" path = "Error" responseMode = "ExecuteURL" />
            Response.StatusCode = 200;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_GenericMessage");
            }

            return View("Index");
        }


        public ActionResult InternalServer()
        {
            // get error information
            ApplicationException errorApplication = new ApplicationException();
            string errorMessage = errorApplication.Message.ToString();
            string errorType = errorApplication.GetType().ToString();


            // send email with details
            //bool emailSent = SendErrorEmail(errorMessage, errorType);

            // clear error
            Server.ClearError();

            // statusCode = "500" path = "Error" responseMode = "Redirect" />
            // redirect = "~/Error/InternalServer" statusCode = "500" />
            Response.StatusCode = 200;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_GenericMessage");
            }

            return View("Index");
        }


        private bool SendErrorEmail(string errorMessage, string errorType)
        {
            string toAddress = "OhioVoter.org@yahoo.com";
            string subject = "An Error Has Occurred!";
            string body = string.Format("URL: {0}\r\n\r\nUser: {1}\r\n\r\nException Type: {2}\r\n\r\nMessage: {3}",
                Request.RawUrl,
                User.Identity.Name,
                errorType,
                errorMessage);
            Services.Email email = new Services.Email();
            return email.SendEmail(toAddress, subject, body);
        }


    }
}