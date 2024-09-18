using FluentValidation;

namespace AssetAllocation.Api;

public class AllocateCarCommandValidator : AbstractValidator<AllocateCarCommand>
    {
        public AllocateCarCommandValidator()
        {
            RuleFor(x => x.CarId).NotEmpty().WithMessage("Car id is required");
            RuleFor(x => x.PersonId).NotEmpty().WithMessage("Person id is required");
            RuleFor(x => x.AllocationDate).NotEmpty().WithMessage("Allocation date is required");
        }
    }
