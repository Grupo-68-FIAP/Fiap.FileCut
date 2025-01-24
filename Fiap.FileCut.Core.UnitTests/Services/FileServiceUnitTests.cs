using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
        var fileService = new FileService(fileRepositoryMock.Object);
        // Act
        _ = await Assert.ThrowsAsync<NotImplementedException>(async () => await fileService.GetAllFilesName(userId));
        // TODO NOSONAR: Implementar o teste corretamente
    }
}
