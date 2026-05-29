using EgorSalahovSemestrovka22.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EgorSalahovSemestrovka22.Services
{
    public class EmailService
    {
        private readonly SmtpConfig _primary;
        private readonly SmtpConfig _fallback;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _primary = smtpSettings.Value.Primary;
            _fallback = smtpSettings.Value.Fallback;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Пробуем основной SMTP
            try
            {
                await SendViaSmtpAsync(_primary, toEmail, subject, body);
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Primary SMTP failed: {ex.Message}");
            }

            // Fallback — пробуем Gmail
            try
            {
                await SendViaSmtpAsync(_fallback, toEmail, subject, body);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fallback SMTP also failed: {ex.Message}");
                throw new Exception("Both SMTP servers failed to send email.");
            }
        }

        private async Task SendViaSmtpAsync(SmtpConfig config, string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(config.Host, config.Port)
            {
                EnableSsl = config.EnableSsl,
                Credentials = new NetworkCredential(config.UserName, config.Password),
                Timeout = 5000 // 5 секунд
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(config.FromEmail, config.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
