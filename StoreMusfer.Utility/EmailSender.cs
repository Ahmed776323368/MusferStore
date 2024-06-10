using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.Utility
{
    public class EmailSender : IEmailSender
    {
        public string SendGridSecret { get; set; }
        public EmailSender(IConfiguration config)
        {
            SendGridSecret = config.GetValue<string>("SendGrid:SecretKey");
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridSecret);

            // i=new EmailAddress()
            var from = new EmailAddress("ahmedfathimusfer@gmail.com", "Musfer Store");
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            return client.SendEmailAsync(message);
        }
    }
}
