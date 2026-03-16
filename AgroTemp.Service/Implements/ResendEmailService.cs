using AgroTemp.Service.Interfaces;
using AgroTemp.Service.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resend;
using System.Threading.Tasks;

namespace AgroTemp.Service.Implements
{
    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly string _fromEmail;
        private readonly ILogger<ResendEmailService> _logger;

        public ResendEmailService(IResend resend, IConfiguration configuration, ILogger<ResendEmailService> logger)
        {
            _resend = resend;
            _fromEmail = configuration["Resend:FromEmail"] ?? "no-reply@agrotemp.dev"; 
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                _logger.LogInformation("Attempting to send email to {To} with subject {Subject}", to, subject);
                var styledMessage = EmailTemplateBuilder.BuildBasicTemplate(subject, htmlMessage);

                var message = new EmailMessage
                {
                    From = _fromEmail,
                    To = { to },
                    Subject = subject,
                    HtmlBody = styledMessage
                };

                await _resend.EmailSendAsync(message);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw; // Re-throw to ensure caller knows about failure
            }
        }
    }
}
