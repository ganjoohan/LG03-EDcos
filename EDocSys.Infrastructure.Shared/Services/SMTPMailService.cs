using EDocSys.Application.DTOs.Mail;
using EDocSys.Application.DTOs.Settings;
using EDocSys.Application.Interfaces.Shared;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Shared.Services
{
    public class SMTPMailService : IMailService
    {
        public MailSettings _settings { get; }
        public ILogger<SMTPMailService> _logger { get; }

        public SMTPMailService(IOptions<MailSettings> mailSettings, ILogger<SMTPMailService> logger)
        {
            _settings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.DisplayName, request.From ?? _settings.Address));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;

                foreach (var cc in request.Cc)
                {
                    email.Cc.Add(MailboxAddress.Parse(cc));
                }

                var builder = new BodyBuilder();
                if (request.Attachments != null)
                {
                    byte[] fileBytes;
                    foreach (var file in request.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                fileBytes = ms.ToArray();
                            }
                            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                        }
                    }
                }
                builder.HtmlBody = request.Body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                //await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect);
                await smtp.ConnectAsync(_settings.Host, 25, SecureSocketOptions.None);
                await smtp.AuthenticateAsync(_settings.Address, _settings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}