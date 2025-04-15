using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFarmInvitationRepository
{
    // admin get all
    Task<IReadOnlyList<FarmInvitation>> AdminGetAllAsync(CancellationToken cancellationToken = default);
    Task<FarmInvitation?> AdminGetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    // get by id
    Task<FarmInvitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    // admin get by user id (list)
    Task<IReadOnlyList<FarmInvitation>> AdminGetSentByUserIdAsync(string id,
        CancellationToken cancellationToken = default);
    // get by user id (list)
    Task<IReadOnlyList<FarmInvitation>> GetSentByUserIdAsync(string id,
        CancellationToken cancellationToken = default);
    // AdminGetByInvitedEmail
    Task<IReadOnlyList<FarmInvitation>> AdminGetByInvitedEmailAsync(string invitedEmail,
        CancellationToken cancellationToken = default);
    // GetByInvitedEmail
    Task<IReadOnlyList<FarmInvitation>> GetByInvitedEmailAsync(string invitedEmail, CancellationToken cancellationToken = default);
    // admin get by farm id (list)
    Task<IReadOnlyList<FarmInvitation>> AdminGetByFarmIdAsync(Guid id, CancellationToken cancellationToken = default);
    // get by farm 
    Task<IReadOnlyList<FarmInvitation>> GetByFarmIdAsync(Guid id, CancellationToken cancellationToken = default);
    // admin get by token 
    Task<FarmInvitation?> AdminGetByTokenAsync(string token, CancellationToken cancellationToken = default);
    // get by token
    Task<FarmInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    // exists
    Task<bool> ExistsAsync(Guid farmId, string invitedEmail, CancellationToken cancellationToken = default);
    // Resend 
    Task<bool> ResendInvitationAsync(Guid farmId, string invitedEmail, CancellationToken cancellationToken = default);
    // add 
    Task AddAsync(FarmInvitation invitation, CancellationToken cancellationToken = default);
    // update
    Task UpdateAsync(FarmInvitation invitation, CancellationToken cancellationToken = default);
    // remove
    Task RemoveAsync(FarmInvitation invitation, CancellationToken cancellationToken = default);
    // confirm token
    Task<bool> IsTokenValidAsync(string invitedEmail, string token, CancellationToken cancellationToken = default);
    // cleanup expired
    Task<int> CleanupExpiredInvitationsAsync(CancellationToken cancellationToken = default);
}