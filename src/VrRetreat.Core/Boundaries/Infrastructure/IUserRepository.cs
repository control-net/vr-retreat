using VrRetreat.Core.Entities;

namespace VrRetreat.Core.Boundaries.Infrastructure;

public interface IUserRepository
{
    Task<IVrRetreatUser?> GetUserByUsernameAsync(string username);
    Task UpdateUserAsync (IVrRetreatUser user);
    Task<bool> HasLinkedAccountByUsername(string username);
}
