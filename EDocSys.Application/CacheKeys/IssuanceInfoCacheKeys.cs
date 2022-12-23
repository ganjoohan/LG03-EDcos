namespace EDocSys.Application.CacheKeys
{
    public static class IssuanceInfoCacheKeys
    {
        public static string ListKey => "IssuanceInfoList";

        public static string SelectListKey => "IssuanceInfoSelectList";

        public static string GetKey(int docId) => $"IssuanceInfo-{docId}";

        public static string GetDetailsKey(int docId) => $"IssuanceInfoDetails-{docId}";
    }
}