using FluentValidation;

namespace AssetAllocation.Api;

public class CreateCarCommandValidator : AbstractValidator<CreateCarCommand>
{
    public CreateCarCommandValidator()
    {
        RuleFor(x => x.Plate)
            .Matches("^[0-9]{2}[A-Z]{1,3}[0-9]{2,4}$").WithMessage("Plate must be in correct format");

        RuleFor(x => x.Year)
            .InclusiveBetween(DateTime.UtcNow.Year - 30, DateTime.UtcNow.Year + 1).WithMessage("Year must be in between now and 30 years ago"); ;

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required");

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage must be greater than or equal to 0");


        RuleFor(x => x.ModelId)
            .NotEmpty().WithMessage("Model id is required");
    }
}