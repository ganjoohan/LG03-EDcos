using EDocSys.Application.DTOs.Mail;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Shared
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}