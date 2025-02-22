namespace NotificationManager.API.Models;

public record UserPreference(int UserId, string Email, string Telephone, NotificationPreference Preferences);
