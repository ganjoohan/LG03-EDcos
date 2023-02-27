namespace EDocSys.Application.Features.Departments.Queries.GetById
{
    public class GetDepartmentByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InformedList { get; set; }
    }
}