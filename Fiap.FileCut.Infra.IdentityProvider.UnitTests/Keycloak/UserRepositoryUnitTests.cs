using Fiap.FileCut.Infra.IdentityProvider.Keycloak;
using Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using System.Net;

namespace Fiap.FileCut.Infra.IdentityProvider.UnitTests.Keycloak;

public class UserRepositoryUnitTests
{
    [Fact]
    public async Task GetUserAsync_WhenCalled_ReturnsUser()
    {
        // Arrange
        var id = new Guid("9b5199a0-7436-4168-80b3-6966ef60c1f2");

        var memoryCache = new FakeCache();

        var oidcConfigurationResponse = await GetJsonFileByName("oidc-configuration-response.json");
        var oidcGetTokenResponse = await GetJsonFileByName("oidc-get-tonken-response.json");
        var oidcGetUserResponse = await GetJsonFileByName("keycloak-get-user-response.json");

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.NotNull(req.RequestUri);
                if (req.RequestUri.AbsolutePath.Contains("realms/sample/.well-known/openid-configuration")
                || req.RequestUri.AbsolutePath.Contains("/realms/sample/protocol/openid-connect/certs"))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(oidcConfigurationResponse)
                    };
                }
                else if (req.RequestUri.AbsolutePath.Contains("realms/sample/protocol/openid-connect/token"))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(oidcGetTokenResponse)
                    };
                }
                else if (req.RequestUri.AbsolutePath.Contains($"realms/sample/users/{id}"))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(oidcGetUserResponse),
                    };
                }
                throw new Exception("Unexpected request");
            });
        var httpClient = new HttpClient(mockHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var configuration = new KeycloakConfiguration(
            realm: "sample",
            clientId: "admin-cli",
            clientSecret: "fack key",
            authority: "http://keycloak:8080/realms/sample"
        );

        var userRepository = new KeycloakUserRepository(memoryCache, configuration, httpClientFactory.Object);
        // Act
        var opUser = await userRepository.GetUserAsync(id);
        // Assert
        var user = Assert.NotNull(opUser);
        Assert.Equal(id, user.Id);
        Assert.NotNull(user.Email);
    }

    [Fact]
    public async Task GetUserAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        var memoryCache = new FakeCache();

        memoryCache.Set(OidcRepository.CacheKeys.OIDC_CLIENT_TOKEN, "fake token");

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.NotNull(req.RequestUri);
                if (req.RequestUri.AbsolutePath.Contains($"realms/sample/users/{id}"))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Content = new StringContent("{\"error\": \"User not found\"}")
                    };
                }
                throw new Exception("Unexpected request");
            });
        var httpClient = new HttpClient(mockHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var configuration = new KeycloakConfiguration(
            realm: "sample",
            clientId: "admin-cli",
            clientSecret: "fack key",
            authority: "http://keycloak:8080/realms/sample"
        );

        var userRepository = new KeycloakUserRepository(memoryCache, configuration, httpClientFactory.Object);

        // Act
        var user = await userRepository.GetUserAsync(id);

        // Assert
        Assert.Null(user);
    }

    private static async Task<string> GetJsonFileByName(string name)
    {
        return await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Resources", name));
    }
}
