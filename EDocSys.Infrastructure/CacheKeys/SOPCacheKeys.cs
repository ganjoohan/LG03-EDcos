namespace EDocSys.Infrastructure.CacheKeys
{
    public static class SOPCacheKeys
    {
        public static string ListKey => "SOPList";

        public static string SelectListKey => "SOPSelectList";

        public static string GetKey(int sopId) => $"SOP-{sopId}";

        public static string GetDetailsKey(int sopId) => $"SOPDetails-{sopId}";
    }
}