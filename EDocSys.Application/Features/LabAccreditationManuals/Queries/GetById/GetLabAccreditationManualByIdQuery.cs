using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.LabAccreditationManuals.Queries.GetById
{
    public class GetLabAccreditationManualByIdQuery : IRequest<Result<GetLabAccreditationManualByIdResponse>>
    {
        public int Id { get; set; }

        public class GetLabAccreditationManualByIdQueryHandler : IRequestHandler<GetLabAccreditationManualByIdQuery, Result<GetLabAccreditationManualByIdResponse>>
        {
            private readonly ILabAccreditationManualCacheRepository _labAccreditationManualCache;
            private readonly IMapper _mapper;

            public GetLabAccreditationManualByIdQueryHandler(ILabAccreditationManualCacheRepository labAccreditationManualCache, IMapper mapper)
            {
                _labAccreditationManualCache = labAccreditationManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetLabAccreditationManualByIdResponse>> Handle(GetLabAccreditationManualByIdQuery query, CancellationToken cancellationToken)
            {
                var labAccreditationManual = await _labAccreditationManualCache.GetByIdAsync(query.Id);
                var mappedLabAccreditationManual = _mapper.Map<GetLabAccreditationManualByIdResponse>(labAccreditationManual);
                return Result<GetLabAccreditationManualByIdResponse>.Success(mappedLabAccreditationManual);
            }
        }
    }
}