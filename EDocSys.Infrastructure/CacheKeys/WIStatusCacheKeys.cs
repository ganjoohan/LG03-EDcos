namespace EDocSys.Infrastructure.CacheKeys
{
    public static class WIStatusCacheKeys
    {
        public static string ListKey => "WIStatusList";

        public static string SelectListKey => "WIStatusSelectList";

        public static string GetKey(int wiId) => $"WIStatus-{wiId}";

        public static string GetDetailsKey(int wiId) => $"WIDetails-{wiId}";

    }
}