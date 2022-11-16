namespace EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetByDocId
{
    public class GetAttachmentByDocIdResponse
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNameBatch { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public string FileLoc { get; set; }
        public int DocId { get; set; }
        public string DocName { get; set; }
        public bool IsActive { get; set; }
    }
}