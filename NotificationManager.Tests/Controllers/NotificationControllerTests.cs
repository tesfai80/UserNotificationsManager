using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationManager.API.Controllers;
using NotificationManager.API.Models;
using NotificationManager.API.Repositories;
using NotificationManager.API.Services;

namespace NotificationManager.Tests.Controllers;

public class NotificationControllerTests
{
    private readonly Mock<IUserPreferenceRepository> _userRepoMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly NotificationController _controller;

    public NotificationControllerTests()
    {
        _userRepoMock = new Mock<IUserPreferenceRepository>();
        _notificationServiceMock = new Mock<INotificationService>();

        _controller = new NotificationController(_userRepoMock.Object, _notificationServiceMock.Object);
    }

    [Fact]
    public async Task SendNotification_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepoMock.Setup(repo => repo.GetUserPreferenceAsync(It.IsAny<int>()))
            .ReturnsAsync((UserPreference)null);

        var request = new SendNotificationRequest (99,"Test message");

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task SendNotification_ShouldReturnOk_WhenNotificationIsSent()
    {
        // Arrange
        var user = new UserPreference
        (
            1,
             "test@example.com",
             "+123456789",
            new NotificationPreference(true, true)
        );

        _userRepoMock.Setup(repo => repo.GetUserPreferenceAsync(1)).ReturnsAsync(user);
        _notificationServiceMock.Setup(service => service.SendNotificationAsync(user, "Hello")).Returns(Task.CompletedTask);

        var request = new SendNotificationRequest(1,"Hello");

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}

