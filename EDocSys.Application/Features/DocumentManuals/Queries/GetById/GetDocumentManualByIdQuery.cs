using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.DocumentManuals.Queries.GetById
{
    public class GetDocumentManualByIdQuery : IRequest<Result<GetDocumentManualByIdResponse>>
    {
        public int Id { get; set; }

        public class GetDocumentManualByIdQueryHandler : IRequestHandler<GetDocumentManualByIdQuery, Result<GetDocumentManualByIdResponse>>
        {
            private readonly IDocumentManualCacheRepository _documentManualCache;
            private readonly IMapper _mapper;

            public GetDocumentManualByIdQueryHandler(IDocumentManualCacheRepository documentManualCache, IMapper mapper)
            {
                _documentManualCache = documentManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetDocumentManualByIdResponse>> Handle(GetDocumentManualByIdQuery query, CancellationToken cancellationToken)
            {
                var documentManual = await _documentManualCache.GetByIdAsync(query.Id);
                var mappedDocumentManual = _mapper.Map<GetDocumentManualByIdResponse>(documentManual);
                return Result<GetDocumentManualByIdResponse>.Success(mappedDocumentManual);
            }
        }
    }
}