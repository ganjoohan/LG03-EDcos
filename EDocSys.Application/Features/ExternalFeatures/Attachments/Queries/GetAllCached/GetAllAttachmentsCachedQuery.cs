using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories;

namespace EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetAllCached
{
    public class GetAllAttachmentsCachedQuery : IRequest<Result<List<GetAllAttachmentsCachedResponse>>>
    {
        public GetAllAttachmentsCachedQuery()
        {
        }
    }

    public class GetAllAttachmentsCachedQueryHandler : IRequestHandler<GetAllAttachmentsCachedQuery, Result<List<GetAllAttachmentsCachedResponse>>>
    {
        private readonly IAttachmentCacheRepository _attachmentCache;
        private readonly IMapper _mapper;

        public GetAllAttachmentsCachedQueryHandler(IAttachmentCacheRepository attachmentCache, IMapper mapper)
        {
            _attachmentCache = attachmentCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllAttachmentsCachedResponse>>> Handle(GetAllAttachmentsCachedQuery request, CancellationToken cancellationToken)
        {
            var attachmentList = await _attachmentCache.GetCachedListAsync();
            var mappedAttachments = _mapper.Map<List<GetAllAttachmentsCachedResponse>>(attachmentList);
            return Result<List<GetAllAttachmentsCachedResponse>>.Success(mappedAttachments);
        }
    }
}