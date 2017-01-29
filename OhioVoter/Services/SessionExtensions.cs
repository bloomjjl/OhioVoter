﻿using OhioVoter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.Services
{
    public class SessionExtensions
    {
        private const string VoterLocationStatus = "sessionVoterLocationStatus";
        private const string VoterLocationMessage = "sessionVoterLocationMessage";
        private const string VoterLocationStreet = "sessionVoterLocationStreetAddress";
        private const string VoterLocationCity = "sessionVoterLocationCity";
        private const string VoterLocationState = "sessionVoterLocationState";
        private const string VoterLocationZipCode = "sessionVoterLocationZipCode";
        private const string VoterLocationFullAddress = "sessionVoterLocationFullAddress";

        private const string PollingLocationStatus = "sessionPollingLocationStatus";
        private const string PollingLocationName = "sessionPollingLocationName";
        private const string PollingLocationStreet = "sessionPollingLocationStreetAddress";
        private const string PollingLocationCity = "sessionPollingLocationCity";
        private const string PollingLocationState = "sessionPollingLocationState";
        private const string PollingLocationZipCode = "sessionPollingLocationZipCode";
        private const string PollingLocationFullAddress = "sessionPollingLocationFullAddress";
        private const string PollingLocationMap = "sessionPollingLocationMap";

        private const string CountyLocationStatus = "sessionCountyLocationStatus";
        private const string CountyLocationName = "sessionCountyLocationName";
        private const string CountyLocationStreet = "sessionCountyLocationStreetAddress";
        private const string CountyLocationCity = "sessionCountyLocationCity";
        private const string CountyLocationState = "sessionCountyLocationState";
        private const string CountyLocationZipCode = "sessionCountyLocationZipCode";
        private const string CountyLocationFullAddress = "sessionCountyLocationFullAddress";
        private const string CountyLocationPhone = "sessionCountyLocationPhone";
        private const string CountyLocationEmail = "sessionCountyLocationEmail";
        private const string CountyLocationWebsite = "sessionCountyLocationWebsite";


        /// <summary>
        /// update session values with new voter location
        /// </summary>
        /// <param name="voterLocation"></param>
        public void UpdateVoterLocationInSession(Location voterLocation)
        {
            System.Web.HttpContext.Current.Session[VoterLocationStatus] = voterLocation.Status;
            System.Web.HttpContext.Current.Session[VoterLocationMessage] = voterLocation.Message;
            System.Web.HttpContext.Current.Session[VoterLocationStreet] = CapitalizeAllLetters(voterLocation.StreetAddress);
            System.Web.HttpContext.Current.Session[VoterLocationCity] = CapitalizeAllLetters(voterLocation.City);
            System.Web.HttpContext.Current.Session[VoterLocationState] = CapitalizeAllLetters(voterLocation.StateAbbreviation);
            System.Web.HttpContext.Current.Session[VoterLocationZipCode] = voterLocation.ZipCode;
            System.Web.HttpContext.Current.Session[VoterLocationFullAddress] = voterLocation.FullAddress;
        }



        /// <summary>
        /// update session values with new polling location
        /// </summary>
        /// <param name="pollingLocation"></param>
        public void UpdatePollingLocationInSession(Location pollingLocation)
        {
            System.Web.HttpContext.Current.Session[PollingLocationStatus] = pollingLocation.Status;
            System.Web.HttpContext.Current.Session[PollingLocationName] = CapitalizeAllLetters(pollingLocation.LocationName);
            System.Web.HttpContext.Current.Session[PollingLocationStreet] = CapitalizeAllLetters(pollingLocation.StreetAddress);
            System.Web.HttpContext.Current.Session[PollingLocationCity] = CapitalizeAllLetters(pollingLocation.City);
            System.Web.HttpContext.Current.Session[PollingLocationState] = CapitalizeAllLetters(pollingLocation.StateAbbreviation);
            System.Web.HttpContext.Current.Session[PollingLocationZipCode] = pollingLocation.ZipCode;
            System.Web.HttpContext.Current.Session[PollingLocationFullAddress] = pollingLocation.FullAddress;
            System.Web.HttpContext.Current.Session[PollingLocationMap] = pollingLocation.GoogleLocationMapAPI;
        }



        /// <summary>
        /// update session values with new county location
        /// </summary>
        /// <param name="countyLocation"></param>
        public void UpdateCountyLocationInSession(Location countyLocation)
        {
            System.Web.HttpContext.Current.Session[CountyLocationStatus] = countyLocation.Status;
            System.Web.HttpContext.Current.Session[CountyLocationName] = CapitalizeAllLetters(countyLocation.LocationName);
            System.Web.HttpContext.Current.Session[CountyLocationStreet] = CapitalizeAllLetters(countyLocation.StreetAddress);
            System.Web.HttpContext.Current.Session[CountyLocationCity] = CapitalizeAllLetters(countyLocation.City);
            System.Web.HttpContext.Current.Session[CountyLocationState] = CapitalizeAllLetters(countyLocation.StateAbbreviation);
            System.Web.HttpContext.Current.Session[CountyLocationZipCode] = countyLocation.ZipCode;
            System.Web.HttpContext.Current.Session[CountyLocationFullAddress] = countyLocation.FullAddress;
            System.Web.HttpContext.Current.Session[CountyLocationPhone] = countyLocation.Phone;
            System.Web.HttpContext.Current.Session[CountyLocationEmail] = countyLocation.Email;
            System.Web.HttpContext.Current.Session[CountyLocationWebsite] = countyLocation.Website;
        }



        /// <summary>
        /// capitalize letters in string
        /// </summary>
        /// <param name="oldString"></param>
        /// <returns></returns>
        public String CapitalizeAllLetters(String oldString)
        {
            if (oldString != null)
                return oldString.ToUpper();

            return oldString;
        }



        /// <summary>
        /// get the voter location information from the session
        /// </summary>
        /// <returns></returns>
        public Location GetVoterLocationFromSession()
        {
            // make sure session exists
            if (System.Web.HttpContext.Current.Session != null)
            {
                UpdateDefaultSessionItemsForVoterLocation();

            }
            else if (System.Web.HttpContext.Current.Session[VoterLocationStatus] == null ||
                System.Web.HttpContext.Current.Session[VoterLocationStatus] as string == "")
            {
                UpdateDefaultSessionItemsForVoterLocation();
            }

            Location voterLocation = new Location()
            {
                Status = System.Web.HttpContext.Current.Session[VoterLocationStatus] as String,
                Message = System.Web.HttpContext.Current.Session[VoterLocationMessage] as String,
                StreetAddress = System.Web.HttpContext.Current.Session[VoterLocationStreet] as String,
                City = System.Web.HttpContext.Current.Session[VoterLocationCity] as String,
                StateAbbreviation = System.Web.HttpContext.Current.Session[VoterLocationState] as String,
                ZipCode = System.Web.HttpContext.Current.Session[VoterLocationZipCode] as String
            };

            return voterLocation;
        }



        /// <summary>
        /// get the polling location from the session
        /// </summary>
        /// <returns></returns>
        public Location GetPollingLocationFromSession()
        {
            // make sure session exists
            if (System.Web.HttpContext.Current.Session[PollingLocationStatus] == null ||
                System.Web.HttpContext.Current.Session[PollingLocationStatus] as string == "")
            {
                UpdateDefaultSessionItemsForPollingLocation();
            }

            Location pollingLocation = new Location()
            {
                Status = System.Web.HttpContext.Current.Session[PollingLocationStatus] as String,
                LocationName = System.Web.HttpContext.Current.Session[PollingLocationName] as String,
                StreetAddress = System.Web.HttpContext.Current.Session[PollingLocationStreet] as String,
                City = System.Web.HttpContext.Current.Session[PollingLocationCity] as String,
                StateAbbreviation = System.Web.HttpContext.Current.Session[PollingLocationState] as String,
                ZipCode = System.Web.HttpContext.Current.Session[PollingLocationZipCode] as String,
                GoogleLocationMapAPI = System.Web.HttpContext.Current.Session[PollingLocationMap] as String
            };

            return pollingLocation;
        }



        /// <summary>
        /// get the county information from the session
        /// </summary>
        /// <returns></returns>
        public Location GetCountyLocationFromSession()
        {
            // make sure session exists
            if (System.Web.HttpContext.Current.Session[CountyLocationStatus] == null ||
                System.Web.HttpContext.Current.Session[CountyLocationStatus] as string == "")
            {
                UpdateDefaultSessionItemsForCountyLocation();
            }

            Location countyLocation = new Location()
            {
                Status = System.Web.HttpContext.Current.Session[CountyLocationStatus] as String,
                LocationName = System.Web.HttpContext.Current.Session[CountyLocationName] as String,
                StreetAddress = System.Web.HttpContext.Current.Session[CountyLocationStreet] as String,
                City = System.Web.HttpContext.Current.Session[CountyLocationCity] as String,
                StateAbbreviation = System.Web.HttpContext.Current.Session[CountyLocationState] as String,
                ZipCode = System.Web.HttpContext.Current.Session[CountyLocationZipCode] as String,
                Phone = System.Web.HttpContext.Current.Session[CountyLocationPhone] as String,
                Email = System.Web.HttpContext.Current.Session[CountyLocationEmail] as String,
                Website = System.Web.HttpContext.Current.Session[CountyLocationWebsite] as String
            };

            return countyLocation;
        }



        /// <summary>
        /// update the status in the session to update the voter location
        /// </summary>
        public void ChangeVoterLocationStatusToUpdateVoterLocationForm()
        {
            System.Web.HttpContext.Current.Session[VoterLocationStatus] = "Update";
        }



        /// <summary>
        /// update the status in the session to display the voter locatino
        /// </summary>
        public void ChangeVoterLocationStatusToDisplayVoterLocation()
        {
            System.Web.HttpContext.Current.Session[VoterLocationStatus] = "Display";
        }



        /// <summary>
        /// Make sure session has valid voter location information 
        /// </summary>
        public void UpdateDefaultSessionItemsForVoterLocation()
        {
            // set the status for current session if no information available
            // make sure state is always Ohio
            if (System.Web.HttpContext.Current.Session[VoterLocationState] == null)
            {
                System.Web.HttpContext.Current.Session[VoterLocationStatus] = "Update";
                System.Web.HttpContext.Current.Session[VoterLocationState] = "";
            }

            // initiate session values if none are provided
            if (System.Web.HttpContext.Current.Session[VoterLocationStreet] == null)
            {
                System.Web.HttpContext.Current.Session[VoterLocationStatus] = "Update";
                System.Web.HttpContext.Current.Session[VoterLocationMessage] = "Street address must be a valid location in Ohio.";
                System.Web.HttpContext.Current.Session[VoterLocationStreet] = "";
            }
            else if (System.Web.HttpContext.Current.Session[VoterLocationStatus] == null)
            {
                System.Web.HttpContext.Current.Session[VoterLocationStatus] = "Update";
                System.Web.HttpContext.Current.Session[VoterLocationMessage] = "";
            }
            else if (System.Web.HttpContext.Current.Session[VoterLocationMessage] == null)
            {
                System.Web.HttpContext.Current.Session[VoterLocationMessage] = "";
            }
        }



        /// <summary>
        /// Make sure session has valid voter location information 
        /// </summary>
        public void UpdateDefaultSessionItemsForPollingLocation()
        {
            // set the status for current session if no information available
            // make sure state is always Ohio
            if (System.Web.HttpContext.Current.Session[PollingLocationState] == null)
            {
                System.Web.HttpContext.Current.Session[PollingLocationStatus] = "Update";
                System.Web.HttpContext.Current.Session[PollingLocationState] = "";
            }

            // initiate session values if none are provided
            if (System.Web.HttpContext.Current.Session[PollingLocationStreet] == null)
            {
                System.Web.HttpContext.Current.Session[PollingLocationStatus] = "Update";
                System.Web.HttpContext.Current.Session[PollingLocationStreet] = "";
            }
            else if (System.Web.HttpContext.Current.Session[VoterLocationStatus] == null)
            {
                System.Web.HttpContext.Current.Session[PollingLocationStatus] = "Update";
            }
        }



        /// <summary>
        /// Make sure session has valid voter location information 
        /// </summary>
        public void UpdateDefaultSessionItemsForCountyLocation()
        {
            // set the status for current session if no information available
            // make sure state is always Ohio
            if (System.Web.HttpContext.Current.Session[CountyLocationState] == null)
            {
                System.Web.HttpContext.Current.Session[CountyLocationStatus] = "Update";
                System.Web.HttpContext.Current.Session[CountyLocationState] = "";
            }

            // initiate session values if none are provided
            if (System.Web.HttpContext.Current.Session[CountyLocationStreet] == null)
            {
                System.Web.HttpContext.Current.Session[CountyLocationStatus] = "Update";
                System.Web.HttpContext.Current.Session[CountyLocationStreet] = "";
            }
            else if (System.Web.HttpContext.Current.Session[CountyLocationStatus] == null)
            {
                System.Web.HttpContext.Current.Session[CountyLocationStatus] = "Update";
            }
        }



        /// <summary>
        /// provide the status for the voter location
        /// </summary>
        /// <returns></returns>
        public String GetStatusForVoterAddress()
        {
            return System.Web.HttpContext.Current.Session[VoterLocationStatus] as String;
        }


    }
}