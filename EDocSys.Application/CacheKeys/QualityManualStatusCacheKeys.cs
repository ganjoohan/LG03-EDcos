namespace EDocSys.Application.CacheKeys
{
    public static class QualityManualStatusCacheKeys
    {
        public static string ListKey => "QualityManualStatusList";

        public static string SelectListKey => "QualityManualStatusSelectList";

        public static string GetKey(int qualityManualId) => $"QualityStatus-{qualityManualId}";

        public static string GetDetailsKey(int qualityManualId) => $"QualityManualStatusDetails-{qualityManualId}";
    }
}
