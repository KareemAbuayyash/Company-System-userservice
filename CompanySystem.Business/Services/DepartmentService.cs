using Microsoft.EntityFrameworkCore;
using CompanySystem.Business.Interfaces;
using CompanySystem.Data.Context;
using CompanySystem.Data.Models;
using CompanySystem.Business.DTOs;

namespace CompanySystem.Business.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly CompanySystemDbContext _context;

        public DepartmentService(CompanySystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetFilteredDepartmentsAsync(string? searchTerm = null, string sortBy = "name")
        {
            var query = _context.Departments.AsQueryable();

            // Apply search filter at database level - now works from first character
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var trimmedSearchTerm = searchTerm.Trim();
                query = query.Where(d => 
                    EF.Functions.Like(d.DepartmentName, $"%{trimmedSearchTerm}%") ||
                    EF.Functions.Like(d.CreatedBy, $"%{trimmedSearchTerm}%"));
            }

            // Apply sorting at database level
            query = sortBy?.ToLower() switch
            {
                "name" => query.OrderBy(d => d.DepartmentName),
                "name_desc" => query.OrderByDescending(d => d.DepartmentName),
                "date" => query.OrderBy(d => d.CreatedDate),
                "date_desc" => query.OrderByDescending(d => d.CreatedDate),
                "creator" => query.OrderBy(d => d.CreatedBy),
                "creator_desc" => query.OrderByDescending(d => d.CreatedBy),
                _ => query.OrderBy(d => d.DepartmentName)
            };

            return await query.ToListAsync();
        }

        public async Task<DepartmentSearchResultDto> GetDepartmentsForIndexAsync(string? searchTerm = null, string sortBy = "name")
        {
            var departments = await GetFilteredDepartmentsAsync(searchTerm, sortBy);
            
            var departmentDtos = departments.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                CreatedBy = d.CreatedBy,
                CreatedDate = d.CreatedDate,
                UpdatedBy = d.UpdatedBy,
                UpdatedDate = d.UpdatedDate
            }).ToList();

            return new DepartmentSearchResultDto
            {
                Departments = departmentDtos,
                TotalCount = departmentDtos.Count,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                HasSearch = !string.IsNullOrWhiteSpace(searchTerm)
            };
        }

        public async Task<IEnumerable<DepartmentDto>> GetDepartmentsForSearchAsync(string? searchTerm = null, string sortBy = "name")
        {
            var departments = await GetFilteredDepartmentsAsync(searchTerm, sortBy);
            
            return departments.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                CreatedBy = d.CreatedBy,
                CreatedDate = d.CreatedDate,
                UpdatedBy = d.UpdatedBy,
                UpdatedDate = d.UpdatedDate
            }).ToList();
        }

        public async Task<DepartmentDto?> GetDepartmentDtoByIdAsync(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return null;

            return new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                CreatedBy = department.CreatedBy,
                CreatedDate = department.CreatedDate,
                UpdatedBy = department.UpdatedBy,
                UpdatedDate = department.UpdatedDate
            };
        }

        public async Task<int> GetDepartmentCountAsync(string? searchTerm = null)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var trimmedSearchTerm = searchTerm.Trim();
                query = query.Where(d => 
                    EF.Functions.Like(d.DepartmentName, $"%{trimmedSearchTerm}%") ||
                    EF.Functions.Like(d.CreatedBy, $"%{trimmedSearchTerm}%"));
            }

            return await query.CountAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<Department?> CreateDepartmentAsync(string departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
                throw new ArgumentNullException(nameof(departmentName));

            // Check if department name already exists
            if (await DepartmentNameExistsAsync(departmentName))
                return null;

            var department = new Department
            {
                DepartmentName = departmentName,
                CreatedBy = "System", // Replace with actual user when authentication is implemented
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<Department?> UpdateDepartmentAsync(int departmentId, string departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
                throw new ArgumentNullException(nameof(departmentName));

            // Check if department name already exists (excluding current department)
            if (await DepartmentNameExistsAsync(departmentName, departmentId))
                return null;

            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found");

            existingDepartment.DepartmentName = departmentName;
            existingDepartment.UpdatedBy = "System"; // Replace with actual user when authentication is implemented
            existingDepartment.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingDepartment;
        }

        public async Task<bool> SoftDeleteDepartmentAsync(int id, string deletedBy)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return false;

            department.IsDeleted = true;
            department.UpdatedBy = deletedBy;
            department.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DepartmentExistsAsync(int id)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentId == id);
        }

        public async Task<bool> DepartmentNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Departments.Where(d => d.DepartmentName.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(d => d.DepartmentId != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
