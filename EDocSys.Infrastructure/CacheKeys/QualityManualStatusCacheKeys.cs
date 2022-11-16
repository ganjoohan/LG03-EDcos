namespace EDocSys.Infrastructure.CacheKeys
{
    public static class QualityManualStatusCacheKeys
    {
        public static string ListKey => "QualityManualStatusList";

        public static string SelectListKey => "QualityManualStatusSelectList";

        public static string GetKey(int qualityManualId) => $"QualityManualStatus-{qualityManualId}";

        public static string GetDetailsKey(int qualityManualId) => $"QualityManualDetails-{qualityManualId}";

    }
}