namespace EDocSys.Application.CacheKeys
{
    public static class DocumentManualCacheKeys
    {
        public static string ListKey => "DocumentManualList";

        public static string SelectListKey => "DocumentManualSelectList";

        public static string GetKey(int productId) => $"DocumentManual-{productId}";

        public static string GetDetailsKey(int productId) => $"DocumentManualDetails-{productId}";
    }
}