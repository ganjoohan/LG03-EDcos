namespace EDocSys.Infrastructure.CacheKeys
{
    public static class LabAccreditationManualCacheKeys
    {
        public static string ListKey => "LabAccreditationManualList";

        public static string SelectListKey => "LabAccreditationManualSelectList";

        public static string GetKey(int labAccreditationManualId) => $"LabAccreditationManual-{labAccreditationManualId}";

        public static string GetKeyDOCNo(string docno) => $"LabAccreditationManual-{docno}";

        public static string GetDetailsKey(int labAccreditationManualId) => $"LabAccreditationManualDetails-{labAccreditationManualId}";
    }
}