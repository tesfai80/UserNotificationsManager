
namespace NotificationManager.API.Repositories;

public class UserPreferenceRepository: IUserPreferenceRepository
{
    private readonly ConcurrentDictionary<int, UserPreference> _userPreferences = new();
    public UserPreferenceRepository()
    {
        // Preload users
        _userPreferences[1] = new UserPreference(1, "tesfaytadesse80@gmail.com", "+972546592985", new NotificationPreference(true, true));
        _userPreferences[2] = new UserPreference(2, "loki@avengers.com", "+123456788", new NotificationPreference(true, false));
    }

    public Task<UserPreference?> GetUserPreferenceAsync(int userId)
        => Task.FromResult(_userPreferences.TryGetValue(userId, out var pref) ? pref : null);

    public Task CreateUserPreferenceAsync(UserPreference userPreference)
    {
        _userPreferences[userPreference.UserId] = userPreference;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateUserPreferenceAsync(string email, NotificationPreference preferences)
    {
        var user = _userPreferences.Values.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            _userPreferences[user.UserId] = user with { Preferences = preferences };
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }    
}
