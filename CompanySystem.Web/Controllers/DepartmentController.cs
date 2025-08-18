using Microsoft.AspNetCore.Mvc;
using CompanySystem.Business.Interfaces;
using CompanySystem.Data.Models;
using CompanySystem.Web.ViewModels;
using CompanySystem.Business.DTOs;

namespace CompanySystem.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        private static DepartmentViewModel MapToViewModel(DepartmentDto dto)
        {
            return new DepartmentViewModel
            {
                DepartmentId = dto.DepartmentId,
                DepartmentName = dto.DepartmentName,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate,
                UpdatedBy = dto.UpdatedBy,
                UpdatedDate = dto.UpdatedDate
            };
        }

        public IActionResult Test()
        {
            return Content("Department Controller is working!");
        }

        // GET: Department
        public async Task<IActionResult> Index(string searchTerm, string sortBy = "name")
        {
            var result = await _departmentService.GetDepartmentsForIndexAsync(searchTerm, sortBy);
            var viewModels = result.Departments.Select(MapToViewModel).ToList();
            
            ViewBag.SearchTerm = result.SearchTerm;
            ViewBag.SortBy = result.SortBy;
            ViewBag.TotalDepartments = result.TotalCount;
            ViewBag.HasSearch = result.HasSearch;

            return View("Department", viewModels);
        }        
        [HttpGet]
        public async Task<IActionResult> SearchDepartments(string searchTerm, string sortBy = "name")
        {
            var departments = await _departmentService.GetDepartmentsForSearchAsync(searchTerm, sortBy);
            
            return Json(new
            {
                success = true,
                data = departments.Select(d => new
                {
                    departmentId = d.DepartmentId,
                    departmentName = d.DepartmentName,
                    createdBy = d.CreatedBy,
                    createdDate = d.CreatedDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                    updatedBy = d.UpdatedBy,
                    updatedDate = d.UpdatedDate?.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                }),
                count = departments.Count(),
                searchTerm = searchTerm,
                sortBy = sortBy
            });
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var departmentDto = await _departmentService.GetDepartmentDtoByIdAsync(id);
            if (departmentDto == null)
            {
                return NotFound();
            }
            return View(MapToViewModel(departmentDto));
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View(new CreateDepartmentViewModel());
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _departmentService.CreateDepartmentAsync(model.DepartmentName);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Department created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("DepartmentName", "A department with this name already exists.");
            return View(model);
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var viewModel = new EditDepartmentViewModel
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName
            };

            return View(viewModel);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditDepartmentViewModel model)
        {
            if (id != model.DepartmentId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _departmentService.UpdateDepartmentAsync(model.DepartmentId, model.DepartmentName);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Department updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("DepartmentName", "A department with this name already exists.");
            return View(model);
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var departmentDto = await _departmentService.GetDepartmentDtoByIdAsync(id);
            if (departmentDto == null)
            {
                return NotFound();
            }
            return View(MapToViewModel(departmentDto));
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _departmentService.SoftDeleteDepartmentAsync(id, "System");
            if (result)
            {
                TempData["SuccessMessage"] = "Department deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete department.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
