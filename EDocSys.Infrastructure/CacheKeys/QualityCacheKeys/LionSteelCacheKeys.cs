namespace EDocSys.Infrastructure.CacheKeys.QualityCacheKeys
{
    public static class LionSteelCacheKeys
    {
        public static string ListKey => "QualityLionSteelList";

        public static string SelectListKey => "QualityLionSteelSelectList";

        public static string GetKey(int lionSteelId) => $"QualityLionSteel-{lionSteelId}";
        public static string GetKeyDOCNo(string docno) => $"QualityLionSteel-{docno}";

        public static string GetDetailsKey(int lionSteelId) => $"QualityLionSteelDetails-{lionSteelId}";
    }
}