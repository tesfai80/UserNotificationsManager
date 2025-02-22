using NotificationManager.API.Models;
using NotificationManager.API.Repositories;
using FluentAssertions;

namespace NotificationManager.API.Tests.Repositories;

public class UserPreferenceRepositoryTests
{
    private readonly IUserPreferenceRepository _repository;

    public UserPreferenceRepositoryTests()
    {
        _repository = new UserPreferenceRepository();
    }

    [Fact]
    public async Task CreateUserPreferenceAsync_ShouldStoreUser()
    {
        // Arrange
        var userPreference = new UserPreference
        (
            1,
            "user@example.com",
             "+123456789",
            new NotificationPreference(true, false)
       );
       
        // Act
        await _repository.CreateUserPreferenceAsync(userPreference);
        var retrievedUser = await _repository.GetUserPreferenceAsync(1);

        // Assert
        retrievedUser.Should().NotBeNull("User preference should exist");
        retrievedUser!.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task UpdateUserPreferenceAsync_ShouldModifyExistingUserPreferences()
    {
        // Arrange
        var userPreference = new UserPreference
        (
            2,
        "user2@example.com",
         "+123456788",
            new NotificationPreference(false, true)
        );

        await _repository.CreateUserPreferenceAsync(userPreference);

        // Act
        await _repository.UpdateUserPreferenceAsync("user2@example.com", new NotificationPreference(true,true));
        var updatedUser = await _repository.GetUserPreferenceAsync(2);

        // Assert
        updatedUser.Preferences.Email.Should().BeTrue();
        updatedUser.Preferences.Sms.Should().BeTrue();
    }
}

