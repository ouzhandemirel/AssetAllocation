using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class DeallocateCarCommand : IRequest<Result<CarDeallocatedResponse>>, ITransactionalRequest
{
    public Guid AllocationId { get; set; }
    public DateTime ReturnDate { get; set; }
    
    public class DeallocateCarCommandHandler : IRequestHandler<DeallocateCarCommand, Result<CarDeallocatedResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public DeallocateCarCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<CarDeallocatedResponse>> Handle(DeallocateCarCommand request, CancellationToken cancellationToken)
        {
            var allocation = await _assetAllocationDbContext.CarAllocations
                .FindAsync(request.AllocationId, cancellationToken);

            if (allocation is null)
            {
                return Result<CarDeallocatedResponse>.BadRequest("Car allocation not found");
            }

            if (allocation.ReturnDate != null)
            {
                return Result<CarDeallocatedResponse>.BadRequest("Allocation is already deallocated");
            }

            if (request.ReturnDate < allocation.AllocationDate)
            {
                return Result<CarDeallocatedResponse>.BadRequest("Return date can't be before allocation date");
            }

            allocation.ReturnDate = DateTime.UtcNow;
            allocation.DomainEvents.Add(new CarDeallocatedEvent(allocation));

             _assetAllocationDbContext.CarAllocations.Update(allocation);
            await _assetAllocationDbContext.SaveChangesAsync();

            var response = _mapper.Map<CarDeallocatedResponse>(allocation);

            return Result<CarDeallocatedResponse>.Ok(response);
        }
    }
}