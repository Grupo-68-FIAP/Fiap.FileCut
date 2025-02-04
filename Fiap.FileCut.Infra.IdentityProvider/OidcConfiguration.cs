namespace Fiap.FileCut.Infra.IdentityProvider;

public class OidcConfiguration(string clientId, string clientSecret, string authority)
{
    public string ClientId { get; } = clientId;
    public string ClientSecret { get; } = clientSecret;
    public string Authority { get; } = authority;
}
