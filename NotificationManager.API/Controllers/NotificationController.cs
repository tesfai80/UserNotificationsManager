
using Microsoft.AspNetCore.Authorization;

namespace NotificationManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await _userRepo.GetUserPreferenceAsync(request.UserId);
        if (user == null)
            return NotFound("User not found.");

        await _notificationService.SendNotificationAsync(user, request.Message);
        return Ok(new { status = "sent", message = "Notification sent successfully." });
    }
    [HttpPut("preferences")]
    public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdateUserPreferenceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _userRepo.UpdateUserPreferenceAsync(request.Email, request.Preferences);
        return success ? Ok(new { status = "updated", message = "User preferences updated." }) : NotFound("User not found.");
    }
}
