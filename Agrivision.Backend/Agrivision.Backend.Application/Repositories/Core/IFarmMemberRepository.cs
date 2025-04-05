using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Repositories.Core;
public interface IFarmMemberRepository
{
   // Task<FarmMember?> FindByEmailAndRoleName(string Email, FarmRoles roleName,CancellationToken cancellationToken);

    Task<FarmMember> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<FarmMember>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken);
    Task AddAsync(FarmMember farmMember, CancellationToken cancellationToken);
    Task DeleteAsync(FarmMember farmMember, CancellationToken cancellationToken);
    Task<bool> AnyAsync(Func<FarmMember, bool> predicate, CancellationToken cancellationToken);
    Task<IEnumerable<FarmMember>> GetUser_FarmRoles(string Email);
    Task<bool> DeleteListByEmails(List<string> emailsList, CancellationToken cancellationToken);
}
