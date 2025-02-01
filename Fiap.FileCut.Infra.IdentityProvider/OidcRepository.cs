using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Fiap.FileCut.Infra.IdentityProvider;

public abstract class OidcRepository(
    IHttpClientFactory httpClientFactory,
    OidcConfiguration configuration,
    IMemoryCache memoryCache)
{

    protected IHttpClientFactory httpClientFactory = httpClientFactory;
    protected OidcConfiguration configuration = configuration;
    protected IMemoryCache memoryCache = memoryCache;

    protected async Task<HttpClient> GetHttpClient(CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient();
        var autority = new Uri(configuration.Authority);
        httpClient.BaseAddress = new UriBuilder(autority) { Path = "" }.Uri;
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = await GetAuthorizationBearerTokenAsync(cancellationToken);
        return httpClient;
    }

    protected async Task<AuthenticationHeaderValue> GetAuthorizationBearerTokenAsync(CancellationToken cancellationToken = default)
    {
        if (memoryCache.TryGetValue(CacheKeys.OIDC_CLIENT_TOKEN, out string? cachedToken))
            return new AuthenticationHeaderValue("Bearer", cachedToken);

        var disco = await FetchDiscoveryDocumentAsync(cancellationToken);
        var httpClient = httpClientFactory.CreateClient();
        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = configuration.ClientId,
            ClientSecret = configuration.ClientSecret,
            Scope = "openid"
        }, cancellationToken);

        if (tokenResponse.IsError)
            throw new InvalidProgramException($"Erro ao obter token: {tokenResponse.Error}");

        var expirationTime = DateTimeOffset.Now.AddSeconds(tokenResponse.ExpiresIn - 10);
        memoryCache.Set(CacheKeys.OIDC_CLIENT_TOKEN, tokenResponse.AccessToken, expirationTime);

        return new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
    }

    protected async Task<DiscoveryDocumentResponse> FetchDiscoveryDocumentAsync(CancellationToken cancellationToken = default)
    {
        if (memoryCache.TryGetValue(CacheKeys.OIDC_DISCOVERY_DOCUMENT, out DiscoveryDocumentResponse? cached))
        {
            ArgumentNullException.ThrowIfNull(cached);
            return cached;
        }
        var httpClient = httpClientFactory.CreateClient();
        var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = configuration.Authority,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        }, cancellationToken);

        if (disco.IsError)
            throw new InvalidProgramException($"Erro ao obter dados do provedor OIDC: {disco.Error}");

        return memoryCache.Set(CacheKeys.OIDC_DISCOVERY_DOCUMENT, disco);
    }

    public static class CacheKeys
    {
        public readonly static string OIDC_CLIENT_TOKEN = "oidc.client_token";
        public readonly static string OIDC_DISCOVERY_DOCUMENT = "oidc.discovery_document";
    }
}
