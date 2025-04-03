using AutoMapper.Configuration;
using EDocSys.Application.DTOs.Mail;
using EDocSys.Application.DTOs.Settings;
using EDocSys.Application.Interfaces.Shared;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                // Handle email overriding (for development/testing)
                if (_settings.EmailOverride == true && !string.IsNullOrEmpty(_settings.OverrideAddress))
                {
                    // Add the original recipient information to the email body
                    email.To.Add(MailboxAddress.Parse(_settings.OverrideAddress));

                    string excludedEmail = "";
                    if (!string.IsNullOrEmpty(request.To))
                    {
                        var excludedEmails = _settings.ExcludedEmails?.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                            .Select(e => e.Trim().ToLower())
                            .ToList() ?? new List<string>();

                        foreach (var to in SplitEmailAddresses(request.To))
                        {
                            // Skip excluded email addresses
                            if (excludedEmails.Contains(to.Trim().ToLower()))
                            {
                                _logger.LogInformation($"Excluding recipient email: {to}");
                                excludedEmail += $"EXCLUDED: {to}<br>";
                                continue;
                            }
                        }
                    }


                    // Modify the body to include the original recipients
                    string originalBody = request.Body;
                    string originalRecipients = excludedEmail + $"Original TO: {request.To}<br>";

                    if (!string.IsNullOrEmpty(request.Cc))
                    {
                        originalRecipients += $"Original CC: {request.Cc}<br>";
                    }

                    if (!string.IsNullOrEmpty(request.Bcc))
                    {
                        originalRecipients += $"Original BCC: {request.Bcc}<br>";
                    }

                    request.Body = originalRecipients + "<br>" + originalBody;
                }
                else
                {
                    // Normal email sending - add actual recipients
                    // Add TO recipients
                    if (!string.IsNullOrEmpty(request.To))
                    {
                        var excludedEmails = _settings.ExcludedEmails?.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                            .Select(e => e.Trim().ToLower())
                            .ToList() ?? new List<string>();    

                        foreach (var to in SplitEmailAddresses(request.To))
                        {
                            // Skip excluded email addresses
                            if (excludedEmails.Contains(to.Trim().ToLower()))
                            {
                                _logger.LogInformation($"Excluding recipient email: {to}");
                                continue;
                            }
                            email.To.Add(MailboxAddress.Parse(to));
                        }
                    }

                    // Add CC recipients if any
                    if (!string.IsNullOrEmpty(request.Cc))
                    {
                        foreach (var cc in SplitEmailAddresses(request.Cc))
                        {
                            email.Cc.Add(MailboxAddress.Parse(cc));
                        }
                    }

                    // Add BCC recipients if any
                    if (!string.IsNullOrEmpty(request.Bcc))
                    {
                        foreach (var bcc in SplitEmailAddresses(request.Bcc))
                        {
                            email.Bcc.Add(MailboxAddress.Parse(bcc));
                        }
                    }
                }

                email.Subject = request.Subject;

                // Create message body and add attachments
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

                // Send the email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_settings.Host, 25, SecureSocketOptions.None);
                await smtp.AuthenticateAsync(_settings.Address, _settings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully. Subject: {request.Subject}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to send email. Subject: {request.Subject}, Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Splits a string of email addresses separated by semicolons or commas
        /// </summary>
        private List<string> SplitEmailAddresses(string emailAddresses)
        {
            if (string.IsNullOrEmpty(emailAddresses))
                return new List<string>();

            return new List<string>(emailAddresses.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries));
        }
    }
}