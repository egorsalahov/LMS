using EgorSalahovSemestrovka22.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace EgorSalahovSemestrovka22.Services
{
    public class EmailService
    {
        private readonly SmtpConfig _primary;
        private readonly SmtpConfig _fallback;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _primary = smtpSettings.Value.Primary;
            _fallback = smtpSettings.Value.Fallback;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            _logger.LogInformation("Отправка письма на {Email}, тема: {Subject}", toEmail, subject);

            try
            {
                await SendViaSmtpAsync(_primary, toEmail, subject, body);
                _logger.LogInformation("Письмо успешно отправлено через основной SMTP");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Основной SMTP не сработал, пробуем fallback");
            }

            try
            {
                await SendViaSmtpAsync(_fallback, toEmail, subject, body);
                _logger.LogInformation("Письмо отправлено через fallback SMTP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Оба SMTP сервера не сработали");
                throw new Exception("Both SMTP servers failed to send email.", ex);
            }
        }

        private async Task SendViaSmtpAsync(SmtpConfig config, string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(config.Host, config.Port)
            {
                EnableSsl = config.EnableSsl,
                Credentials = new NetworkCredential(config.UserName, config.Password),
                Timeout = 5000
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