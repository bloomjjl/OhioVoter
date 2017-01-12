using OhioVoter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.Services
{
    public class SessionExtensions
    {
        public void UpdateVoterLocationInSession(Location voterLocation)
        {
            System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] = voterLocation.Status;
            System.Web.HttpContext.Current.Session["sessionVoterLocationMessage"] = voterLocation.Message;
            System.Web.HttpContext.Current.Session["sessionVoterLocationStreetAddress"] = CapitalizeAllLetters(voterLocation.StreetAddress);
            System.Web.HttpContext.Current.Session["sessionVoterLocationCity"] = CapitalizeAllLetters(voterLocation.City);
            System.Web.HttpContext.Current.Session["sessionVoterLocationState"] = CapitalizeAllLetters(voterLocation.StateAbbreviation);
            System.Web.HttpContext.Current.Session["sessionVoterLocationZipCode"] = voterLocation.ZipCode;
            System.Web.HttpContext.Current.Session["sessionVoterLocationFullAddress"] = voterLocation.FullAddress;
        }



        public void UpdatePollingLocationInSession(Location pollingLocation)
        {
            System.Web.HttpContext.Current.Session["sessionPollingLocationStatus"] = pollingLocation.Status;
            System.Web.HttpContext.Current.Session["sessionPollingLocationName"] = CapitalizeAllLetters(pollingLocation.LocationName);
            System.Web.HttpContext.Current.Session["sessionPollingLocationStreetAddress"] = CapitalizeAllLetters(pollingLocation.StreetAddress);
            System.Web.HttpContext.Current.Session["sessionPollingLocationCity"] = CapitalizeAllLetters(pollingLocation.City);
            System.Web.HttpContext.Current.Session["sessionPollingLocationState"] = CapitalizeAllLetters(pollingLocation.StateAbbreviation);
            System.Web.HttpContext.Current.Session["sessionPollingLocationZipCode"] = pollingLocation.ZipCode;
            System.Web.HttpContext.Current.Session["sessionPollingLocationFullAddress"] = pollingLocation.FullAddress;
            System.Web.HttpContext.Current.Session["sessionPollingLocationMap"] = pollingLocation.GoogleLocationMapAPI;
        }



        public void UpdateCountyLocationInSession(Location countyLocation)
        {
            System.Web.HttpContext.Current.Session["sessionCountyLocationStatus"] = countyLocation.Status;
            System.Web.HttpContext.Current.Session["sessionCountyLocationName"] = CapitalizeAllLetters(countyLocation.LocationName);
            System.Web.HttpContext.Current.Session["sessionCountyLocationStreetAddress"] = CapitalizeAllLetters(countyLocation.StreetAddress);
            System.Web.HttpContext.Current.Session["sessionCountyLocationCity"] = CapitalizeAllLetters(countyLocation.City);
            System.Web.HttpContext.Current.Session["sessionCountyLocationState"] = CapitalizeAllLetters(countyLocation.StateAbbreviation);
            System.Web.HttpContext.Current.Session["sessionCountyLocationZipCode"] = countyLocation.ZipCode;
            System.Web.HttpContext.Current.Session["sessionCountyLocationFullAddress"] = countyLocation.FullAddress;
            System.Web.HttpContext.Current.Session["sessionCountyLocationPhone"] = countyLocation.Phone;
            System.Web.HttpContext.Current.Session["sessionCountyLocationEmail"] = countyLocation.Email;
            System.Web.HttpContext.Current.Session["sessionCountyLocationWebsite"] = countyLocation.Website;
        }



        public String CapitalizeAllLetters(String oldString)
        {
            if (oldString != null)
                return oldString.ToUpper();

            return oldString;
        }



        public Location GetVoterLocationFromSession()
        {
            // make sure session exists
            if (System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] == null ||
                System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] as string == "")
            {
                UpdateDefaultSessionItemsForVoterLocation();
            }

            Location voterLocation = new Location()
            {
                Status = System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] as String,
                Message = System.Web.HttpContext.Current.Session["sessionVoterLocationMessage"] as String,
                StreetAddress = System.Web.HttpContext.Current.Session["sessionVoterLocationStreetAddress"] as String,
                City = System.Web.HttpContext.Current.Session["sessionVoterLocationCity"] as String,
                StateAbbreviation = System.Web.HttpContext.Current.Session["sessionVoterLocationState"] as String,
                ZipCode = System.Web.HttpContext.Current.Session["sessionVoterLocationZipCode"] as String
            };

            return voterLocation;
        }



        public Location GetPollingLocationFromSession()
        {
            if (System.Web.HttpContext.Current.Session["sessionPollingLocationStreetAddress"] == null)
                return new Location();

            Location pollingLocation = new Location()
            {
                LocationName = System.Web.HttpContext.Current.Session["sessionPollingLocationName"] as String,
                StreetAddress = System.Web.HttpContext.Current.Session["sessionPollingLocationStreetAddress"] as String,
                City = System.Web.HttpContext.Current.Session["sessionPollingLocationCity"] as String,
                StateAbbreviation = System.Web.HttpContext.Current.Session["sessionPollingLocationState"] as String,
                ZipCode = System.Web.HttpContext.Current.Session["sessionPollingLocationZipCode"] as String,
                GoogleLocationMapAPI = System.Web.HttpContext.Current.Session["sessionPollingLocationMap"] as String
            };

            return pollingLocation;
        }



        public Location GetCountyLocationFromSession()
        {
            if (System.Web.HttpContext.Current.Session["sessionCountyLocationCity"] == null)
                return new Location();

            Location countyLocation = new Location()
            {
                Status = System.Web.HttpContext.Current.Session["sessionCountyLocationStatus"] as String,
                LocationName = System.Web.HttpContext.Current.Session["sessionCountyLocationName"] as String,
                StreetAddress = System.Web.HttpContext.Current.Session["sessionCountyLocationStreetAddress"] as String,
                City = System.Web.HttpContext.Current.Session["sessionCountyLocationCity"] as String,
                StateAbbreviation = System.Web.HttpContext.Current.Session["sessionCountyLocationState"] as String,
                ZipCode = System.Web.HttpContext.Current.Session["sessionCountyLocationZipCode"] as String,
                Phone = System.Web.HttpContext.Current.Session["sessionCountyLocationPhone"] as String,
                Email = System.Web.HttpContext.Current.Session["sessionCountyLocationEmail"] as String,
                Website = System.Web.HttpContext.Current.Session["sessionCountyLocationWebsite"] as String
            };

            return countyLocation;
        }



        public void ChangeVoterLocationStatusToUpdateVoterLocationForm()
        {
            System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] = "Update";
        }



        public void ChangeVoterLocationStatusToDisplayVoterLocation()
        {
            System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] = "Display";
        }



        public void UpdateDefaultSessionItemsForVoterLocation()
        {
            // set the status for current session if no information available
            // make sure state is always Ohio
            if (System.Web.HttpContext.Current.Session["sessionVoterLocationState"] == null)
            {
                System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] = "Update";
                System.Web.HttpContext.Current.Session["sessionVoterLocationState"] = "OH";
            }

            // initiate session values if none are provided
            if (System.Web.HttpContext.Current.Session["sessionVoterLocationStreetAddress"] == null)
            {
                System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] = "Update";
                System.Web.HttpContext.Current.Session["sessionVoterLocationMessage"] = "Street address must be provided.";
            }
            else if (System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] == null)
            {
                System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] = "Display";
                System.Web.HttpContext.Current.Session["sessionVoterLocationMessage"] = "";
            }
            else if (System.Web.HttpContext.Current.Session["sessionVoterLocationMessage"] == null)
            {
                System.Web.HttpContext.Current.Session["sessionVoterLocationMessage"] = "";
            }
        }



        public String GetStatusForVoterAddress()
        {
            return System.Web.HttpContext.Current.Session["sessionVoterLocationStatus"] as String;
        }


    }
}