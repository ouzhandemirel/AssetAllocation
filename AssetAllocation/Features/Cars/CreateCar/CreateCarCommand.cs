using AssetAllocation.Api.Infrastructure.Persistence.Contexts;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace AssetAllocation.Api;

public class CreateCarCommand : IRequest<Result<CarCreatedResponse>>, ICacheRemoverRequest, ILoggableRequest
{
    public string? Plate { get; set; }
    public int Year { get; set; }
    public string? Color { get; set; }
    public int Mileage { get; set; }
    public Guid ModelId { get; set; }

    public string? CacheKey => null;
    public string GroupKey => "GetCars";

    public bool BypassCache { get; }

    public CreateCarCommand()
    {
        Plate = string.Empty;
        Color = string.Empty;
    }

    public CreateCarCommand(string? plate, int year, string? color, int mileage, Guid modelId)
    {
        Plate = plate;
        Year = year;
        Color = color;
        Mileage = mileage;
        ModelId = modelId;
    }

    public class CreteCarCommandHandler : IRequestHandler<CreateCarCommand, Result<CarCreatedResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public CreteCarCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<CarCreatedResponse>> Handle(CreateCarCommand request, CancellationToken cancellationToken)
        {
            if (await _assetAllocationDbContext.CarModels.FindAsync(request.ModelId, cancellationToken) == null)
            {
                return Result<CarCreatedResponse>.BadRequest("Car model not found");
            }

            if (_assetAllocationDbContext.Cars.Any(x => x.Plate == request.Plate))
            {
                return Result<CarCreatedResponse>.BadRequest("A car with the plate already exists");
            }

            Car car = new(request.Year, request.Color!, request.Plate!, request.Mileage, request.ModelId);
            car.Mileages = [new Mileage(car.Id, request.Mileage, DateTime.UtcNow)];

            await _assetAllocationDbContext.Cars.AddAsync(car, cancellationToken);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<CarCreatedResponse>(car);

            return Result<CarCreatedResponse>.Ok(response);
        }
    }
}