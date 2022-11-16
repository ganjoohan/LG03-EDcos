using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.DocumentManualStatuses.Queries.GetAllCached
{
    public class GetAllDocumentManualStatusCachedQuery : IRequest<Result<List<GetAllDocumentManualStatusCachedResponse>>>
    {
        public GetAllDocumentManualStatusCachedQuery()
        {
        }

        public class GetAllDocumentManualStatusCachedQueryHandler : IRequestHandler<GetAllDocumentManualStatusCachedQuery, Result<List<GetAllDocumentManualStatusCachedResponse>>>
        {
            private readonly IDocumentManualStatusCacheRepository _documentManualStatusCache;
            private readonly IMapper _mapper;

            public GetAllDocumentManualStatusCachedQueryHandler(IDocumentManualStatusCacheRepository documentManualStatusCache, IMapper mapper)
            {
                _documentManualStatusCache = documentManualStatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllDocumentManualStatusCachedResponse>>> Handle(GetAllDocumentManualStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var documentManualStatusList = await _documentManualStatusCache.GetCachedListAsync();
                var mappedDocumentManualStatus = _mapper.Map<List<GetAllDocumentManualStatusCachedResponse>>(documentManualStatusList);
                return Result<List<GetAllDocumentManualStatusCachedResponse>>.Success(mappedDocumentManualStatus);
            }
        }
    }
}
