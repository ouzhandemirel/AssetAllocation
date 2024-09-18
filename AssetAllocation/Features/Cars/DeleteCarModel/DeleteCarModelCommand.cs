using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class DeleteCarModelCommand : IRequest<Result<CarModelDeletedResponse>>
{
    public Guid ModelId { get; set; }

    public DeleteCarModelCommand(Guid modelId)
    {
        ModelId = modelId;
    }
    
    public class DeleteCarModelCommandHandler : IRequestHandler<DeleteCarModelCommand, Result<CarModelDeletedResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public DeleteCarModelCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<CarModelDeletedResponse>> Handle(DeleteCarModelCommand request, CancellationToken cancellationToken)
        {
            var carModel = await _assetAllocationDbContext.CarModels.FindAsync(request.ModelId, cancellationToken);

            if (carModel is null)
            {
                return Result<CarModelDeletedResponse>.BadRequest("Car model not found");
            }

            if (await _assetAllocationDbContext.Cars.AnyAsync(c => c.ModelId == request.ModelId, cancellationToken))
            {
                return Result<CarModelDeletedResponse>.BadRequest("Can't delete car model because it has car(s) depending on it");
            }

            await _assetAllocationDbContext.SoftRemoveAsync(carModel);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<CarModelDeletedResponse>(carModel);

            return Result<CarModelDeletedResponse>.Ok(response);
        }
    }
}
