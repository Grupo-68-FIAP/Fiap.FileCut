using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Fiap.FileCut.Infra.IdentityProvider.Keycloak;

public partial class KeycloakUserRepository : IUserRepository
{
    public readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public KeycloakUserRepository(
        KeycloakConfiguration configuration, 
        IHttpClientFactory httpClientFactory, 
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(configuration.KeycloakUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _httpContextAccessor = httpContextAccessor;
    }

    private AuthenticationHeaderValue GetAuthorizationBearerToken()
    {
        // Recuperar o token do contexto HTTP (assim como o JwtBearer)
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token de autenticação não encontrado.");

        return new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var endpoint = $"admin/realms/sample/users/{id}";

        _httpClient.DefaultRequestHeaders.Authorization = GetAuthorizationBearerToken();
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var user = await response.Content.ReadFromJsonAsync<KeycloakUser>(cancellationToken: cancellationToken);
        return ParseUser(user);
    }

    public Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static User ParseUser(KeycloakUser user)
    {
        return new User(user.Id, user.Username, user.Email, user.FirstName, user.LastName);
    }
}
