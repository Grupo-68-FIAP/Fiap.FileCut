using System.Net.Mail;

namespace Fiap.FileCut.Core.Objects;

public record struct User(Guid Id, string Username, string Email, string Name, string LastName)
{
    public readonly string FullName => $"{Name} {LastName}";
}
