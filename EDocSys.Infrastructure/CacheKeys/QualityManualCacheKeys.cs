namespace EDocSys.Infrastructure.CacheKeys
{
    public static class QualityManualCacheKeys
    {
        public static string ListKey => "QualityManualList";

        public static string SelectListKey => "QualityManualSelectList";

        public static string GetKey(int qualityManualId) => $"QualityManual-{qualityManualId}";

        public static string GetKeyDOCNo(string docno) => $"QualityManual-{docno}";

        public static string GetDetailsKey(int qualityManualId) => $"QualityManualDetails-{qualityManualId}";
    }
}