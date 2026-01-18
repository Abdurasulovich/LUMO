using Lummo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lummo.Persistence.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    ValueTask<RefreshToken> CreateAsync(
        RefreshToken refreshToken,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<RefreshToken?> GetByValueAsync(
        string refreshTokenValue,
        CancellationToken cancellationToken = default);

    ValueTask RemoveAsync(
        string refreshTokenValue,
        CancellationToken cancellationToken = default);
}
