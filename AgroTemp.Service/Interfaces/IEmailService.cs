using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
    }
}
