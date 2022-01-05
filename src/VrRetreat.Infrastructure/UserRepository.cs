using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Entities;

namespace VrRetreat.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<IVrRetreatUser?> GetUserByUsernameAsync(string username)
    {
        if (!_context.Users.Any(u => u.UserName == username))
            return Task.FromResult<IVrRetreatUser?>(null);

        var user = _context.Users.First(u => u.UserName == username);
        
        return Task.FromResult((IVrRetreatUser?)user);
    }

    public Task<bool> HasLinkedAccountByUsername(string username)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);

        if (user is null)
            return Task.FromResult(false);

        return Task.FromResult(user.IsVrChatAccountLinked);
    }

    public Task UpdateUserAsync(IVrRetreatUser user)
    {
        _context.Update(user);
        _context.SaveChanges();
        return Task.CompletedTask;
    }
}
