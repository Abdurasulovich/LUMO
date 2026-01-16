using FluentValidation;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Persistence.Extensions;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lummo.Infrastructure.Notifications.Services;

public class EmailTemplateService(IEmailTemplateRepository emailTemplateRepository,
    IValidator<EmailTemplate> emailTemplateValidator) : IEmailTemplateService
{
    private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
    private readonly IValidator<EmailTemplate> _emailTemplateValidator = emailTemplateValidator;

    public ValueTask<EmailTemplate> CreateAsync(EmailTemplate emailTemplate, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var validationResult = _emailTemplateValidator.Validate(emailTemplate);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        return _emailTemplateRepository.CreateAsync(emailTemplate, saveChanges, cancellationToken);
    }

    public async ValueTask<IList<EmailTemplate>> GetByFilterAsync(FilterPagination filterPagination, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        return await Get(asNoTracking: asNoTracking).ApplyPagination(filterPagination).ToListAsync(cancellationToken);
    }

    public async ValueTask<EmailTemplate?> GetByTypeAsync(NotificationTemplateType templateType, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        return await _emailTemplateRepository.Get(template => template.TemplateType == templateType, asNoTracking)
            .SingleOrDefaultAsync(cancellationToken);
    }

    private IQueryable<EmailTemplate> Get(Expression<Func<EmailTemplate, bool>>? predicate = default,
        bool asNoTracking = false)
    {
        return _emailTemplateRepository.Get(predicate, asNoTracking);
    }
}
