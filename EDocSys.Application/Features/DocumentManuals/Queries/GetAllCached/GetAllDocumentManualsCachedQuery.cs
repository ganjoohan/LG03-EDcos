using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.DocumentManuals.Queries.GetAllCached
{
    public class GetAllDocumentManualsCachedQuery : IRequest<Result<List<GetAllDocumentManualsCachedResponse>>>
    {
        public GetAllDocumentManualsCachedQuery()
        {
        }

        public class GetAllDocumentManualsCachedQueryHandler : IRequestHandler<GetAllDocumentManualsCachedQuery, Result<List<GetAllDocumentManualsCachedResponse>>>
        {
            private readonly IDocumentManualCacheRepository _documentManualCache;
            private readonly IMapper _mapper;

            public GetAllDocumentManualsCachedQueryHandler(IDocumentManualCacheRepository documentManualCache, IMapper mapper)
            {
                _documentManualCache = documentManualCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllDocumentManualsCachedResponse>>> Handle(GetAllDocumentManualsCachedQuery request, CancellationToken cancellationToken)
            {
                var documentManualList = await _documentManualCache.GetCachedListAsync();
                var mappedDocumentManuals = _mapper.Map<List<GetAllDocumentManualsCachedResponse>>(documentManualList);
                return Result<List<GetAllDocumentManualsCachedResponse>>.Success(mappedDocumentManuals);
            }
        }
    }
}
