using AutoMapper;
using MediatR;

namespace AssetAllocation.Api;

public class CreateCarModelCommand : IRequest<Result<CarModelCreatedResponse>>
{
    public string? Name { get; set; }
    public Guid MakeId { get; set; }

    public CreateCarModelCommand(string? name, Guid makeId)
    {
        Name = name;
        MakeId = makeId;
    }

    public class CreateCarModelCommandHandler : IRequestHandler<CreateCarModelCommand, Result<CarModelCreatedResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public CreateCarModelCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }
        public async Task<Result<CarModelCreatedResponse>> Handle(CreateCarModelCommand request, CancellationToken cancellationToken)
        {
            if (await _assetAllocationDbContext.CarMakes.FindAsync(request.MakeId, cancellationToken) is null)
            {
                return Result<CarModelCreatedResponse>.BadRequest("Car make not found");
            }

            var carModel = new CarModel(request.Name!, request.MakeId);

            _assetAllocationDbContext.CarModels.Add(carModel);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<CarModelCreatedResponse>(carModel);

            return Result<CarModelCreatedResponse>.Ok(response);
        }
    }
}
