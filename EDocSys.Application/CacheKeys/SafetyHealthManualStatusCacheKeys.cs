namespace EDocSys.Application.CacheKeys
{
    public static class SafetyHealthManualStatusCacheKeys
    {
        public static string ListKey => "SafetyHealthManualStatusList";

        public static string SelectListKey => "SafetyHealthManualStatusSelectList";

        public static string GetKey(int safetyHealthManualId) => $"SafetyHealthStatus-{safetyHealthManualId}";

        public static string GetDetailsKey(int safetyHealthManualId) => $"SafetyHealthManualStatusDetails-{safetyHealthManualId}";
    }
}
