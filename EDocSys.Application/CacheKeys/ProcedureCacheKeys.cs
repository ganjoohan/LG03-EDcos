namespace EDocSys.Application.CacheKeys
{
    public static class ProcedureCacheKeys
    {
        public static string ListKey => "ProcedureList";

        public static string SelectListKey => "ProcedureSelectList";

        public static string GetKey(int procedureId) => $"Procedure-{procedureId}";

        public static string GetKeyParameter(int companyId, int departmentId) => $"Procedure-Comp{companyId}-Dept{departmentId}";

        public static string GetDetailsKey(int procedureId) => $"ProcedureDetails-{procedureId}";
    }
}