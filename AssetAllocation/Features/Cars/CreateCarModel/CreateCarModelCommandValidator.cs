using FluentValidation;

namespace AssetAllocation.Api;

public class CreateCarModelCommandValidator : AbstractValidator<CreateCarModelCommand>
{
    public CreateCarModelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.MakeId)
            .NotEmpty().WithMessage("Make id is required");
    }
}
