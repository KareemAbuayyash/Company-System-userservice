using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Business.DTOs
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateDepartmentDto
    {
        public string DepartmentName { get; set; }
    }

    public class EditDepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class DepartmentSearchResultDto
    {
        public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
        public int TotalCount { get; set; }
        public string? SearchTerm { get; set; }
        public string SortBy { get; set; } = "name";
        public bool HasSearch { get; set; }
    }
}
