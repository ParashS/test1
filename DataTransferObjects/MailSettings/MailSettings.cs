using SendGrid.Helpers.Mail;

namespace DataTransferObjects.MailSettings
{
    public class MailSettings
    {
        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
        public string? EmailTo { get; set; }
        public List<EmailAddress>? EmailAddresses { get; set; }
    }
}
