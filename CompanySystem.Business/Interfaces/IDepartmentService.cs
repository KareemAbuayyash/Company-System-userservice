using CompanySystem.Data.Models;
using CompanySystem.Business.DTOs;

namespace CompanySystem.Business.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<IEnumerable<Department>> GetFilteredDepartmentsAsync(string? searchTerm = null, string sortBy = "name");
        Task<DepartmentSearchResultDto> GetDepartmentsForIndexAsync(string? searchTerm = null, string sortBy = "name");
        Task<IEnumerable<DepartmentDto>> GetDepartmentsForSearchAsync(string? searchTerm = null, string sortBy = "name");
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<DepartmentDto?> GetDepartmentDtoByIdAsync(int id);
        Task<Department?> CreateDepartmentAsync(string departmentName);
        Task<Department?> UpdateDepartmentAsync(int departmentId, string departmentName);
        Task<bool> SoftDeleteDepartmentAsync(int id, string deletedBy);
        Task<bool> DepartmentExistsAsync(int id);
        Task<bool> DepartmentNameExistsAsync(string name, int? excludeId = null);
        Task<int> GetDepartmentCountAsync(string? searchTerm = null);
    }
}
