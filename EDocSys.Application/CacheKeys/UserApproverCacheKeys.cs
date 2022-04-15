namespace EDocSys.Application.CacheKeys
{
    public static class UserApproverCacheKeys
    {
        public static string ListKey => "UserApproverList";

        public static string SelectListKey => "UserApproverSelectList";

        public static string GetKey(int userapproverId) => $"UserApprover-{userapproverId}";

        public static string GetDetailsKey(int userapproverId) => $"UserApproverDetails-{userapproverId}";
    }
}
