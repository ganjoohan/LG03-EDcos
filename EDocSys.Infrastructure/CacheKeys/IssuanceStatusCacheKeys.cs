namespace EDocSys.Infrastructure.CacheKeys
{
    public static class IssuanceStatusCacheKeys
    {
        public static string ListKey => "IssuanceStatusList";

        public static string SelectListKey => "IssuanceStatusSelectList";

        public static string GetKey(int docId) => $"IssuanceStatus-{docId}";

        public static string GetDetailsKey(int docId) => $"IssuanceStatusDetails-{docId}";

    }
}