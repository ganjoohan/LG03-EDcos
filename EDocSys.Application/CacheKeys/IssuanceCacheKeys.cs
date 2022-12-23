namespace EDocSys.Application.CacheKeys
{
    public static class IssuanceCacheKeys
    {
        public static string ListKey => "IssuanceList";

        public static string SelectListKey => "IssuanceSelectList";

        public static string GetKey(int docId) => $"Issuance-{docId}";

        public static string GetDetailsKey(int docId) => $"IssuanceDetails-{docId}";
    }
}