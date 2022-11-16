namespace EDocSys.Application.CacheKeys
{
    public static class EnvironmentalManualCacheKeys
    {
        public static string ListKey => "EnvironmentalManualList";

        public static string SelectListKey => "EnvironmentalManualSelectList";

        public static string GetKey(int productId) => $"EnvironmentalManual-{productId}";

        public static string GetDetailsKey(int productId) => $"EnvironmentalManualDetails-{productId}";
    }
}