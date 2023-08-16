using Contracts;
using DataTransferObjects.MailSettings;
using Entities.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using ServiceContracts.EntityContracts;

namespace Services.EntityServices
{
    public class ServiceLogsDataService : IServiceLogsDataService
    {
        private IRepositoryWrapper _repositoryWrapper;
        private readonly DataTransferObjects.MailSettings.MailSettings _mailSettings;
        private readonly ISendGridClient _sendGridClient;

        public ServiceLogsDataService(IRepositoryWrapper repositoryWrapper, IOptions<DataTransferObjects.MailSettings.MailSettings> mailSettingsOptions, ISendGridClient sendGridClient)
        {
            _repositoryWrapper = repositoryWrapper;
            _mailSettings = mailSettingsOptions.Value;
            _sendGridClient = sendGridClient;
        }

        public async Task CreateAsync(TbServiceLogs tbServiceLogs)
        {
            await _repositoryWrapper.ServiceLogsData.CreateAsync(tbServiceLogs);
            await _repositoryWrapper.SaveAync();

            await SendMailAsync(new MailData()
            {
                EmailSubject = string.Concat(string.Join("-", tbServiceLogs.ServiceName, tbServiceLogs.StatusCode, tbServiceLogs.StatusDescription)),
                EmailBody = tbServiceLogs.ServiceName == "Veslink" ?
              $"<p>We have failed to call the {tbServiceLogs.ServiceName} API to submit the vessel report. The reason for failing the API call is:</p>" +
              $"<p><b>Error Code {tbServiceLogs.StatusCode}:</b> {tbServiceLogs.StatusDescription}</p>" +
              $"<p><b>Resource URL:</b> {tbServiceLogs.ResourceUri}</p>" +
              $"<p><b>Called At:</b> {tbServiceLogs.CreatedDateTime}</p>" +
              $"<p><b>Request:</b><br/>{tbServiceLogs.RequestJSONBody}</p>" +
              $"<p><b>Response:</b><br/>{tbServiceLogs.ResponseJSONBody}</p>" :

              $"<p>We have failed to call the email - veslinkapi@ship-watch.com to fetch the XML reports. The reason for failing is:</p>" +
              $"<p><b>Error Code {tbServiceLogs.StatusCode}:</b> {tbServiceLogs.StatusDescription}</p>" +
              $"<p><b>Resource URL:</b> {tbServiceLogs.ResourceUri}</p>" +
              $"<p><b>Called At:</b> {tbServiceLogs.CreatedDateTime}</p>"
            });
        }

        public async Task SendMailAsync(MailData mailData)
        {
            try
            {
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName),
                    Subject = mailData.EmailSubject,
                    HtmlContent = mailData.EmailBody,
                };
               
                msg.AddTos(_mailSettings.EmailAddresses);
                //await _sendGridClient.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                await _repositoryWrapper.ServiceLogsData.CreateAsync(new TbServiceLogs()
                {
                    ServiceName = "Mail",
                    StartedDateTime = DateTime.UtcNow,
                    EndedDateTime = DateTime.UtcNow,
                    CreatedDateTime = DateTime.UtcNow,
                    ResourceUri = null,
                    RecordsToBeProcessed = 0,
                    RecordsActuallyProcessed = 0,
                    Content = ex.Message,
                    Source = ex.Source,
                    Message = ex.Message,
                    InnerException = ex?.InnerException?.ToString(),
                    StackTrace = ex?.StackTrace
                });
                await _repositoryWrapper.SaveAync();
            }
        }

        public async Task CreateServiceNotificationAsync(TbServiceNotifications tbServiceNotifications)
        {
            await _repositoryWrapper.ServiceNotificationsData.CreateAsync(tbServiceNotifications);
            await _repositoryWrapper.SaveAync();
        
            //Commenting the below code for stopping mails as of now. We already inserting the same records in database.
            //await SendMailAsync(new MailData()
            //{
            //    EmailSubject = string.Concat(string.Join(" - ", tbServiceNotifications.ServiceName, tbServiceNotifications.EmailSubject)),
            //    EmailBody = $"<p>{tbServiceNotifications.Description}</p>"
            //});
        }
    }
}
