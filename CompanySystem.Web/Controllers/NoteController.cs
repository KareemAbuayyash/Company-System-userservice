using Microsoft.AspNetCore.Mvc;
using CompanySystem.Business.Interfaces;
using CompanySystem.Web.ViewModels;
using CompanySystem.Web.Helpers;

namespace CompanySystem.Web.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<IActionResult> Index(NoteFilterViewModel filter)
        {
            var filterDto = NoteMapper.MapToFilterDto(filter);
            var result = await _noteService.GetNotesByFilterAsync(filterDto);

            var viewModel = new NotesIndexViewModel
            {
                Notes = result.Notes.Select(NoteMapper.MapToViewModel),
                Filter = filter,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            return note == null ? NotFound() : View(NoteMapper.MapToViewModel(note));
        }

        public IActionResult Create()
        {
            return View(new CreateNoteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNoteViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var createDto = NoteMapper.MapToCreateDto(viewModel);
            var result = await _noteService.CreateNoteAsync(createDto, "System");

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Note created successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Note!.NoteId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            return note == null ? NotFound() : View(NoteMapper.MapToEditViewModel(note));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditNoteViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var updateDto = NoteMapper.MapToUpdateDto(viewModel);
            var result = await _noteService.UpdateNoteAsync(updateDto, "System");

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Note updated successfully.";
            return RedirectToAction(nameof(Details), new { id = viewModel.NoteId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            return note == null ? NotFound() : View(NoteMapper.MapToDeleteViewModel(note));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _noteService.DeleteNoteAsync(id, "System");

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "Note deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetNotesByEmployee(int employeeId)
        {
            var notes = await _noteService.GetNotesByEmployeeIdAsync(employeeId);
            return Json(notes.Select(NoteMapper.MapToViewModel));
        }

        [HttpGet]
        public async Task<IActionResult> GetNotesByType(string noteType)
        {
            var notes = await _noteService.GetNotesByTypeAsync(noteType);
            return Json(notes.Select(NoteMapper.MapToViewModel));
        }
    }
}
