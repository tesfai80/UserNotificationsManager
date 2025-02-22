using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NotificationManager.API.Models;
using NotificationManager.API.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NotificationManager.API.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<NotificationService>> _loggerMock;
    private readonly IConfiguration _configuration;
    private readonly NotificationService _notificationService;

    public NotificationServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _loggerMock = new Mock<ILogger<NotificationService>>();

        var inMemorySettings = new Dictionary<string, string>
            {
                { "NotificationService:BaseUrl", "http://mock-notification-service" }
            };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _notificationService = new NotificationService(_httpClient, _loggerMock.Object, _configuration);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldSucceed_WhenServiceReturnsSuccess()
    {
        // Arrange
        var user = new UserPreference(1, "test@example.com", "+123456789", new NotificationPreference(true, false));

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new { status = "sent" }), Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        // Act
        var exception = await Record.ExceptionAsync(() => _notificationService.SendNotificationAsync(user, "Test Message"));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenServiceReturnsError()
    {
        // Arrange
        var user = new UserPreference(1, "test@example.com", "+123456789", new NotificationPreference(true, false));

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent(JsonSerializer.Serialize(new { error = "Invalid request" }), Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _notificationService.SendNotificationAsync(user, "Test Email Message"));

        exception.Message.Should().Contain("BadRequest");
    }
}

