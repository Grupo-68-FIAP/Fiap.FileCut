using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Objects.Enums;
using Fiap.FileCut.Core.Objects.QueueEvents;

namespace Fiap.FileCut.Core.UnitTests.Objects;

public class VideoUploadedEventTests
{
    [Fact]
    public void Constructor_Should_Set_VideoName_Property_Correctly()
    {
        // Arrange
        var expectedVideoName = "TestVideo";

        // Act
        var videoUploadedEvent = new VideoUploadedEvent(expectedVideoName);

        // Assert
        Assert.Equal(expectedVideoName, videoUploadedEvent.VideoName);
    }

    [Fact]
    public void Class_Should_Have_MessageQueue_Attribute()
    {
        // Act
        var attribute = (MessageQueueAttribute)Attribute.GetCustomAttribute(
            typeof(VideoUploadedEvent), typeof(MessageQueueAttribute)
        );

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal(Queues.PROCESS, attribute.Queue);
    }
}