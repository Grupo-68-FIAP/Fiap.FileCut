namespace Fiap.FileCut.Infra.IdentityProvider.UnitTests;

internal static class IdentityProviderTestsHelpers
{

    public static async Task<string> GetJsonFileByName(string name)
    {
        return await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Resources", name));
    }
}