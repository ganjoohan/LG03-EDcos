namespace EDocSys.Application.CacheKeys.ExternalCacheKeys
{
    public static class LionSteelCacheKeys
    {
        public static string ListKey => "ExternalLionSteelList";

        public static string SelectListKey => "ExternalLionSteelSelectList";

        public static string GetKey(int productId) => $"ExternalLionSteel-{productId}";

        public static string GetDetailsKey(int productId) => $"ExternalLionSteelDetails-{productId}";
    }
}