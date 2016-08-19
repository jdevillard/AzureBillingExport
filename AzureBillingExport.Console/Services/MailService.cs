using System;
using System.Configuration;
using System.IO;
using System.Linq;
using SendGrid.Helpers.Mail;

namespace AzureBillingExport.Console.Services
{
    internal static class MailService
    {
        public static void SendMail(MemoryStream stream)
        {
            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var sender = ConfigurationManager.AppSettings["BillingEmailSender"];
            var recipient = System.Configuration.ConfigurationManager.AppSettings["BillingEmails"].Split(new[] { '|' });

            dynamic sg = new SendGrid.SendGridAPIClient(apiKey, "https://api.sendgrid.com");
            
            var from = new Email(sender);
            var subject = "Azure Billing Export";
            var content = new Content("text/plain", "Please found attached the export of your Azure Consumption");

            var to = new Email(recipient.First());
            var mail = new Mail(@from, subject, to, content);

            foreach (var emailString in recipient.Skip(1))
            {
                var email = new Email(emailString);
                mail.Personalization[0].AddTo(email);
            }
            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment
            {
                Content = Convert.ToBase64String(stream.ToArray()),
                Type = "application/xml",
                Filename = "billing.xlsx",
                Disposition = "attachment",
                ContentId = "Billing Sheet"
            };
            mail.AddAttachment(attachment);

            var ret = mail.Get();

            var requestBody = ret;
            dynamic response = sg.client.mail.send.post(requestBody: requestBody);
        }
    }
}