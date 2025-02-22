namespace NotificationManager.API.Models;

public record UpdateUserPreferenceRequest(string Email, NotificationPreference Preferences);

