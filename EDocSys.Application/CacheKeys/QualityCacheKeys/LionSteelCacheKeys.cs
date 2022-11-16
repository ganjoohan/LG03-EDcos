namespace EDocSys.Application.CacheKeys.QualityCacheKeys
{
    public static class LionSteelCacheKeys
    {
        public static string ListKey => "QualityLionSteelList";

        public static string SelectListKey => "QualityLionSteelSelectList";

        public static string GetKey(int productId) => $"QualityLionSteel-{productId}";

        public static string GetDetailsKey(int productId) => $"QualityLionSteelDetails-{productId}";
    }
}