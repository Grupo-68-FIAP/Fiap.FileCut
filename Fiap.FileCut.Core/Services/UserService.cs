using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await userRepository.GetUserAsync(userId, cancellationToken);
        }
    }
}
