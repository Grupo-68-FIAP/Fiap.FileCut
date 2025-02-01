using Fiap.FileCut.Infra.IdentityProvider.Keycloak;
using Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;

namespace Fiap.FileCut.Infra.IdentityProvider.UnitTests.Keycloak;

public class UserRepositoryUnitTests
{
    [Fact]
    public async Task GetUserAsync_WhenCalled_ReturnsUser()
    {
        // Arrange
        var id = new Guid("c9e0258d-6864-49d9-85de-16fa38990ec3");
        var configuration = new KeycloakConfiguration()
        {
            Realm = "sample",
            ClientId = "admin-cli",
            ClientSecret = "ggHA5bduwAAv3dQDw5JOJYwyW75wrUZI",
            KeycloakUrl = "http://keycloak:8080"
        };
        var userRepository = new KeycloakUserRepository(configuration);
        // Act
        var opUser = await userRepository.GetUserAsync(id);
        // Assert
        var user = Assert.NotNull(opUser);
        Assert.Equal(id, user.Id);
    }
}
