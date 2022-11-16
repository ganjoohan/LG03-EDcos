namespace EDocSys.Infrastructure.CacheKeys.ExternalCacheKeys
{
    public static class LionSteelCacheKeys
    {
        public static string ListKey => "ExternalLionSteelList";

        public static string SelectListKey => "ExternalLionSteelSelectList";

        public static string GetKey(int lionSteelId) => $"ExternalLionSteel-{lionSteelId}";
        public static string GetKeyDOCNo(string docno) => $"ExternalLionSteel-{docno}";

        public static string GetDetailsKey(int lionSteelId) => $"ExternalLionSteelDetails-{lionSteelId}";
    }
}