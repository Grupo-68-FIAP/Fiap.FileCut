using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IUserService
{
    Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
