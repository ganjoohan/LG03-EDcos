using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.DocumentManuals.Queries.GetByDOCPNo
{
    public class GetDocumentManualByDOCNoQuery : IRequest<Result<GetDocumentManualByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetDocumentManualByDOCNoQueryHandler : IRequestHandler<GetDocumentManualByDOCNoQuery, Result<GetDocumentManualByDOCNoResponse>>
        {
            private readonly IDocumentManualCacheRepository _documentManualCache;
            private readonly IMapper _mapper;

            public GetDocumentManualByDOCNoQueryHandler(IDocumentManualCacheRepository documentManualCache, IMapper mapper)
            {
                _documentManualCache = documentManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetDocumentManualByDOCNoResponse>> Handle(GetDocumentManualByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var documentManual = await _documentManualCache.GetByDOCNoAsync(query.docno);
                var mappedDocumentManual = _mapper.Map<GetDocumentManualByDOCNoResponse>(documentManual);
                return Result<GetDocumentManualByDOCNoResponse>.Success(mappedDocumentManual);
            }
        }
    }
}