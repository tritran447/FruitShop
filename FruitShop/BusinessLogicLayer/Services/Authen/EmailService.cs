using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Authen
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var settings = _config.GetSection("EmailSettings");
            var smtp = new SmtpClient(settings["SmtpServer"])
            {
                Port = int.Parse(settings["Port"]!),
                Credentials = new NetworkCredential(settings["Username"], settings["Password"]),
                EnableSsl = bool.Parse(settings["EnableSsl"]!)
            };

            var message = new MailMessage
            {
                From = new MailAddress(settings["SenderEmail"], settings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            message.To.Add(toEmail);

            await smtp.SendMailAsync(message);
        }
    }
}
