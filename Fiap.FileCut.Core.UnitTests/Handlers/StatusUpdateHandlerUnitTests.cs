using Fiap.FileCut.Core.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.UnitTests.Handlers
{
    public class StatusUpdateHandlerUnitTests
    {
        [Fact]
        public async Task HandleAsync_WhenValidContext_ShouldNotify()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var notifyService = new Mock<INotifyService>();
            var logger = new Mock<ILogger<StatusUpdateHandler>>();
            var handler = new StatusUpdateHandler(notifyService.Object, logger.Object);
            var context = new NotifyContext<string>("test", userId);
            // Act
            await handler.HandleAsync(context);
            // Assert
            notifyService.Verify(c => c.NotifyAsync(It.IsAny<NotifyContext<FileCutMailMessage>>()), Times.Once);
            logger.Verify(x => x.Log(It.Is<LogLevel>(ll => ll == LogLevel.Debug), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}
