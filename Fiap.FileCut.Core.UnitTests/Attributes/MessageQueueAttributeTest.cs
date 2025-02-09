﻿using Castle.Core.Internal;
using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Objects.Enums;

namespace Fiap.FileCut.Core.UnitTests.Attributes;

public class MessageQueueAttributeTests
{
    [Fact]
    public void Constructor_Should_Set_Queue_Property_Correctly()
    {
        // Arrange
        var expectedQueue = Queues.INFORMATION;

        // Act
        var attribute = new MessageQueueAttribute(expectedQueue);

        // Assert
        Assert.Equal(expectedQueue, attribute.Queue);
    }

    [Fact]
    public void Attribute_Should_Be_Applied_To_Class()
    {
        // Act
        var attributeUsage = typeof(TestClass).GetAttributes<MessageQueueAttribute>().FirstOrDefault();

        // Assert
        Assert.NotNull(attributeUsage);
        Assert.Equal(Queues.INFORMATION, attributeUsage.Queue);
    }

    [MessageQueue(Queues.INFORMATION)]
    private class TestClass { }
}
