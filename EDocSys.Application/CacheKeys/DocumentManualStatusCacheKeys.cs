namespace EDocSys.Application.CacheKeys
{
    public static class DocumentManualStatusCacheKeys
    {
        public static string ListKey => "DocumentManualStatusList";

        public static string SelectListKey => "DocumentManualStatusSelectList";

        public static string GetKey(int documentManualId) => $"DocumentManualStatus-{documentManualId}";

        public static string GetDetailsKey(int documentManualId) => $"DocumentManualStatusDetails-{documentManualId}";
    }
}
