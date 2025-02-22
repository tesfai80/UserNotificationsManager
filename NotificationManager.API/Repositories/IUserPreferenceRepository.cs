
namespace NotificationManager.API.Repositories;

public interface  IUserPreferenceRepository
{
    Task<UserPreference?> GetUserPreferenceAsync(int userId);
    Task CreateUserPreferenceAsync(UserPreference userPreference);
    Task<bool> UpdateUserPreferenceAsync(string email, NotificationPreference preferences);
}
