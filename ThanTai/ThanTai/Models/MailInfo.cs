using System.ComponentModel.DataAnnotations.Schema;

namespace ThanTai.Models
{
    [NotMapped]
    public class MailInfo
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string TemplateName { get; set; } // Tên file HTML trong wwwroot/Emails
        public Dictionary<string, string> DataReplacements { get; set; } // Dữ liệu thay thế trong template

        public List<IFormFile>? Attachments { get; set; }
    }

    [NotMapped]
    public class MailSettings
    {
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
