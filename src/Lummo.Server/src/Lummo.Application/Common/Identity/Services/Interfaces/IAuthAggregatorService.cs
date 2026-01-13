using Lummo.Application.Common.Identity.Models;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IAuthAggregatorService
{
    ValueTask<bool> SignUpAsync(SignUpDetails signUpDetails, CancellationToken cancellationToken = default);
}
