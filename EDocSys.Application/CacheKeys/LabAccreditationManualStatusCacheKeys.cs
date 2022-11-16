namespace EDocSys.Application.CacheKeys
{
    public static class LabAccreditationManualStatusCacheKeys
    {
        public static string ListKey => "LabAccreditationManualStatusList";

        public static string SelectListKey => "LabAccreditationManualStatusSelectList";

        public static string GetKey(int labAccreditationManualId) => $"LabAccreditationManualStatus-{labAccreditationManualId}";

        public static string GetDetailsKey(int labAccreditationManualId) => $"LabAccreditationManualStatusDetails-{labAccreditationManualId}";
    }
}
