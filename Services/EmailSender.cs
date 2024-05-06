using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace FantasyChas_Backend.Services
{
    // This class is responsible for sending emails.
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        // Constructor for EmailSender class.
        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        // Method to send email asynchronously.
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                // Retrieving email and password from environment variables.
                var email = Environment.GetEnvironmentVariable("EMAIL");
                var password = Environment.GetEnvironmentVariable("PASSWORD");
                // Executing email sending.
                await Execute(email, password, subject, message, toEmail);
            }
            catch (Exception ex)
            {
                // Logging any error that occurred during email sending.
                _logger.LogError($"An error occurred while sending email: {ex.Message}");
                throw; // Re-throwing the exception to propagate it up the call stack.
            }
        }

        // Method to execute email sending asynchronously.
        public async Task Execute(string email, string password, string subject, string message, string toEmail)
        {
            try
            {
                // Creating a MimeMessage for the email.
                var emailMessage = new MimeMessage();
                // Setting the sender's email address and display name for the email.
                emailMessage.From.Add(new MailboxAddress("Grupp 1", email));
                // Adding the recipient's email address to the email.
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                // Setting the subject of the email.
                emailMessage.Subject = subject;


                // Creating a BodyBuilder to build the body of the email.
                var builder = new BodyBuilder();
                // Setting the plain text body of the email.
                builder.TextBody = message;
                // Setting the HTML body of the email.
                builder.HtmlBody = message;
                // Combining the plain text and HTML bodies into the email message.
                emailMessage.Body = builder.ToMessageBody();


                // Using SmtpClient to connect to the SMTP server and send the email.
                using (var client = new SmtpClient())
                {
                    // Connecting to the SMTP server (gmail) asynchronously.
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    // Authenticating the sender's email address and password with the SMTP server.
                    await client.AuthenticateAsync(email, password);
                    // Sending the email asynchronously.
                    await client.SendAsync(emailMessage);
                    // Disconnecting from the SMTP server after sending the email.
                    await client.DisconnectAsync(true);
                }

                // Logging successful queuing of email.
                _logger.LogInformation($"Email to {toEmail} queued successfully!");
            }
            catch (Exception ex)
            {
                // Logging any error that occurred during email sending.
                _logger.LogError($"An error occurred while executing email sending: {ex.Message}");
                throw; // Re-throwing the exception to propagate it up the call stack.
            }
        }
    }
}
