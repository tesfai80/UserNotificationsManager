

namespace NotificationManager.API.Models;
public record UpdateUserPreferenceRequest(
    [Required, EmailAddress(ErrorMessage = "Invalid email format.")] string Email,
    [Required] NotificationPreference Preferences
);

