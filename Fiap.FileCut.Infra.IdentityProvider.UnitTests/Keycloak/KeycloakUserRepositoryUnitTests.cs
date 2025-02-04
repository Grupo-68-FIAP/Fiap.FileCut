using Fiap.FileCut.Infra.IdentityProvider.Keycloak;
using Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

namespace Fiap.FileCut.Infra.IdentityProvider.UnitTests.Keycloak;

public class KeycloakUserRepositoryUnitTests
{
    [Fact]
    public async Task GetUserAsync_WhenCalled_ReturnsUser()
    {
        // Arrange
        var id = new Guid("9b5199a0-7436-4168-80b3-6966ef60c1f2");

        using var memoryCache = new FakeCache();
        memoryCache.Set(OidcRepository.CacheKeys.OIDC_CLIENT_TOKEN, "fake token");

        var oidcGetUserResponse = await IdentityProviderTestsHelpers.GetJsonFileByName("keycloak-get-user-response.json");
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

        using var memoryCache = new FakeCache();
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

    [Fact]
    public void KeycloakConfiguration_WhenEnvironmentVariablesAreSet_ReturnsInstance()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENID_AUDIENCE", "TestValue");
        Environment.SetEnvironmentVariable("OPENID_SECRET", "TestValue");
        Environment.SetEnvironmentVariable("OPENID_AUTHORITY", "http://test:100/realms/test");

        // Act
        var configuration = new KeycloakConfiguration();
        // Assert
        Assert.NotNull(configuration);
        Assert.Equal("test", configuration.Realm);
    }

    [Fact]
    public void KeycloakConfiguration_WhenConfigurationIsSet_ReturnsInstance()
    {
        // Arrange
        var vars = new Dictionary<string, string?>
        {
            ["OpenIdAudiance"] = "TestValue",
            ["OpenIdSecret"] = "TestValue",
            ["OpenIdAuthority"] = "http://test:100/realms/test",
            ["KeycloakRealm"] = "test"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(vars)
            .Build();
        // Act
        var keycloakConfiguration = new KeycloakConfiguration(configuration);
        // Assert
        Assert.NotNull(keycloakConfiguration);
        Assert.Equal("test", keycloakConfiguration.Realm);
    }

    [Theory]
    [InlineData("OPENID_AUDIENCE")]
    [InlineData("OPENID_SECRET")]
    [InlineData("OPENID_AUTHORITY")]
    [InlineData("KEYCLOAK_RELM")]
    public void KeycloakConfiguration_WhenEnvironmentVariablesAreNotSet_ThrowsException(string varToSetNull)
    {
        // Arrange
        void Set(string varName, string value)
        {
            if (varName == varToSetNull)
                Environment.SetEnvironmentVariable(varName, null);
            else
                Environment.SetEnvironmentVariable(varName, value);
        }
        Set("OPENID_AUDIENCE", "TestValue");
        Set("OPENID_SECRET", "TestValue");
        Set("OPENID_AUTHORITY", "TestValue");
        Set("KEYCLOAK_RELM", "TestValue");
        try
        {
            // Act
            var instance = new KeycloakConfiguration();
            Assert.Null(instance);
            Assert.Fail("Should have thrown an exception");
        }
        catch (MissingFieldException)
        {
            // Assert
            Assert.True(true);
        }
    }

    [Theory]
    [InlineData("OpenIdAudiance")]
    [InlineData("OpenIdSecret")]
    [InlineData("OpenIdAuthority")]
    [InlineData("KeycloakRealm")]
    public void KeycloakConfiguration_WhenConfigurationIsNotSet_ThrowsException(string varToSetNull)
    {
        // Arrange
        var vars = new Dictionary<string, string?>
        {
            ["OpenIdAudiance"] = "TestValue",
            ["OpenIdSecret"] = "TestValue",
            ["OpenIdAuthority"] = "TestValue",
            ["KeycloakRealm"] = "TestValue"
        };
        void Set(string varName, string value)
        {
            if (varName == varToSetNull)
                vars[varName] = null;
            else
                vars[varName] = value;
        }
        Set("OpenIdAudiance", "TestValue");
        Set("OpenIdSecret", "TestValue");
        Set("OpenIdAuthority", "TestValue");
        Set("KeycloakRealm", "TestValue");
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(vars)
            .Build();
        try
        {
            // Act
            var instance = new KeycloakConfiguration(configuration);
            Assert.Null(instance);
            Assert.Fail("Should have thrown an exception");
        }
        catch (MissingFieldException)
        {
            // Assert
            Assert.True(true);
        }
    }
}
