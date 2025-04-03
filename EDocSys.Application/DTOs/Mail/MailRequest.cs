using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace EDocSys.Application.DTOs.Mail
{
    public class MailRequest
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}