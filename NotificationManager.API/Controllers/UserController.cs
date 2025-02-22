

namespace NotificationManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserPreferenceRepository _userRepo;
    public UserController(IUserPreferenceRepository userRepo)
    {
        _userRepo = userRepo;
    }

    /// <summary>
    /// Creates a new user preference entry.
    /// </summary>
    [HttpPost("preferences")]
    public async Task<IActionResult> CreateUserPreferences([FromBody] UserPreference userPreference)
    {
        await _userRepo.CreateUserPreferenceAsync(userPreference);
        return CreatedAtAction(nameof(GetUserPreferences), new { userId = userPreference.UserId }, userPreference);
    }

    /// <summary>
    /// Updates an existing user's notification preferences.
    /// </summary>
    [HttpPut("preferences")]
    public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdateUserPreferenceRequest request)
    {
        var success = await _userRepo.UpdateUserPreferenceAsync(request.Email, request.Preferences);
        if (!success)
        {
            return NotFound("User not found."); 
        }

        return Ok("User preferences updated.");
    }

    /// <summary>
    /// Retrieves a user's notification preferences.
    /// </summary>
    [HttpGet("{userId}/preferences")]
    public async Task<IActionResult> GetUserPreferences(int userId)
    {
        var userPreferences = await _userRepo.GetUserPreferenceAsync(userId);
        if (userPreferences == null)
        {
            return NotFound("User not found.");
        }
        return Ok(userPreferences);
    }
}
