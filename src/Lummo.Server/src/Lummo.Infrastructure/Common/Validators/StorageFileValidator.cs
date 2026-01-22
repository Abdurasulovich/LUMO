using FluentValidation;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.Common.Validators;

public class StorageFileValidator : AbstractValidator<StorageFile>
{
    public StorageFileValidator()
    {
        RuleSet(
            EntityEvent.OnGet.ToString(),
            () =>
            {
                RuleFor(storageFile => storageFile.Id).NotEqual(Guid.Empty);

                RuleFor(storageFile => storageFile.FileName).NotEmpty().MaximumLength(64);

                RuleFor(storageFile => storageFile.FileType).NotEmpty().IsInEnum();
            });
    }
}
