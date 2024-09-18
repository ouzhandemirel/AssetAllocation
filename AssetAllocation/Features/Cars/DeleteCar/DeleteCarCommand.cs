using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class DeleteCarCommand : IRequest<Result<CarDeletedResponse>>, ICacheRemoverRequest
{
    public Guid CarId { get; set; }

    public string? CacheKey => null!;

    public string GroupKey => "GetCars";

    public bool BypassCache { get; }

    public DeleteCarCommand(Guid carId)
    {
        CarId = carId;
    }


    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, Result<CarDeletedResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public DeleteCarCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<CarDeletedResponse>> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _assetAllocationDbContext.Cars.FindAsync(request.CarId, cancellationToken);

            if (car is null)
            {
                return Result<CarDeletedResponse>.BadRequest("Car not found");
            }


            if (await _assetAllocationDbContext.CarAllocations.AnyAsync(c => c.Car == car && c.ReturnDate == null, cancellationToken))
            {
                return Result<CarDeletedResponse>.BadRequest("Can't delete car because it is currently allocated to someone");
            }

            await _assetAllocationDbContext.SoftRemoveAsync(car);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<CarDeletedResponse>(car);

            return Result<CarDeletedResponse>.Ok(response);
        }
    }
}
