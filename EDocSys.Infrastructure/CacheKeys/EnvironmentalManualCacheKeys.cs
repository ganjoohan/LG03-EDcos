namespace EDocSys.Infrastructure.CacheKeys
{
    public static class EnvironmentalManualCacheKeys
    {
        public static string ListKey => "EnvironmentalManualList";

        public static string SelectListKey => "EnvironmentalManualSelectList";

        public static string GetKey(int environmentalManualId) => $"EnvironmentalManual-{environmentalManualId}";

        public static string GetKeyDOCNo(string docno) => $"EnvironmentalManual-{docno}";

        public static string GetDetailsKey(int environmentalManualId) => $"EnvironmentalManualDetails-{environmentalManualId}";
    }
}