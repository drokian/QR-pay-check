using FluentValidation;

namespace QRPayCheck.Application.Tenants.Commands.UpdateTenant;

public sealed record UpdateTenantCommand(
    string Name,
    string? Phone,
    string? Email,
    string? TaxNumber
);

public sealed class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => x.Email is not null);
        RuleFor(x => x.TaxNumber).MaximumLength(20).When(x => x.TaxNumber is not null);
    }
}
