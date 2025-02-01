using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}
