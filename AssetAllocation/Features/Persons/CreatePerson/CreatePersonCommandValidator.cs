using FluentValidation;

namespace AssetAllocation.Api;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required");

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateTime.UtcNow.AddYears(-18));

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");
    }
}
