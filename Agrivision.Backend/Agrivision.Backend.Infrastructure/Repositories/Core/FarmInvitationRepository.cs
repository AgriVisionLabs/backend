using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.InvitationTokenGenerator;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FarmInvitationRepository(CoreDbContext coreDbContext, IInvitationTokenGenerator invitationTokenGenerator) : IFarmInvitationRepository
{
    public async Task<IReadOnlyList<FarmInvitation>> AdminGetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations.ToListAsync(cancellationToken);
    }
    
    public async Task<FarmInvitation?> AdminGetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .FirstOrDefaultAsync(inv => inv.Id == id, cancellationToken);
    }

    public async Task<FarmInvitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .FirstOrDefaultAsync(inv => inv.Id == id && !inv.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<FarmInvitation>> AdminGetSentByUserIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .Where(inv => inv.CreatedById == id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmInvitation>> GetSentByUserIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .Where(inv => inv.CreatedById == id && !inv.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmInvitation>> AdminGetByInvitedEmailAsync(string invitedEmail, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .Where(inv => inv.InvitedEmail == invitedEmail)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmInvitation>> GetByInvitedEmailAsync(string invitedEmail, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .Where(inv => inv.InvitedEmail == invitedEmail && !inv.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmInvitation>> AdminGetByFarmIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .Where(inv => inv.FarmId == id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmInvitation>> GetByFarmIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .Where(inv => inv.FarmId == id && !inv.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<FarmInvitation?> AdminGetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .FirstOrDefaultAsync(inv => inv.Token == token, cancellationToken);
    }

    public async Task<FarmInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .FirstOrDefaultAsync(inv => inv.Token == token && !inv.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid farmId, string invitedEmail, CancellationToken cancellationToken = default)
    {
        var invitation = await coreDbContext.FarmInvitations
            .FirstOrDefaultAsync(inv => inv.FarmId == farmId && inv.InvitedEmail == invitedEmail && !inv.IsDeleted,
                cancellationToken);

        return invitation is not null;
    }

    public async Task<bool> ResendInvitationAsync(Guid farmId, string invitedEmail, CancellationToken cancellationToken = default)
    {
        var invitation = await coreDbContext.FarmInvitations
            .FirstOrDefaultAsync(inv => 
                    inv.FarmId == farmId &&
                    inv.InvitedEmail == invitedEmail &&
                    !inv.IsDeleted &&
                    !inv.IsAccepted,
                cancellationToken);

        if (invitation is null)
            return false;
        
        invitation.Token = invitationTokenGenerator.GenerateToken();
        invitation.ExpiresAt = DateTime.UtcNow.AddDays(7);
        invitation.UpdatedOn = DateTime.UtcNow;

        coreDbContext.FarmInvitations.Update(invitation);
        await coreDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task AddAsync(FarmInvitation invitation, CancellationToken cancellationToken = default)
    {
        await coreDbContext.FarmInvitations.AddAsync(invitation, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FarmInvitation invitation, CancellationToken cancellationToken = default)
    {
        coreDbContext.FarmInvitations.Update(invitation);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(FarmInvitation invitation, CancellationToken cancellationToken = default)
    {
        coreDbContext.FarmInvitations.Remove(invitation);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsTokenValidAsync(string invitedEmail, string token, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmInvitations
            .AnyAsync(
                inv => inv.InvitedEmail == invitedEmail && inv.Token == token && !inv.IsDeleted && !inv.IsAccepted &&
                       inv.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task<int> CleanupExpiredInvitationsAsync(CancellationToken cancellationToken = default)
    {
        var expired = await coreDbContext.FarmInvitations
            .Where(inv => !inv.IsDeleted && !inv.IsAccepted && inv.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var invitation in expired)
        {
            invitation.IsDeleted = true;
            invitation.DeletedOn = DateTime.UtcNow;
        }
        
        coreDbContext.FarmInvitations.UpdateRange(expired);
        return await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}