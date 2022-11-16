namespace EDocSys.Infrastructure.CacheKeys.ExternalCacheKeys
{
    public static class AttachmentCacheKeys
    {
        public static string ListKey => "AttachmentList";

        public static string SelectListKey => "AttachmentSelectList";

        public static string GetKey(int attachmentId) => $"Attachment-{attachmentId}";

        public static string GetKeyDOCID(int docid) => $"AttachmentDoc-{docid}";
        //public static string GetKeyDOCNo(string docno) => $"Attachment-{docno}";

        public static string GetDetailsKey(int attachmentId) => $"AttachmentDetails-{attachmentId}";
    }
}