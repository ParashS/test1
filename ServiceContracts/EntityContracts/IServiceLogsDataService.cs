using DataTransferObjects.MailSettings;
using Entities.Models;

namespace ServiceContracts.EntityContracts
{
  public interface IServiceLogsDataService
  {
    Task CreateAsync(TbServiceLogs tbServiceLogs);
    Task SendMailAsync(MailData mailData);
    Task CreateServiceNotificationAsync(TbServiceNotifications tbServiceNotifications);
  }
}
