using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class UpdateCarMileageCommand : IRequest<Result<CarMileageUpdatedResponse>>, ICacheRemoverRequest/*, ITransactionalRequest*/
{
    public Guid CarId { get; set; }
    public int Mileage { get; set; }

    public string? CacheKey => null;

    public string GroupKey => "GetCars";

    public bool BypassCache { get; }

    public UpdateCarMileageCommand(Guid carId, int mileage)
    {
        CarId = carId;
        Mileage = mileage;
    }

    public class UpdateCarMileageCommandHandler : IRequestHandler<UpdateCarMileageCommand, Result<CarMileageUpdatedResponse>>
    {
        private readonly AssetAllocationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateCarMileageCommandHandler(AssetAllocationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<CarMileageUpdatedResponse>> Handle(UpdateCarMileageCommand request, CancellationToken cancellationToken)
        {
            var car = await _context.Cars
                .Include(c => c.Mileages)
                .FirstOrDefaultAsync(c => c.Id == request.CarId, cancellationToken);

            if (car is null)
            {
                return Result<CarMileageUpdatedResponse>.BadRequest("Car not found");
            }

            if (request.Mileage <= car.CurrentMileage)
            {
                return Result<CarMileageUpdatedResponse>.BadRequest("New mileage value must be greater than the current value");
            }

            if (car.Mileages.MaxBy(m => m.Date)?.Date.AddDays(30) is DateTime date && date > DateTime.UtcNow)
            {
                return Result<CarMileageUpdatedResponse>.BadRequest("New mileage must be set at least 30 days after the last mileage record");
            }

            car.SetMileage(request.Mileage);

            _context.Update(car);
            await _context.SaveChangesAsync(cancellationToken);
            
            CarMileageUpdatedResponse response = _mapper.Map<CarMileageUpdatedResponse>(car);

            return Result<CarMileageUpdatedResponse>.Ok(response);
        }
    }
}
