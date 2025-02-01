namespace Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;

public readonly struct KeycloakConfiguration
{
    public string Realm { get; init; }
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public string KeycloakUrl { get; init; }
}
