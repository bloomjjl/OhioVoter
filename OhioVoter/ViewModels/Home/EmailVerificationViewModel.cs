using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Home
{
    public class EmailVerificationViewModel
    {
        public EmailVerificationViewModel() { }

        public EmailVerificationViewModel(string messageHeader, string messageBody)
        {
            MessageHeader = messageHeader;
            MessageBody = messageBody;
        }

        public string MessageBody { get; set; }
        public string MessageHeader { get; set; }
    }
}