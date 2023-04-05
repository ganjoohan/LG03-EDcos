namespace EDocSys.Application.CacheKeys
{
    public static class WICacheKeys
    {
        public static string ListKey => "WIList";

        public static string SelectListKey => "WISelectList";

        public static string GetKey(int wiId) => $"WI-{wiId}";

        public static string GetKeyParameter(int companyId, int departmentId) => $"WI-Comp{companyId}-Dept{departmentId}";

        public static string GetDetailsKey(int wiId) => $"WIDetails-{wiId}";
    }
}