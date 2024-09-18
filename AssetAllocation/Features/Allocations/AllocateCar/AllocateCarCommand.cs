using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class AllocateCarCommand : IRequest<Result<CarAllocatedResponse>>
{
    public Guid CarId { get; set; }
    public Guid PersonId { get; set; }
    public DateTime AllocationDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public AllocateCarCommand(Guid carId, Guid personId, DateTime allocationDate, DateTime? returnDate)
    {
        CarId = carId;
        PersonId = personId;
        AllocationDate = allocationDate;
        ReturnDate = returnDate;
    }

    public class AllocateCarCommandHandler : IRequestHandler<AllocateCarCommand, Result<CarAllocatedResponse>>
    {
        private readonly AssetAllocationDbContext _context;
        private readonly IMapper _mapper;

        public AllocateCarCommandHandler(AssetAllocationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<CarAllocatedResponse>> Handle(AllocateCarCommand request, CancellationToken cancellationToken)
        {
            if (!await _context.Persons.AnyAsync(p => p.Id == request.PersonId, cancellationToken))
            {
                return Result<CarAllocatedResponse>.BadRequest("Person not found");
            }

            if (!await _context.Cars.AnyAsync(c => c.Id == request.CarId, cancellationToken))
            {
                return Result<CarAllocatedResponse>.BadRequest("Car not found");
            }

            if (await _context.CarAllocations.AnyAsync(ca => ca.CarId == request.CarId && ca.ReturnDate == null, cancellationToken))
            {
                return Result<CarAllocatedResponse>.BadRequest("Car is already allocated");
            }

            CarAllocation carAllocation = _mapper.Map<CarAllocation>(request);

            carAllocation.DomainEvents.Add(new CarAllocatedEvent(carAllocation));

            await _context.AddAsync(carAllocation, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            CarAllocatedResponse response = _mapper.Map<CarAllocatedResponse>(carAllocation);

            return Result<CarAllocatedResponse>.Ok(response);
        }
    }
}