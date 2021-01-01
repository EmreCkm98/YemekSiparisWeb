using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
//using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Services
{
    public class EmailSender : IEmailSender//nugetten sendgrid paketini yukledik.
    {
        public EmailOptions Options { get; set; }

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;//dependency injection ile email optiontaki propa api keyi vericez.
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            
            return Execute(Options.SendGridKey, subject, message, email);
        }

        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("emreckmk98@gmail.com", "Yemek Restaurant"),
                Subject=subject,
                PlainTextContent=message,
                HtmlContent=message
            };
            msg.AddTo(new EmailAddress(email));//
            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }
    }
}
