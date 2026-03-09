using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using URLapi.Domain.Config;
using URLapi.Domain.Entities;
using URLapi.Domain.IServices;

namespace URLapi.Infra.Services;

public class EmailService(IOptions<EmailSettings> settings) : IVerificateUserService
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendVerificationAsync(User user, CancellationToken cancellationToken)
    {
        await SendEmailAsync(
            user.Email.ToString(),
            "Verificação de Email",
            WriteVerificationEmailBody(user.Email.Verification.VerifyHashCode.ToString()),
            _settings.SmtpUsername,
            _settings.SmtpPassword
        );
    }

    private static async Task SendEmailAsync(string recipientEmail, string subject, string messageBody, string userName,
        string password)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Novaweb", userName)); // Sender's display name and email
        message.To.Add(new MailboxAddress("Cliente", recipientEmail)); // Recipient's display name and email
        message.Subject = subject;

        message.Body = new TextPart("HTML") { Text = messageBody };

        using (var client = new SmtpClient())
        {
            // Connect to the Office 365 SMTP server
            await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);

            // Authenticate with your Microsoft 365 credentials (use App Passwords if MFA is enabled)
            await client.AuthenticateAsync(userName, password);

            // Send the email
            await client.SendAsync(message);

            // Disconnect from the server
            await client.DisconnectAsync(true);
        }
    }

    private static string WriteVerificationEmailBody(string token)
    {
        return
            @$"http://localhost:5106/v1/users/verify-{token}"; // TODO colocar porta da aplicação, tentar pegar pelo Asembly
    }
}