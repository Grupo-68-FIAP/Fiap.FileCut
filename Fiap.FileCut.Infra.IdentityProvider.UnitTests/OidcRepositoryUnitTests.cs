using Castle.Components.DictionaryAdapter;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Headers;

namespace Fiap.FileCut.Infra.IdentityProvider.UnitTests;

public class OidcRepositoryUnitTests
{
    [Fact]
    public async Task GetHttpClient_WhenCalled_ReturnsHttpClient()
    {
        // Arrange
        var oidcConfigurationResponse = await IdentityProviderTestsHelpers.GetJsonFileByName("oidc-configuration-response.json");
        var oidcGetTokenResponse = await IdentityProviderTestsHelpers.GetJsonFileByName("oidc-get-tonken-response.json");

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
                else if (req.RequestUri.AbsolutePath.Contains("test"))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("ok"),
                    };
                }
                throw new Exception("Unexpected request");
            });

        var initialHttpClient = new HttpClient(mockHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(initialHttpClient);
        var memoryCache = new Mock<IMemoryCache>();
        memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
        var configuration = new OidcConfiguration("clientId", "clientSecret", "http://keycloak:8080/realms/sample");
        var oidcRepository = new FakeOidcRepository(httpClientFactory.Object, configuration, memoryCache.Object);
        // Act
        var httpClient = await oidcRepository.GetHttpClientAsync();
        var v = await httpClient.GetAsync("test");
        // Assert
        Assert.NotNull(v);
        Assert.Equal(HttpStatusCode.OK, v.StatusCode);
        Assert.Equal("ok", await v.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetAuthorizationBearerTokenAsync_WhenCached_ReturnsCachedValue()
    {
        // Arrange
        var oidcConfigurationResponse = await IdentityProviderTestsHelpers.GetJsonFileByName("oidc-configuration-response.json");
        var oidcGetTokenResponse = await IdentityProviderTestsHelpers.GetJsonFileByName("oidc-get-tonken-response.json");
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
                throw new Exception("Unexpected request");
            });
        var initialHttpClient = new HttpClient(mockHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(initialHttpClient);
        using var memoryCache = new FakeCache();
        var configuration = new OidcConfiguration("clientId", "clientSecret", "http://keycloak:8080/realms/sample");
        var oidcRepository = new FakeOidcRepository(httpClientFactory.Object, configuration, memoryCache);
        // Act
        var response = await oidcRepository.GetAuthorizationBearerTokenAsync();
        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Scheme);
        Assert.NotNull(response.Parameter);
    }

    [Fact]
    public async Task FetchDiscoveryDocumentAsync_WhenCached_ReturnsCachedValue()
    {
        // Arrange
        var oidcConfigurationResponse = await IdentityProviderTestsHelpers.GetJsonFileByName("oidc-configuration-response.json");
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
                    throw new Exception("Unexpected request");
                });
        var initialHttpClient = new HttpClient(mockHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(initialHttpClient);
        using var memoryCache = new FakeCache();
        var mockDiscoveryDocumentResponse = Mock.Of<DiscoveryDocumentResponse>();
        memoryCache.Set(OidcRepository.CacheKeys.OIDC_DISCOVERY_DOCUMENT, mockDiscoveryDocumentResponse);
        var configuration = new OidcConfiguration("clientId", "clientSecret", "http://keycloak:8080/realms/sample");
        var oidcRepository = new FakeOidcRepository(httpClientFactory.Object, configuration, memoryCache);
        // Act
        var response = await oidcRepository.FetchDiscoveryDocumentAsync();
        // Assert
        Assert.NotNull(response);
        Assert.Equal(mockDiscoveryDocumentResponse, response);
    }

    [Fact]
    public async Task FetchDiscoveryDocumentAsync_WhenNoCached_ReturnsValue()
    {
        // Arrange
        var oidcConfigurationResponse = await IdentityProviderTestsHelpers.GetJsonFileByName("oidc-configuration-response.json");
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
                    throw new Exception("Unexpected request");
                });
        var initialHttpClient = new HttpClient(mockHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(initialHttpClient);
        using var memoryCache = new FakeCache();
        var configuration = new OidcConfiguration("clientId", "clientSecret", "http://keycloak:8080/realms/sample");
        var oidcRepository = new FakeOidcRepository(httpClientFactory.Object, configuration, memoryCache);
        // Act
        var response = await oidcRepository.FetchDiscoveryDocumentAsync();
        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.TokenEndpoint);
    }

    [Fact]
    public async Task FetchDiscoveryDocumentAsync_WhenError_ThrowsException()
    {
        // Arrange
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
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("invalid")
                    };
                });
        var initialHttpClient = new HttpClient(mockHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(initialHttpClient);
        using var memoryCache = new FakeCache();
        var configuration = new OidcConfiguration("clientId", "clientSecret", "http://keycloak:8080/realms/sample");
        var oidcRepository = new FakeOidcRepository(httpClientFactory.Object, configuration, memoryCache);
        // Act
        // Assert
        await Assert.ThrowsAsync<InvalidProgramException>(() => oidcRepository.FetchDiscoveryDocumentAsync());
    }

    internal class FakeOidcRepository(IHttpClientFactory httpClientFactory, OidcConfiguration configuration, IMemoryCache memoryCache) : OidcRepository(httpClientFactory, configuration, memoryCache)
    {
        public new Task<DiscoveryDocumentResponse> FetchDiscoveryDocumentAsync(CancellationToken cancellationToken = default)
        {
            return base.FetchDiscoveryDocumentAsync(cancellationToken);
        }

        public new Task<AuthenticationHeaderValue> GetAuthorizationBearerTokenAsync(CancellationToken cancellationToken = default)
        {
            return base.GetAuthorizationBearerTokenAsync(cancellationToken);
        }

        public new async Task<HttpClient> GetHttpClientAsync(CancellationToken cancellationToken = default)
        {
            return await base.GetHttpClientAsync(cancellationToken);
        }
    }
}
