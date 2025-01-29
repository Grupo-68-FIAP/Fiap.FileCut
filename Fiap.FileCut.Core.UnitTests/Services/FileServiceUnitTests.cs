using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.UnitTests.Services;

public class FileServiceUnitTests
{
    [Fact]
    public async Task GetAllFilesName_WhenCalled_ShouldReturnAllFilesName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileRepositoryMock = new Mock<IFileRepository>();
        var _logger = new Mock<ILogger<FileService>>(); 

		var fileService = new FileService(fileRepositoryMock.Object, _logger.Object);
        // Act
        _ = await Assert.ThrowsAsync<NotImplementedException>(async () => await fileService.GetFileNamesAsync(userId, CancellationToken.None));
        // TODO NOSONAR: Implementar o teste corretamente
    }
}
