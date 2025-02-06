using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Extensions;
using Fiap.FileCut.Core.Objects.Enums;

namespace Fiap.FileCut.Core.UnitTests.Extensions;

public class MessageQueueExtensionTests
{
    private enum TestQueue
    {
        [MessageQueueName("TestQueueName")] TestValue
    }

    [Fact]
    public void GetQueueNameAttribute_Should_Return_Correct_QueueName()
    {
        // Arrange
        var enumValue = TestQueue.TestValue;

        // Act
        var queueName = enumValue.GetQueueNameAttribute();

        // Assert
        Assert.Equal("TestQueueName", queueName);
    }

    [MessageQueueAttribute(Queues.INFORMATION)]
    private class TestClassWithAttribute
    {
        public string? Property { get; set; }
    }

    [Fact]
    public void GetQueueName_Should_Return_Correct_QueueName()
    {
        // Act
        var queueName = MessageQueueExtension.GetQueueName<TestClassWithAttribute>();

        // Assert
        Assert.NotNull(queueName);
        Assert.Equal("FIAP-FILECUT-INFORMATION-QUEUE", queueName);
    }
}