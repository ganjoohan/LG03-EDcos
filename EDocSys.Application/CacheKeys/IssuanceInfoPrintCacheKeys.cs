namespace EDocSys.Application.CacheKeys
{
    public static class IssuanceInfoPrintCacheKeys
    {
        public static string ListKey => "IssuanceInfoPrintList";

        public static string SelectListKey => "IssuanceInfoPrintSelectList";

        public static string GetKey(int docId) => $"IssuancePrintInfo-{docId}";

        public static string GetDetailsKey(int docId) => $"IssuanceInfoPrintDetails-{docId}";
    }
}