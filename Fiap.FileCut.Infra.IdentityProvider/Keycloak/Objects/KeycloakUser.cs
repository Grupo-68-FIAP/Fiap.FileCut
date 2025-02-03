namespace Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;

internal readonly record struct KeycloakUser
{
    public Guid Id { get; init; }
    public long CreatedTimestamp { get; init; }
    public string Username { get; init; }
    public bool Enabled { get; init; }
    public bool TOTP { get; init; }
    public bool EmailVerified { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public IEnumerable<string> DisableableCredentialTypes { get; init; }
    public IEnumerable<string> RequiredActions { get; init; }
    public long NotBefore { get; init; }
    public Access Access { get; init; }
}
