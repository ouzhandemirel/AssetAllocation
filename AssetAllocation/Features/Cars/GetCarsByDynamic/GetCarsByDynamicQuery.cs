using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class GetCarsByDynamicQuery : IRequest<Result<GetListResponse<GetCarsByDynamicDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; }

    public GetCarsByDynamicQuery(PageRequest pageRequest, DynamicQuery dynamicQuery)
    {
        PageRequest = pageRequest;
        DynamicQuery = dynamicQuery;
    }

    public class GetCarsByDynamicQueryHandler : IRequestHandler<GetCarsByDynamicQuery, Result<GetListResponse<GetCarsByDynamicDto>>>
    {
        private readonly AssetAllocationDbContext _context;
        private readonly IMapper _mapper;

        public GetCarsByDynamicQueryHandler(AssetAllocationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<GetListResponse<GetCarsByDynamicDto>>> Handle(GetCarsByDynamicQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Cars
                .Include(c => c.Model).ThenInclude(c => c.Make)
                .AsNoTracking()
                .ToDynamicQuery(request.DynamicQuery);

            var cars = await _mapper
                .ProjectTo<GetCarsByDynamicDto>(query)
                .ToPaginatedListAsync(request.PageRequest.Index, request.PageRequest.Size, cancellationToken);

            GetListResponse<GetCarsByDynamicDto> response = _mapper.Map<GetListResponse<GetCarsByDynamicDto>>(cars);

            return Result<GetListResponse<GetCarsByDynamicDto>>.Ok(response);
        }
    }
}
