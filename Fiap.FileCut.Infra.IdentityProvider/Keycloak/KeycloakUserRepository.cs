using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace Fiap.FileCut.Infra.IdentityProvider.Keycloak;

public class KeycloakUserRepository(
    IMemoryCache memoryCache,
    KeycloakConfiguration configuration,
    IHttpClientFactory httpClientFactory) : OidcRepository(httpClientFactory, configuration, memoryCache), IUserRepository
{
    private readonly string KEYCLOAK_GET_USER_URL = $"admin/realms/{configuration.Realm}/users";

    public async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var httpClient = await GetHttpClientAsync(cancellationToken);
        var response = await httpClient.GetAsync($"{KEYCLOAK_GET_USER_URL}/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var user = await response.Content.ReadFromJsonAsync<KeycloakUser>(cancellationToken: cancellationToken);
        return ParseUser(user);
    }

    private static User ParseUser(KeycloakUser user)
    {
        return new User(user.Id, user.Username, user.Email, user.FirstName, user.LastName);
    }
}
