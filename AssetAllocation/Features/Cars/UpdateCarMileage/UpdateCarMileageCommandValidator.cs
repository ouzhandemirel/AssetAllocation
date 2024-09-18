using FluentValidation;

namespace AssetAllocation.Api;

public class UpdateCarMileageCommandValidator : AbstractValidator<UpdateCarMileageCommand>
{
    public UpdateCarMileageCommandValidator()
    {
        RuleFor(x => x.CarId).NotEmpty().WithMessage("Car id required");

        RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0).WithMessage("Mileage value should be greater than or equal to 0");
    }
}