namespace EDocSys.Application.CacheKeys
{
    public static class QualityManualCacheKeys
    {
        public static string ListKey => "QualityManualList";

        public static string SelectListKey => "QualityManualSelectList";

        public static string GetKey(int productId) => $"QualityManual-{productId}";

        public static string GetDetailsKey(int productId) => $"QualityManualDetails-{productId}";
    }
}