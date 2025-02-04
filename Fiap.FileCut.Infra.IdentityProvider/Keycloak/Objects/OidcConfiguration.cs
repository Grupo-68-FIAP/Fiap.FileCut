using Microsoft.Extensions.Configuration;

namespace Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;

public class KeycloakConfiguration : OidcConfiguration
{
    public KeycloakConfiguration()
        : base(
            clientId: Environment.GetEnvironmentVariable("OPENID_AUDIENCE")
                ?? throw new MissingFieldException("Need to configure a OpenId Audience"),
            clientSecret: Environment.GetEnvironmentVariable("OPENID_SECRET")
                ?? throw new MissingFieldException("Need to configure a OpenId Secret"),
            authority: Environment.GetEnvironmentVariable("OPENID_AUTHORITY")
                ?? throw new MissingFieldException("Need to configure a OpenId Authority"))
    {
        Realm = RealmResolve(Environment.GetEnvironmentVariable("KEYCLOAK_RELM"), Authority);
    }

    public KeycloakConfiguration(IConfiguration configuration)
        : base(
            clientId: configuration["OpenIdAudiance"]
                ?? throw new MissingFieldException("Need to configure a OpenId Audience"),
            clientSecret: configuration["OpenIdSecret"]
                ?? throw new MissingFieldException("Need to configure a OpenId Secret"),
            authority: configuration["OpenIdAuthority"]
                ?? throw new MissingFieldException("Need to configure a OpenId Authority"))
    {
        Realm = RealmResolve(configuration["KeycloakRealm"], Authority);
    }

    public KeycloakConfiguration(string clientId, string clientSecret, string authority, string realm)
        : base(clientId, clientSecret, authority)
    {
        Realm = realm;
    }

    public readonly string Realm;

    private static string RealmResolve(string? realm, string authority)
    {
        if (!string.IsNullOrWhiteSpace(realm))
            return realm;

        if (authority.Contains("/realms/"))
        {
            var parts = authority.Split("/realms/");
            if (parts.Length > 1)
                return parts[1];
        }

        throw new MissingFieldException("Need to configure a Keycloak Realm");
    }
}
