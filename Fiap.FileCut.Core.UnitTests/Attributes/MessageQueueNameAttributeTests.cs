using Castle.Core.Internal;
using Fiap.FileCut.Core.Attributes;

namespace Fiap.FileCut.Core.UnitTests.Attributes;

public class MessageQueueNameAttributeTests
{
    [Fact]
    public void Constructor_Should_Set_QueueName_Property_Correctly()
    {
        // Arrange
        var expectedQueueName = "TestQueue";

        // Act
        var attribute = new MessageQueueNameAttribute(expectedQueueName);

        // Assert
        Assert.Equal(expectedQueueName, attribute.QueueName);
    }

    [Fact]
    public void Attribute_Should_Be_Applied_To_Field()
    {
        // Act
        var attributeUsage = typeof(TestEnum).GetAttributes<MessageQueueNameAttribute>().FirstOrDefault();

        // Assert
        Assert.NotNull(attributeUsage);
        Assert.Equal("TestQueue", attributeUsage.QueueName);
    }

    private enum TestEnum
    {
        [MessageQueueName("TestQueue")]
        TestField,
    }
}
