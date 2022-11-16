namespace EDocSys.Infrastructure.CacheKeys
{
    public static class SafetyHealthManualCacheKeys
    {
        public static string ListKey => "SafetyHealthManualList";

        public static string SelectListKey => "SafetyHealthManualSelectList";

        public static string GetKey(int safetyHealthManualId) => $"SafetyHealthManual-{safetyHealthManualId}";

        public static string GetKeyDOCNo(string docno) => $"SafetyHealthManual-{docno}";

        public static string GetDetailsKey(int safetyHealthManualId) => $"SafetyHealthManualDetails-{safetyHealthManualId}";
    }
}