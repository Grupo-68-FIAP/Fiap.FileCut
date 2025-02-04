using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
}
