using Lummo.Domain.Enums;

namespace Lummo.Application.Common.Verifications.Services.Interfaces;

public interface IVerificationCodeService
{
    ValueTask<VerificationType?> GetVerificationTypeAsync(string code, CancellationToken cancellationToken = default);
}
