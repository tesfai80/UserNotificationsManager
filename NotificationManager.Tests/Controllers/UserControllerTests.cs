using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationManager.API.Controllers;
using NotificationManager.API.Models;
using NotificationManager.API.Repositories;

namespace NotificationManager.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserPreferenceRepository> _userRepoMock;
    private readonly UserController _controller;
    public UserControllerTests()
    {
        _userRepoMock = new Mock<IUserPreferenceRepository>();
        _controller = new UserController(_userRepoMock.Object);
    }
    [Fact]
    public async Task GetUserPreferences_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepoMock.Setup(repo => repo.GetUserPreferenceAsync(It.IsAny<int>()))
            .ReturnsAsync((UserPreference)null);

        // Act
        var result = await _controller.GetUserPreferences(99);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetUserPreferences_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var user = new UserPreference(1, "test@example.com", "+123456789", new NotificationPreference(true, false));
        _userRepoMock.Setup(repo => repo.GetUserPreferenceAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserPreferences(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(user);
    }

    [Fact]
    public async Task CreateUserPreferences_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var user = new UserPreference(2, "newuser@example.com", "+987654321", new NotificationPreference(false, true));

        _userRepoMock.Setup(repo => repo.CreateUserPreferenceAsync(It.IsAny<UserPreference>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateUserPreferences(user);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task UpdateUserPreferences_ShouldReturnOk_WhenUpdateSuccessful()
    {
        // Arrange
        var updateRequest = new UpdateUserPreferenceRequest("test@example.com", new NotificationPreference(true, true));

        _userRepoMock.Setup(repo => repo.UpdateUserPreferenceAsync(
                It.Is<string>(email => email == updateRequest.Email),
                It.Is<NotificationPreference>(pref => pref.Email == true && pref.Sms == true)
            ))
            .Returns(Task.FromResult(true)); 

        // Act
        var result = await _controller.UpdateUserPreferences(updateRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateUserPreferences_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var updateRequest = new UpdateUserPreferenceRequest("unknown@example.com", new NotificationPreference(false, false));

        _userRepoMock.Setup(repo => repo.UpdateUserPreferenceAsync(
                It.Is<string>(email => email == updateRequest.Email),
                It.Is<NotificationPreference>(pref => pref.Email == false && pref.Sms == false)
            ))
            .Returns(Task.FromResult(false)); 

        // Act
        var result = await _controller.UpdateUserPreferences(updateRequest);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
