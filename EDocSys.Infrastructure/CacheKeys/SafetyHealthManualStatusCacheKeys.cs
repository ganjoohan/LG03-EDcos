namespace EDocSys.Infrastructure.CacheKeys
{
    public static class SafetyHealthManualStatusCacheKeys
    {
        public static string ListKey => "SafetyHealthManualStatusList";

        public static string SelectListKey => "SafetyHealthManualStatusSelectList";

        public static string GetKey(int safetyHealthManualId) => $"SafetyHealthManualStatus-{safetyHealthManualId}";

        public static string GetDetailsKey(int safetyHealthManualId) => $"SafetyHealthManualDetails-{safetyHealthManualId}";

    }
}