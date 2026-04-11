using FluentValidation;

namespace QRPayCheck.Application.Tenants.Commands.CreateTenant;

public sealed record CreateTenantCommand(
    string Name,
    string Slug,
    string? Phone,
    string? Email,
    string? TaxNumber
);

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100)
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug yalnızca küçük harf, rakam ve tire içerebilir.");
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => x.Email is not null);
        RuleFor(x => x.TaxNumber).MaximumLength(20).When(x => x.TaxNumber is not null);
    }
}
