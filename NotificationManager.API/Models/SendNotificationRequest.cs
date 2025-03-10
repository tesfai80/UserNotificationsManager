namespace NotificationManager.API.Models;

public record SendNotificationRequest(
    [Required(ErrorMessage = "UserId is required.")] int UserId,
    [Required, MinLength(5, ErrorMessage = "Message must be at least 5 characters long.")] string Message
);