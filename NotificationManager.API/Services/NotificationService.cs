
namespace NotificationManager.API.Services;

public class NotificationService: INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationService> _logger;
    private readonly string _baseUrl;

    public NotificationService(HttpClient httpClient, ILogger<NotificationService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["NotificationService:BaseUrl"] ?? "http://localhost:5001";
    }

    public async Task SendNotificationAsync(UserPreference user, string message)
    {
        var tasks = new List<Task>();

        if (user.Preferences.Email)
        {
            tasks.Add(SendEmailAsync(user.Email, message));
        }

        if (user.Preferences.Sms)
        {
            tasks.Add(SendSmsAsync(user.Telephone, message));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError($"One or more notification tasks failed: {ex.Message}");
            throw;
        }
    }

    private async Task SendEmailAsync(string email, string message)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { email, message }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_baseUrl}/send-email", content);
        await HandleResponse(response);
    }

    private async Task SendSmsAsync(string telephone, string message)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { telephone, message }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_baseUrl}/send-sms", content);
       await HandleResponse(response);
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(); 
            _logger.LogError($"Error sending notification: {response.StatusCode}, Response: {errorContent}");
            throw new HttpRequestException($"Error sending notification: {response.StatusCode}, Response: {errorContent}");
        }
    }
}
