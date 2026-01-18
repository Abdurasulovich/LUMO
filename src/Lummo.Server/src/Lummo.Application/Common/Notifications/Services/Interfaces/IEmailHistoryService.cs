using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using System.Linq.Expressions;

namespace Lummo.Application.Common.Notifications.Services.Interfaces;

public interface IEmailHistoryService
{

    IQueryable<EmailHistory> Get(Expression<Func<EmailHistory, bool>>? predicate = default,
        bool asNoTracking = false);

    ValueTask<EmailHistory> CreateAsync(
        EmailHistory emailHistory,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
