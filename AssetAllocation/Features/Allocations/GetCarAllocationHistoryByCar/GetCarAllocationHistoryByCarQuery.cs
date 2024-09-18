using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class GetCarAllocationHistoryByCarQuery : IRequest<Result<List<GetCarAllocationHistoryByCarDto>>>
{
    public Guid CarId { get; }

    public GetCarAllocationHistoryByCarQuery(Guid carId)
    {
        CarId = carId;
    }

    public class GetCarAllocationHistoryByCarQueryHandler
    : IRequestHandler<GetCarAllocationHistoryByCarQuery, Result<List<GetCarAllocationHistoryByCarDto>>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;

        public GetCarAllocationHistoryByCarQueryHandler(AssetAllocationDbContext assetAllocationDbContext)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
        }

        public async Task<Result<List<GetCarAllocationHistoryByCarDto>>> Handle(GetCarAllocationHistoryByCarQuery request, CancellationToken cancellationToken)
        {
            var car = await _assetAllocationDbContext.Cars
                .Where(c => c.Id == request.CarId)
                .Include(c => c.Allocations).ThenInclude(c => c.Person)
                .FirstAsync(cancellationToken);

            if (car is null)
            {
                return Result<List<GetCarAllocationHistoryByCarDto>>.BadRequest($"Car not found");
            }

            var carAllocationHistory = car.Allocations
                .Select(x => new GetCarAllocationHistoryByCarDto
                {
                    AllocationId = x.Id,
                    PersonName = x.Person.Name,
                    PersonSurname = x.Person.Surname,
                    RegistrationNumber = x.Person.RegistrationNumber,
                    Department = x.Person.Department,
                    Title = x.Person.Title,
                    AllocationDate = x.AllocationDate,
                    ReturnDate = x.ReturnDate
                })
                .OrderByDescending(x => x.AllocationDate)
                .ToList();

            return Result<List<GetCarAllocationHistoryByCarDto>>.Ok(carAllocationHistory);
        }
    }
}