namespace EDocSys.Application.CacheKeys
{
    public static class EnvironmentalManualStatusCacheKeys
    {
        public static string ListKey => "EnvironmentalManualStatusList";

        public static string SelectListKey => "EnvironmentalManualStatusSelectList";

        public static string GetKey(int environmentalManualId) => $"EnvironmentalManualStatus-{environmentalManualId}";

        public static string GetDetailsKey(int environmentalManualId) => $"EnvironmentalManualStatusDetails-{environmentalManualId}";
    }
}
