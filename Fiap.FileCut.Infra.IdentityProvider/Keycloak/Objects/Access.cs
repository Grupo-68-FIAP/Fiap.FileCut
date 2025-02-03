namespace Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;

internal readonly record struct Access
{
    public bool ManageGroupMembership { get; init; }
    public bool View { get; init; }
    public bool MapRoles { get; init; }
    public bool Impersonate { get; init; }
    public bool Manage { get; init; }
}
