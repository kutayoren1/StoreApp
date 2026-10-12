using System.Net;
using System.Net.Mail;
using StoreApp.Models;

namespace StoreApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendContactEmailAsync(ContactViewModel model)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPortStr = _configuration["EmailSettings:SmtpPort"];
            int smtpPort = int.TryParse(smtpPortStr, out var port) ? port : 587;
            var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "kutayoren78@gmail.com";
            var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? "";
            var receiverEmail = _configuration["EmailSettings:ReceiverEmail"] ?? "kutayoren78@gmail.com";
            var enableSsl = !bool.TryParse(_configuration["EmailSettings:EnableSsl"], out var ssl) || ssl;

            if (string.IsNullOrWhiteSpace(senderPassword))
            {
                _logger.LogWarning("EmailSettings:SenderPassword is empty in appsettings.json. Email simulation mode: message from {Email} ({Name}) to {ReceiverEmail}", model.Email, model.Name, receiverEmail);
                return;
            }

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "StoreApp Contact Form"),
                Subject = $"[StoreApp Contact] New Message from {model.Name}",
                Body = $@"<html>
<body style='font-family: Arial, sans-serif; color: #333;'>
    <h2>StoreApp - New Contact Form Message</h2>
    <hr style='border: 1px solid #1abc9c;'/>
    <p><strong>Username / Name:</strong> {WebUtility.HtmlEncode(model.Name)}</p>
    <p><strong>Email:</strong> <a href='mailto:{WebUtility.HtmlEncode(model.Email)}'>{WebUtility.HtmlEncode(model.Email)}</a></p>
    <p><strong>Phone:</strong> {WebUtility.HtmlEncode(model.Phone)}</p>
    <p><strong>Date:</strong> {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>
    <hr/>
    <p><strong>Message:</strong></p>
    <div style='background: #f9f9f9; padding: 15px; border-left: 4px solid #1abc9c; border-radius: 4px; white-space: pre-wrap;'>
{WebUtility.HtmlEncode(model.Message)}
    </div>
</body>
</html>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(receiverEmail);
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                mailMessage.ReplyToList.Add(new MailAddress(model.Email, model.Name));
            }

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Contact email successfully sent to {ReceiverEmail} from {SenderEmail}", receiverEmail, senderEmail);
        }
    }
}
