using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.Services
{
    public class Email
    {
        private static string _twcSMTP_Host = "mail.twc.com";
        private string _amazonSMTP_Host;
        private string _amazonSMTP_UserName;
        private string _amazonSMTP_Password;
        private int _amazonSMTP_Port;

        public Email()
        {
            _amazonSMTP_Host = getAmazonHostFromDatabase("OhioVoter.org");
            _amazonSMTP_UserName = GetAmazonUserNameFromDatabase("OhioVoter.org");
            _amazonSMTP_Password = GetAmazonPasswordFromDatabase(_amazonSMTP_UserName);
            _amazonSMTP_Port = 587;
        }


        public string getAmazonHostFromDatabase(string website)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                EmailServer dtoEmailService = context.EmailServers.FirstOrDefault(x => x.Website == website);

                if (dtoEmailService == null)
                {
                    return string.Empty;
                }
                else
                {
                    return dtoEmailService.SmtpServerName;
                }
            }
        }


        public string GetAmazonUserNameFromDatabase(string website)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                EmailServer dtoEmailService = context.EmailServers.FirstOrDefault(x => x.Website == website);

                if (dtoEmailService == null)
                {
                    return string.Empty;
                }
                else
                {
                    return dtoEmailService.SmtpUserName;
                }
            }
        }


        public string GetAmazonPasswordFromDatabase(string userName)
        {
            using (OhioVoterDbContext context = new OhioVoterDbContext())
            {
                EmailServer dtoEmailService = context.EmailServers.FirstOrDefault(x => x.SmtpUserName == userName);

                if (dtoEmailService == null)
                {
                    return string.Empty;
                }
                else
                {
                    return dtoEmailService.SmtpPassword;
                }
            }
        }


        public bool SendEmail(string address, string subject, string body)
        {
            // make sure email address supplied
            if (string.IsNullOrWhiteSpace(address)) { return false; }

            // get email message
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            email.To.Add(address);
            //email.To.Add("OhioVoter.org@gmail.com");
            email.From = new System.Net.Mail.MailAddress("OhioVoter.org@yahoo.com");
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = false;

            try
            {
                
                // *********************************
                // setup amazon email transfer for website
                System.Net.Mail.SmtpClient smtpWebsiteClient = new System.Net.Mail.SmtpClient()
                {
                    Host = _amazonSMTP_Host,
                    Credentials = new System.Net.NetworkCredential(_amazonSMTP_UserName, _amazonSMTP_Password),
                    Port = _amazonSMTP_Port,
                    EnableSsl = true
                };

                // Send Message
                smtpWebsiteClient.Send(email);
                // *********************************
                


                /*
                // *********************************
                // setup email transfer for test
                System.Net.Mail.SmtpClient smtpTestClient = new System.Net.Mail.SmtpClient()
                {
                    Host = "mail.twc.com", // Time Warner Cable
                };
                // Send Message
                smtpTestClient.Send(email);
                // *********************************
                */

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}