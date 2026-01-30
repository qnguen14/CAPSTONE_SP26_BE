using AgroTemp.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Resend;
using System.Threading.Tasks;

namespace AgroTemp.Service.Implements
{
    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly string _fromEmail;

        public ResendEmailService(IResend resend, IConfiguration configuration)
        {
            _resend = resend;
            _fromEmail = configuration["Resend:FromEmail"] ?? "no-reply@agrotemp.dev"; 
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var message = new EmailMessage
            {
                From = _fromEmail,
                To = { to },
                Subject = subject,
                HtmlBody = htmlMessage
            };

            await _resend.EmailSendAsync(message);
        }
    }
}
