namespace EDocSys.Application.Features.UserApprovers.Queries.GetAllCached
{
    public class GetAllUserApproversCachedResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ApprovalType { get; set; }
    }
}
