using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EDocSys.Application.Interfaces.CacheRepositories.QualityCacheRepositories;

namespace EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetByDocId
{
    public class GetAttachmentByDocIdQuery : IRequest<Result<List<GetAttachmentByDocIdResponse>>>
    {
        public int DocId { get; set; }
        //public GetAttachmentByDocIdQuery()
        //{
        //}
        public class GetAttachmentByDocIdQueryHandler : IRequestHandler<GetAttachmentByDocIdQuery, Result<List<GetAttachmentByDocIdResponse>>>
        {
            private readonly IAttachmentCacheRepository _attachmentCache;
            private readonly IMapper _mapper;

            public GetAttachmentByDocIdQueryHandler(IAttachmentCacheRepository attachmentCache, IMapper mapper)
            {
                _attachmentCache = attachmentCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAttachmentByDocIdResponse>>> Handle(GetAttachmentByDocIdQuery query, CancellationToken cancellationToken)
            {
                var attachment = await _attachmentCache.GetByDocIdAsync(query.DocId);
                var mappedAttachment = _mapper.Map<List<GetAttachmentByDocIdResponse>>(attachment);
                return Result< List<GetAttachmentByDocIdResponse>>.Success(mappedAttachment);
            }
        }
    }
}