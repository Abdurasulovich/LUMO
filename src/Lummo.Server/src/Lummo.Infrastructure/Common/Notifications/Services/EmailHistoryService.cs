using FluentValidation;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Persistence.Extensions;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lummo.Infrastructure.Common.Notifications.Services;

public class EmailHistoryService(IEmailHistoryRepository emailHistoryRepository, 
    IValidator<EmailHistory> emailhistoryValidator) : IEmailHistoryService
{
    private readonly IEmailHistoryRepository _emailHistoryRepository = emailHistoryRepository;
    private readonly IValidator<EmailHistory> _emailhistoryValidator = emailhistoryValidator;

    public async ValueTask<EmailHistory> CreateAsync(EmailHistory emailHistory, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var validationResult = _emailhistoryValidator.Validate(
            emailHistory,
            options => options.IncludeRuleSets(EntityEvent.OnCreate.ToString())
            );
        if(!validationResult.IsValid) 
            throw new ValidationException(validationResult.Errors);

        return await _emailHistoryRepository.CreateAsync(emailHistory, saveChanges, cancellationToken);
    }

    public IQueryable<EmailHistory> Get(Expression<Func<EmailHistory, bool>>? predicate = null, bool asNoTracking = false)
        => _emailHistoryRepository.Get(predicate, asNoTracking);
}
