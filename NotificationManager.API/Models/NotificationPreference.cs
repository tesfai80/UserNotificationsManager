namespace NotificationManager.API.Models;

public record NotificationPreference(
    [Required] bool Email,
    [Required] bool Sms
);
