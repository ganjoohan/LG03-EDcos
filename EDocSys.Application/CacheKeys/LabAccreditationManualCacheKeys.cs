namespace EDocSys.Application.CacheKeys
{
    public static class LabAccreditationManualCacheKeys
    {
        public static string ListKey => "LabAccreditationManualList";

        public static string SelectListKey => "LabAccreditationManualSelectList";

        public static string GetKey(int productId) => $"LabAccreditationManual-{productId}";

        public static string GetDetailsKey(int productId) => $"LabAccreditationManualDetails-{productId}";
    }
}