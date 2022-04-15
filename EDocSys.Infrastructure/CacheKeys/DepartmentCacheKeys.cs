﻿namespace EDocSys.Infrastructure.CacheKeys
{
    public static class DepartmentCacheKeys
    {
        public static string ListKey => "DepartmentList";

        public static string SelectListKey => "DepartmentSelectList";

        public static string GetKey(int departmentId) => $"Department-{departmentId}";

        public static string GetDetailsKey(int departmentId) => $"DepartmentDetails-{departmentId}";
    }
}