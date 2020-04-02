using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using log4net;

namespace Digbyswift.Interview.UI.Email
{
    public class EmailProvider
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(EmailProvider));

        public async Task<EmailStatus> EnqueueAsync(string recipient, string subject, string body, bool isHtml = false, string from = null, string bccRecipient = null)
        {
            if (recipient == null)
                throw new ArgumentNullException(nameof(recipient));

            if (String.IsNullOrWhiteSpace(recipient))
                throw new ArgumentException("The recipient cannot be empty", nameof(recipient));

            List<string> recipients;

            if (recipient.Contains(","))
            {
                recipients = recipient.Split(',').Select(x => x.Trim().ToLower()).Where(x => !String.IsNullOrWhiteSpace(x) && x.IsEmail()).ToList();
            }
            else if(recipient.IsEmail())
            {
                recipients = new List<string> {recipient};
            }
            else
            {
                return EmailStatus.Aborted;
            }

            return await EnqueueAsync(recipients, subject, body, isHtml, from, bccRecipient);
        }
        
        public async Task<EmailStatus> EnqueueAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = false, string from = null, string bccRecipient = null)
        {
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            if (!recipients.Any())
                throw new ArgumentException("At least one recipient must be provided", nameof(recipients));

            try
            {
                using (var smtp = new SmtpClient())
                using (var mail = new MailMessage())
                {
                    foreach (var email in recipients)
                    {
                        mail.To.Add(email.Trim());
                    }

                    if (from != null && from.IsEmail())
                    {
                        mail.From = new MailAddress(from);
                    }

                    if (bccRecipient != null && bccRecipient.IsEmail())
                    {
                        mail.Bcc.Add(bccRecipient);
                    }

                    mail.IsBodyHtml = isHtml;
                    mail.Subject = subject;
                    mail.Body = body;

                    await smtp.SendMailAsync(mail);
                }

                return EmailStatus.Sent;
            }
            catch (Exception ex)
            {
                _log.Error(ex);

                return EmailStatus.Failed;
            }
        }

    }
}