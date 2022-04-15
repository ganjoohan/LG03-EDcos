namespace EDocSys.Infrastructure.CacheKeys
{
    public static class SOPStatusCacheKeys
    {
        public static string ListKey => "SOPStatusList";

        public static string SelectListKey => "SOPStatusSelectList";

        public static string GetKey(int sopId) => $"SOPStatus-{sopId}";

        public static string GetDetailsKey(int sopId) => $"SOPDetails-{sopId}";

    }
}