using NotificationManager.API.Models;

namespace NotificationManager.API.Services;

public interface INotificationService
{
    Task SendNotificationAsync(UserPreference user, string message);
}
