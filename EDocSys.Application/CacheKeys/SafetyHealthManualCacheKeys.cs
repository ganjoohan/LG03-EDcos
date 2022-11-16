namespace EDocSys.Application.CacheKeys
{
    public static class SafetyHealthManualCacheKeys
    {
        public static string ListKey => "SafetyHealthManualList";

        public static string SelectListKey => "SafetyHealthManualSelectList";

        public static string GetKey(int productId) => $"SafetyHealthManual-{productId}";

        public static string GetDetailsKey(int productId) => $"SafetyHealthManualDetails-{productId}";
    }
}