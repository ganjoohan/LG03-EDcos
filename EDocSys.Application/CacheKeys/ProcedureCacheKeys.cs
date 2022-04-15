namespace EDocSys.Application.CacheKeys
{
    public static class ProcedureCacheKeys
    {
        public static string ListKey => "ProcedureList";

        public static string SelectListKey => "ProcedureSelectList";

        public static string GetKey(int productId) => $"Procedure-{productId}";

        public static string GetDetailsKey(int productId) => $"ProcedureDetails-{productId}";
    }
}