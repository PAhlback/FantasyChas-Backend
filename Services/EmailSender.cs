using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace FantasyChas_Backend.Services
{

    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var email = Environment.GetEnvironmentVariable("EMAIL");
                var password = Environment.GetEnvironmentVariable("PASSWORD");

                await Execute(email, password, subject, message, toEmail);
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occurred while sending email: {ex.Message}");
                throw;
            }
        }

        public async Task Execute(string email, string password, string subject, string message, string toEmail)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Group 1", email));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = subject;


                var builder = new BodyBuilder();
                builder.TextBody = message;
                builder.HtmlBody = message;
                emailMessage.Body = builder.ToMessageBody();


                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync(email, password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email to {toEmail} queued successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while executing email sending: {ex.Message}");
                throw;
            }
        }
    }
}
