using FluentValidation;

namespace AssetAllocation.Api;

public class CreateCarMakeCommandValidator : AbstractValidator<CreateCarMakeCommand>
{   
    public CreateCarMakeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required");
    }
}
