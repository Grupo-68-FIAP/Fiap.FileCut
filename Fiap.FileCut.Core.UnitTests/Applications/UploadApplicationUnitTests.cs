using Fiap.FileCut.Core.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Applications;

public class UploadApplicationUnitTests
{
    [Fact]
    public async Task UploadApplication_UploadFile_ShouldReturnSuccess()
    {
        // Arrange
        var notifyService = new Mock<INotifyService>();
        var fileService = new Mock<IFileService>();
        var uploadApplication = new UploadApplication(notifyService.Object, fileService.Object);
        var userId = Guid.NewGuid();
        var fileName = "file.txt";
        var fileStream = new MemoryStream();
        var cancellationToken = new CancellationToken();
        fileService.Setup(x => x.SaveFileAsync(userId, fileName, fileStream, CancellationToken.None)).ReturnsAsync(true);
        // Act
        var result = await uploadApplication.UploadFileAsync(userId, fileName, fileStream, cancellationToken);
        // Assert
        Assert.True(result);
        notifyService.Verify(x => x.NotifyAsync(It.IsAny<NotifyContext<VideoUploadedEvent>>()), Times.Once);
        fileService.Verify(x => x.SaveFileAsync(userId, fileName, fileStream, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UploadApplication_UploadFile_ShouldReturnFalse()
    {
        // Arrange
        var notifyService = new Mock<INotifyService>();
        var fileService = new Mock<IFileService>();
        var uploadApplication = new UploadApplication(notifyService.Object, fileService.Object);
        var userId = Guid.NewGuid();
        var fileName = "file.txt";
        var fileStream = new MemoryStream();
        var cancellationToken = new CancellationToken();
        fileService.Setup(x => x.SaveFileAsync(userId, fileName, fileStream, CancellationToken.None)).ReturnsAsync(false);
        // Act
        var result = await uploadApplication.UploadFileAsync(userId, fileName, fileStream, cancellationToken);
        // Assert
        Assert.False(result);
        notifyService.Verify(x => x.NotifyAsync(It.IsAny<NotifyContext<UserNotifyEvent>>()), Times.Once);
        fileService.Verify(x => x.SaveFileAsync(userId, fileName, fileStream, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UploadApplication_UploadFile_ShouldReturnFalseAndNotifyUser()
    {
        // Arrange
        var notifyService = new Mock<INotifyService>();
        var fileService = new Mock<IFileService>();
        var uploadApplication = new UploadApplication(notifyService.Object, fileService.Object);
        var userId = Guid.NewGuid();
        var fileName = "file.txt";
        var fileStream = new MemoryStream();
        var cancellationToken = new CancellationToken();
        fileService.Setup(x => x.SaveFileAsync(userId, fileName, fileStream, CancellationToken.None)).ThrowsAsync(new Exception("Error"));
        // Act
        var result = await uploadApplication.UploadFileAsync(userId, fileName, fileStream, cancellationToken);
        // Assert
        Assert.False(result);
        notifyService.Verify(x => x.NotifyAsync(It.IsAny<NotifyContext<UserNotifyEvent>>()), Times.Once);
        fileService.Verify(x => x.SaveFileAsync(userId, fileName, fileStream, CancellationToken.None), Times.Once);
    }
}
