using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AssetAllocation.Api;

public class GetCarsQuery : IRequest<Result<GetListResponse<GetCarsDto>>>, ICachableRequest, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string CacheKey => $"GetCarsQuery_{PageRequest.Index}_{PageRequest.Size}";
    public string? GroupKey => "GetCars";

    public bool BypassCache { get; }

    public TimeSpan? SlidingExpiration { get; }

    public GetCarsQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public string[] Roles => [PersonOperationClaims.Admin];

    public class GetCarsQueryHandler : IRequestHandler<GetCarsQuery, Result<GetListResponse<GetCarsDto>>>
    {
        private readonly AssetAllocationDbContext _context;
        private readonly IMapper _mapper;

        public GetCarsQueryHandler(AssetAllocationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<GetListResponse<GetCarsDto>>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Cars
                .Include(c => c.Model).ThenInclude(c => c.Make)
                .OrderBy(c => c.CreatedDate)
                .AsNoTracking();

            var cars = await _mapper
                .ProjectTo<GetCarsDto>(query)
                .ToPaginatedListAsync(request.PageRequest.Index, request.PageRequest.Size, cancellationToken);

            GetListResponse<GetCarsDto> response = _mapper.Map<GetListResponse<GetCarsDto>>(cars);

            return Result<GetListResponse<GetCarsDto>>.Ok(response);
        }
    }
}