using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private const string StudentRoleName = "Student";

    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email.Trim().ToLower(), ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        var studentRole = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == StudentRoleName, ct);
        if (studentRole is null)
            throw new InvalidOperationException("Rolul Student nu este configurat.");

        await _dbContext.Users.AddAsync(user, ct);
        await _dbContext.UserRoles.AddAsync(new UserRole(user.Id, studentRole.Id), ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
