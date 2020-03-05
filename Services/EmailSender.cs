using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorStripe.Services
{
    // This class is used by the application to send email for account confirmation and password
    // reset. For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient("smtp.mailgun.org", 587)
            {
                UseDefaultCredentials = false,

                //add credentials
                Credentials = new NetworkCredential("postmaster@email.email.com", "xxxx"),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                //from address
                From = new MailAddress("email@email.com"),
                Body = message,
                IsBodyHtml = true,
                Subject = subject
            };
            mailMessage.To.Add(email);

            client.Send(mailMessage);

            return Task.CompletedTask;
        }
    }
}