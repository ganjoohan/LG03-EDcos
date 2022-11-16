namespace EDocSys.Infrastructure.CacheKeys
{
    public static class DocumentManualCacheKeys
    {
        public static string ListKey => "DocumentManualList";

        public static string SelectListKey => "DocumentManualSelectList";

        public static string GetKey(int documentManualId) => $"DocumentManual-{documentManualId}";

        public static string GetKeyDOCNo(string docno) => $"DocumentManual-{docno}";

        public static string GetDetailsKey(int documentManualId) => $"DocumentManualDetails-{documentManualId}";
    }
}