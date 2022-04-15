namespace EDocSys.Infrastructure.CacheKeys
{
    public static class CompanyCacheKeys
    {
        public static string ListKey => "CompanyList";

        public static string SelectListKey => "CompanySelectList";

        public static string GetKey(int companyId) => $"Company-{companyId}";

        public static string GetDetailsKey(int companyId) => $"CompanyDetails-{companyId}";
    }
}