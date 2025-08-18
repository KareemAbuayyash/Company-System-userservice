using Microsoft.AspNetCore.Mvc;
using CompanySystem.Business.Interfaces;
using CompanySystem.Web.ViewModels;

namespace CompanySystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMainPageContentService _mainPageContentService;

        public HomeController(IMainPageContentService mainPageContentService)
        {
            _mainPageContentService = mainPageContentService;
        }

        public async Task<IActionResult> Index()
        {
            var contents = await _mainPageContentService.GetPublicMainPageContentsAsync();
            var viewModel = new MainPageViewModel
            {
                Sections = contents.Select(c => new PublicMainPageViewModel
                {
                    SectionName = c.SectionName,
                    Title = c.Title,
                    Content = c.Content
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickEdit(string sectionName, string title, string content)
        {
            try
            {
                // Get the existing content by section name
                var existingContent = await _mainPageContentService.GetMainPageContentBySectionAsync(sectionName);
                
                if (existingContent == null)
                {
                    return Json(new { success = false, message = "Content not found" });
                }

                // Update the content
                var updateDto = new Business.DTOs.UpdateMainPageContentDto
                {
                    SectionName = sectionName,
                    Title = title,
                    Content = content
                };

                var result = await _mainPageContentService.UpdateMainPageContentAsync(existingContent.ContentId, updateDto);
                
                if (result != null)
                {
                    return Json(new { success = true, message = "Content updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update content" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
    }
} 