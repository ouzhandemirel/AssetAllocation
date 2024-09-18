using AutoMapper;
using MediatR;

namespace AssetAllocation.Api;

public class CreateCarMakeCommand : IRequest<Result<CarMakeCreatedResponse>>
{
    public string? Name { get; set; }
    public string? Country { get; set; }

    public class CreateCarMakeCommandHandler : IRequestHandler<CreateCarMakeCommand, Result<CarMakeCreatedResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;
        public CreateCarMakeCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }
        public async Task<Result<CarMakeCreatedResponse>> Handle(CreateCarMakeCommand request, CancellationToken cancellationToken)
        {
            if (_assetAllocationDbContext.CarMakes.Any(cm => cm.Name == request.Name))
            {
                return Result<CarMakeCreatedResponse>.BadRequest("Car make already exists");
            }

            var carMake = new CarMake(request.Name!, request.Country!);

            await _assetAllocationDbContext.CarMakes.AddAsync(carMake, cancellationToken);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<CarMakeCreatedResponse>(carMake);

            return Result<CarMakeCreatedResponse>.Ok(response);
        }
    }
}