using Microsoft.AspNetCore.Mvc;
using CompanySystem.Business.Interfaces;
using CompanySystem.Business.DTOs;
using CompanySystem.Web.ViewModels;

namespace CompanySystem.Web.Controllers
{
    public class MainPageContentController : Controller
    {
        private readonly IMainPageContentService _mainPageContentService;

        public MainPageContentController(IMainPageContentService mainPageContentService)
        {
            _mainPageContentService = mainPageContentService;
        }

        // GET: MainPageContent
        public async Task<IActionResult> Index()
        {
            var contents = await _mainPageContentService.GetAllMainPageContentsAsync();
            var viewModels = contents.Select(c => new MainPageContentListViewModel
            {
                ContentId = c.ContentId,
                SectionName = c.SectionName,
                Title = c.Title,
                ContentPreview = c.ContentPreview,
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate
            });

            return View(viewModels);
        }

        // GET: MainPageContent/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var content = await _mainPageContentService.GetMainPageContentByIdAsync(id);
            if (content == null)
            {
                TempData["ErrorMessage"] = "Main page content not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new MainPageContentViewModel
            {
                ContentId = content.ContentId,
                SectionName = content.SectionName,
                Title = content.Title,
                Content = content.Content,
                CreatedDate = content.CreatedDate,
                UpdatedDate = content.UpdatedDate,
                CreatedBy = content.CreatedBy,
                UpdatedBy = content.UpdatedBy
            };

            return View(viewModel);
        }

        // GET: MainPageContent/Create
        public IActionResult Create()
        {
            var viewModel = new CreateMainPageContentViewModel();
            return View(viewModel);
        }

        // POST: MainPageContent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMainPageContentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Check if section already exists
            if (await _mainPageContentService.SectionExistsAsync(viewModel.SectionName))
            {
                ModelState.AddModelError("SectionName", "A content with this section name already exists.");
                return View(viewModel);
            }

            var createDto = new CreateMainPageContentDto
            {
                SectionName = viewModel.SectionName,
                Title = viewModel.Title,
                Content = viewModel.Content
            };

            try
            {
                var created = await _mainPageContentService.CreateMainPageContentAsync(createDto);
                TempData["SuccessMessage"] = "Main page content created successfully.";
                return RedirectToAction(nameof(Details), new { id = created.ContentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the content. Please try again.");
                return View(viewModel);
            }
        }

        // GET: MainPageContent/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var content = await _mainPageContentService.GetMainPageContentByIdAsync(id);
            if (content == null)
            {
                TempData["ErrorMessage"] = "Main page content not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditMainPageContentViewModel
            {
                ContentId = content.ContentId,
                SectionName = content.SectionName,
                Title = content.Title,
                Content = content.Content
            };

            return View(viewModel);
        }

        // POST: MainPageContent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditMainPageContentViewModel viewModel)
        {
            if (id != viewModel.ContentId)
            {
                TempData["ErrorMessage"] = "Invalid content ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Check if section already exists (excluding current content)
            if (await _mainPageContentService.SectionExistsAsync(viewModel.SectionName, viewModel.ContentId))
            {
                ModelState.AddModelError("SectionName", "A content with this section name already exists.");
                return View(viewModel);
            }

            var updateDto = new UpdateMainPageContentDto
            {
                SectionName = viewModel.SectionName,
                Title = viewModel.Title,
                Content = viewModel.Content
            };

            try
            {
                var updated = await _mainPageContentService.UpdateMainPageContentAsync(id, updateDto);
                if (updated == null)
                {
                    TempData["ErrorMessage"] = "Main page content not found.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Main page content updated successfully.";
                return RedirectToAction(nameof(Details), new { id = updated.ContentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the content. Please try again.");
                return View(viewModel);
            }
        }

        // GET: MainPageContent/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var content = await _mainPageContentService.GetMainPageContentByIdAsync(id);
            if (content == null)
            {
                TempData["ErrorMessage"] = "Main page content not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new DeleteMainPageContentViewModel
            {
                ContentId = content.ContentId,
                SectionName = content.SectionName,
                Title = content.Title,
                Content = content.Content,
                CreatedDate = content.CreatedDate,
                CreatedBy = content.CreatedBy
            };

            return View(viewModel);
        }

        // POST: MainPageContent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _mainPageContentService.DeleteMainPageContentAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Main page content not found.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Main page content deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the content. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
