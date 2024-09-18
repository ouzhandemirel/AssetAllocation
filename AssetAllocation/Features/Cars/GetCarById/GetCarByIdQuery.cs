using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class GetCarByIdQuery : IRequest<Result<GetCarByIdResponse>>//, ICachableRequest
{
    public Guid Id { get; set; }

    public string CacheKey => $"GetCarByIdQuery_{Id}";
    //public string? GroupKey => throw new NotImplementedException();

    public bool BypassCache { get; }

    public TimeSpan? SlidingExpiration { get; }

    public GetCarByIdQuery(Guid id)
    {
        Id = id;
    }

    public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, Result<GetCarByIdResponse>>
    {
        private readonly AssetAllocationDbContext _context;
        private readonly IMapper _mapper;

        public GetCarByIdQueryHandler(AssetAllocationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<GetCarByIdResponse>> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Cars
                .Include(c => c.Model).ThenInclude(c => c.Make)
                .Where(c => c.Id == request.Id)
                .AsNoTracking();

            var response = await _mapper.ProjectTo<GetCarByIdResponse>(query).FirstOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result<GetCarByIdResponse>.NotFound("Car not found");
            }
            
            return Result<GetCarByIdResponse>.Ok(response);
        }
    }
}