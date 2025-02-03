using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Services;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Services;

public class UserServiceUnitTests
{
    [Fact]
    public async Task GetUsers_WhenCalled_ShouldReturnAllUsers()
    {
        // Arrange
        var userRepository = new Mock<IUserRepository>();
        var user = new User { Id = Guid.NewGuid(), Name = "User 1", Email = "admin@test.com" };

        userRepository.Setup(x => x.GetUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var userService = new UserService(userRepository.Object);
        // Act
        var result = await userService.GetUserAsync(Guid.NewGuid());
        // Assert
        Assert.Equal(user, result);
        var uResult = Assert.NotNull(result);
        Assert.NotNull(uResult.Email);
        Assert.NotNull(uResult.FullName);
    }
}
