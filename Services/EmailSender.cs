using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorStripe.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient("smtp.mailgun.org", 587);
            client.UseDefaultCredentials = false;
            //add credentials
            client.Credentials = new NetworkCredential("postmaster@email.email.com", "xxxx");
            client.EnableSsl = true;

            //MailMessage mailMessage = new MailMessage
            //{
            //    From = new MailAddress("email@email.com"),
            //    Body = message,
            //    IsBodyHtml = true
            //};
            MailMessage mailMessage = new MailMessage();
            //from address
            mailMessage.From = new MailAddress("email@email.com");
            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;

            //mailMessage.Body = message;
            //mailMessage.IsBodyHtml = true;
            client.Send(mailMessage);

            return Task.CompletedTask;
        }
    }
}