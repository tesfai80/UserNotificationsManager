
namespace NotificationManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly IUserPreferenceRepository _userRepo;
    private readonly INotificationService _notificationService;

    public NotificationController(IUserPreferenceRepository userRepo, INotificationService notificationService)
    {
        _userRepo = userRepo;
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        var user = await _userRepo.GetUserPreferenceAsync(request.UserId);
        if (user == null)
            return NotFound("User not found.");

        await _notificationService.SendNotificationAsync(user, request.Message);
        return Ok("Notification sent.");
    }
}
