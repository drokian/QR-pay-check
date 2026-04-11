using FluentValidation;

namespace QRPayCheck.Application.Branches.Commands.UpdateBranch;

public sealed record UpdateBranchCommand(
    Guid Id,
    string Name,
    string? Address,
    string? City,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    string? Phone,
    bool IsActive
);

public sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.City).MaximumLength(100).When(x => x.City is not null);
        RuleFor(x => x.District).MaximumLength(100).When(x => x.District is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
    }
}
