using Agrivision.Backend.Domain.Entities.Identity;
using Agrivision.Backend.Domain.Enums.Identity;

namespace Agrivision.Backend.Application.Repositories.Identity;
public interface IOtpVerificationRepository
{
    Task<OtpVerification?> GetByIdAsync(Guid otpId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<OtpVerification>> GetExpiredOtpsAsync(DateTime olderThanUtc, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<OtpVerification>> GetAllActiveOtpsForUserAsync(string userId, OtpPurpose purpose, CancellationToken cancellationToken = default);
    
    Task AddAsync(OtpVerification otp, CancellationToken cancellationToken = default);
    
    Task<OtpVerification?> GetLatestActiveOtpAsync(string userId, OtpPurpose purpose, CancellationToken cancellationToken = default);
    
    Task<int> CountRecentOtpsAsync(string userId, OtpPurpose purpose, TimeSpan window, CancellationToken cancellationToken = default);

    Task UpdateAsync(OtpVerification otp, CancellationToken cancellationToken = default);
    
    Task UpdateRangeAsync(IEnumerable<OtpVerification> otps, CancellationToken cancellationToken = default);
}
