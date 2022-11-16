using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.LabAccreditationManuals.Queries.GetByDOCPNo
{
    public class GetLabAccreditationManualByDOCNoQuery : IRequest<Result<GetLabAccreditationManualByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetLabAccreditationManualByDOCNoQueryHandler : IRequestHandler<GetLabAccreditationManualByDOCNoQuery, Result<GetLabAccreditationManualByDOCNoResponse>>
        {
            private readonly ILabAccreditationManualCacheRepository _labAccreditationManualCache;
            private readonly IMapper _mapper;

            public GetLabAccreditationManualByDOCNoQueryHandler(ILabAccreditationManualCacheRepository labAccreditationManualCache, IMapper mapper)
            {
                _labAccreditationManualCache = labAccreditationManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetLabAccreditationManualByDOCNoResponse>> Handle(GetLabAccreditationManualByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var labAccreditationManual = await _labAccreditationManualCache.GetByDOCNoAsync(query.docno);
                var mappedLabAccreditationManual = _mapper.Map<GetLabAccreditationManualByDOCNoResponse>(labAccreditationManual);
                return Result<GetLabAccreditationManualByDOCNoResponse>.Success(mappedLabAccreditationManual);
            }
        }
    }
}