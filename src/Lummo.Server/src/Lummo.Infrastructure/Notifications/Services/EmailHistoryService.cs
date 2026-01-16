using FluentValidation;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Persistence.Extensions;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Infrastructure.Notifications.Services;

public class EmailHistoryService(IEmailHistoryRepository emailHistoryRepository, 
    IValidator<EmailHistory> emailhistoryValidator) : IEmailHistoryService
{
    private readonly IEmailHistoryRepository _emailHistoryRepository = emailHistoryRepository;
    private readonly IValidator<EmailHistory> _emailhistoryValidator = emailhistoryValidator;

    public async ValueTask<IList<EmailHistory>> GetByFilterAsync(FilterPagination paginationOptions, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        return await _emailHistoryRepository.Get().ApplyPagination(paginationOptions).ToListAsync(cancellationToken);
    }

    public async ValueTask<EmailHistory> CreateAsync(EmailHistory emailHistory, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var validationResult = await _emailhistoryValidator.ValidateAsync(
            emailHistory,
            options => options.IncludeRuleSets(EntityEvent.OnCreate.ToString()),
            cancellationToken
            );
        if(!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
        return await _emailHistoryRepository.CreateAsync(emailHistory, saveChanges, cancellationToken);
    }
}
