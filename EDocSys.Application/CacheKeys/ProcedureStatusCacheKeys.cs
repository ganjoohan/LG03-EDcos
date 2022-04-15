namespace EDocSys.Application.CacheKeys
{
    public static class ProcedureStatusCacheKeys
    {
        public static string ListKey => "ProcedureStatusList";

        public static string SelectListKey => "ProcedureStatusSelectList";

        public static string GetKey(int procedureId) => $"ProcedureStatus-{procedureId}";

        public static string GetDetailsKey(int procedureId) => $"ProcedureStatusDetails-{procedureId}";
    }
}
