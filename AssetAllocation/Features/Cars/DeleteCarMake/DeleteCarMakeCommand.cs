using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class DeleteCarMakeCommand : IRequest<Result<CarMakeDeletedResponse>>
{
    public Guid CarMakeId { get; set; }

    public DeleteCarMakeCommand(Guid carMakeId)
    {
        CarMakeId = carMakeId;
    }

    public class DeleteCarMakeCommandHandler : IRequestHandler<DeleteCarMakeCommand, Result<CarMakeDeletedResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public DeleteCarMakeCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<CarMakeDeletedResponse>> Handle(DeleteCarMakeCommand request, CancellationToken cancellationToken)
        {
            var carMake = await _assetAllocationDbContext.CarMakes.FindAsync(request.CarMakeId, cancellationToken);

            if (carMake is null)
            {
                return Result<CarMakeDeletedResponse>.BadRequest("Car make not found");
            }

            if (await _assetAllocationDbContext.Cars.AnyAsync(c => c.Model.Make.Id == request.CarMakeId, cancellationToken))
            {
                return Result<CarMakeDeletedResponse>.BadRequest("Can't delete car make because it has car(s) depending on it");
            }

            await _assetAllocationDbContext.SoftRemoveAsync(carMake);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<CarMakeDeletedResponse>(carMake);

            return Result<CarMakeDeletedResponse>.Ok(response);
        }
    }
}