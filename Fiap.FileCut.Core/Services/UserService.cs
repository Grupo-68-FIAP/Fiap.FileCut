using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return await userRepository.CreateUserAsync(user, cancellationToken);
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await userRepository.DeleteUserAsync(id, cancellationToken);
        }

        public async Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await userRepository.GetUserAsync(userId, cancellationToken);
        }

        public Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return userRepository.UpdateUserAsync(user, cancellationToken);
        }
    }
}
