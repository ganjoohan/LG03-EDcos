namespace EDocSys.Infrastructure.CacheKeys
{
    public static class IssuanceInfoPrintCacheKeys
    {
        public static string ListKey => "IssuanceInfoPrintList";

        public static string SelectListKey => "IssuanceInfoPrintSelectList";

        public static string GetKey(int docId) => $"IssuanceInfoPrint-{docId}";

        public static string GetKeyDOCNo(string docno) => $"IssuanceInfoPrint-{docno}";

        public static string GetDetailsKey(int docId) => $"IssuanceInfoPrintDetails-{docId}";
    }
}