using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.CacheRepositories.QualityCacheRepositories;

namespace EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetById
{
    public class GetAttachmentByIdQuery : IRequest<Result<GetAttachmentByIdResponse>>
    {
        public int Id { get; set; }

        public class GetAttachmentByIdQueryHandler : IRequestHandler<GetAttachmentByIdQuery, Result<GetAttachmentByIdResponse>>
        {
            private readonly IAttachmentCacheRepository _attachmentCache;
            private readonly IMapper _mapper;

            public GetAttachmentByIdQueryHandler(IAttachmentCacheRepository attachmentCache, IMapper mapper)
            {
                _attachmentCache = attachmentCache;
                _mapper = mapper;
            }

            public async Task<Result<GetAttachmentByIdResponse>> Handle(GetAttachmentByIdQuery query, CancellationToken cancellationToken)
            {
                var attachment = await _attachmentCache.GetByIdAsync(query.Id);
                var mappedAttachment = _mapper.Map<GetAttachmentByIdResponse>(attachment);
                return Result<GetAttachmentByIdResponse>.Success(mappedAttachment);
            }
        }
    }
}