using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.Services
{
    public class Email
    {

        public Email()
        {

        }


        public bool SendEmail(string address, string subject, string body)
        {
            // make sure email address supplied
            if (string.IsNullOrWhiteSpace(address)) { return false; }

            // get email message
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            email.To.Add(address);
            email.From = new System.Net.Mail.MailAddress("OhioVoter.org@yahoo.com");
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = false;

            // setup email transfer
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient()
            {
                //Host = "smtp.mail.yahoo.com", // Yahoo email
                Host = "mail.twc.com", // Time Warner Cable
                //Host = "smtp.fuse.net",  // Cincinnati Bell
                //Credentials = new System.Net.NetworkCredential("OhioVoter.org@yahoo.com", "P@$$word!007"),
                //Port = 25, // SSL = 465, TLS = 25, 587
                //EnableSsl = true
            };

            try
            {
                // Send Message
                smtpClient.Send(email);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}