using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Services;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Services;

public class NotifyServiceUnitTests
{
    [Fact]
    public void Notify_WhenCalled_ShouldCallNotifyAsyncOnAllAdapters()
    {
        // Arrange
        var adapter1 = new Mock<INotifyAdapter>();
        var adapter2 = new Mock<INotifyAdapter>();
        var notifyService = new NotifyService([adapter1.Object, adapter2.Object]);
        // Act
        var exception = Record.Exception(() => notifyService.Notify(new NotifyContext<string>("test", Guid.NewGuid())));
        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task NotifyAsync_WhenCalled_ShouldCallNotifyAsyncOnAllAdapters()
    {
        // Arrange
        var adapter1 = new Mock<INotifyAdapter>();
        var adapter2 = new Mock<INotifyAdapter>();
        var notifyService = new NotifyService([adapter1.Object, adapter2.Object]);
        // Act
        await notifyService.NotifyAsync(new NotifyContext<string>("test", Guid.NewGuid()));
        // Assert
        adapter1.Verify(x => x.NotifyAsync(It.IsAny<NotifyContext<string>>()), Times.Once);
        adapter2.Verify(x => x.NotifyAsync(It.IsAny<NotifyContext<string>>()), Times.Once);
    }
}
