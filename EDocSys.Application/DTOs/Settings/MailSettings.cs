namespace EDocSys.Application.DTOs.Settings
{
    public class MailSettings
    {
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EmailOverride { get; set; }
        public string OverrideAddress { get; set; }
        public string ExcludedEmails { get; set; }
    }
}