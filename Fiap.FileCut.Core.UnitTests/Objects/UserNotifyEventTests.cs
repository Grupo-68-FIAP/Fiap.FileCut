using Fiap.FileCut.Core.Objects.QueueEvents;

namespace Fiap.FileCut.Core.UnitTests.Objects;

public class UserNotifyEventTests
{
    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var expectedVideoName = "TestVideo";

        // Act
        var userNotifyEvent = new UserNotifyEvent(expectedVideoName);

        // Assert
        Assert.Equal(expectedVideoName, userNotifyEvent.VideoName);
        Assert.True(userNotifyEvent.IsSuccess);
        Assert.Null(userNotifyEvent.ErrorMessage);
        Assert.Null(userNotifyEvent.PackName);
    }
}