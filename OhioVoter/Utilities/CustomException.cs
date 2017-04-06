using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.Utilities
{
    public class CustomException : Exception
    {
        public CustomException(string message)
        {

        }


        private static void TestThrow()
        {
            CustomException ex =
                new CustomException("Custom exception in TestThrow()");

            throw ex;
        }


    }
}