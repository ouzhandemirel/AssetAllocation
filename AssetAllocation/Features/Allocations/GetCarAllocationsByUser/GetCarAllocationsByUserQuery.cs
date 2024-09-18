using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class GetCarAllocationsByUserQuery : IRequest<Result<List<GetCarAllocationsByUserDto>>>
{
    public Guid PersonId { get; set; }

    public GetCarAllocationsByUserQuery(Guid personId)
    {
        PersonId = personId;
    }

    public class GetCarAllocationsByUserQueryHandler : IRequestHandler<GetCarAllocationsByUserQuery, Result<List<GetCarAllocationsByUserDto>>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public GetCarAllocationsByUserQueryHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<List<GetCarAllocationsByUserDto>>> Handle(GetCarAllocationsByUserQuery request, CancellationToken cancellationToken)
        {
            if (await _assetAllocationDbContext.Persons.FindAsync(request.PersonId, cancellationToken) is null)
            {
                return Result<List<GetCarAllocationsByUserDto>>.BadRequest("Person not found");
            }

            IQueryable query = _assetAllocationDbContext.CarAllocations
                .Where(a => a.PersonId == request.PersonId).OrderBy(a => a.AllocationDate);

            var response = await _mapper.ProjectTo<GetCarAllocationsByUserDto>(query).ToListAsync(cancellationToken);

            return Result<List<GetCarAllocationsByUserDto>>.Ok(response);
        }
    }
}
