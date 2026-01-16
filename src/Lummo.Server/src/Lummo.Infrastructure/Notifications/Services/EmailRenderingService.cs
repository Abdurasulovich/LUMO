using FluentValidation;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Enums;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;

namespace Lummo.Infrastructure.Notifications.Services;

public class EmailRenderingService(IOptions<TemplateRenderingSettings> templateRenderingSettings,
    IValidator<EmailMessage> emailMessageValidator) : IEmailRenderingService
{
    private readonly IValidator<EmailMessage> _emailMessageValidator = emailMessageValidator;
    private readonly TemplateRenderingSettings _templateRenderingSettings = templateRenderingSettings.Value;

    public ValueTask<string> RenderAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        var validationResult = _emailMessageValidator.Validate(
            emailMessage,
            options => options.IncludeRuleSets(NotificationProcessingEvent.OnRendering.ToString())
            );

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        var placeholderRegex = new Regex(
            _templateRenderingSettings.PlaceholderRegexPattern,
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(_templateRenderingSettings.RegexMatchTimeoutInSeconds)
            );

        var placeholderValueRegex = new Regex(
            _templateRenderingSettings.PlaceholderValueRegexPattern,
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(_templateRenderingSettings.RegexMatchTimeoutInSeconds)
            );

        var matches = placeholderRegex.Matches(emailMessage.EmailTemplate.Content);
        if (matches.Any() && !emailMessage.Variables.Any())
            throw new InvalidOperationException("Variables for required for this template.");

        var templatePlaceholder = matches.Select(
            match =>
            {
                var placeholder = match.Value;
                var placeholderValue = placeholderValueRegex.Match(placeholder).Groups[1].Value;
                var valid = emailMessage.Variables.TryGetValue(placeholderValue, out var value);

                return new TemplatePlaceholder
                {
                    Placeholder = placeholder,
                    PlaceholderValue = placeholderValue,
                    Value = value,
                    IsValid = valid
                };
            }).ToList();
        ValidatePlaceholders(templatePlaceholder);
        var messageBuilder = new StringBuilder(emailMessage.EmailTemplate.Content);
        templatePlaceholder.ForEach(placeholder => messageBuilder.Replace(placeholder.Placeholder, placeholder.Value));

        var message = messageBuilder.ToString();
        emailMessage.Body = message;
        emailMessage.Subject = emailMessage.EmailTemplate.Subject;

        return ValueTask.FromResult(message);
    }

    private void ValidatePlaceholders(IEnumerable<TemplatePlaceholder> templatePlaceholders)
    {
        var missingPlaceholders = templatePlaceholders.Where(placeholder => !placeholder.IsValid)
            .Select(placeholder =>placeholder.PlaceholderValue)
            .ToList();

        if (!missingPlaceholders.Any()) return;

        var errorMessage = new StringBuilder();
        missingPlaceholders.ForEach(placeholderValue =>errorMessage.AppendLine(placeholderValue).Append(' '));

        throw new InvalidOperationException(
            $"Variable for given placeholders is not found - {string.Join(',', missingPlaceholders)}");
    }
}
