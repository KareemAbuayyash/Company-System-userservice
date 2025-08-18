using CompanySystem.Business.DTOs;
using CompanySystem.Web.ViewModels;

namespace CompanySystem.Web.Helpers
{
    public static class NoteMapper
    {
        public static NoteViewModel MapToViewModel(NoteDto dto)
        {
            return new NoteViewModel
            {
                NoteId = dto.NoteId,
                EmployeeId = dto.EmployeeId,
                NoteType = dto.NoteType,
                Title = dto.Title,
                Content = dto.Content,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate,
                UpdatedBy = dto.UpdatedBy,
                UpdatedDate = dto.UpdatedDate
            };
        }

        public static CreateNoteDto MapToCreateDto(CreateNoteViewModel viewModel)
        {
            return new CreateNoteDto
            {
                EmployeeId = viewModel.EmployeeId,
                NoteType = viewModel.NoteType,
                Title = viewModel.Title,
                Content = viewModel.Content
            };
        }

        public static UpdateNoteDto MapToUpdateDto(EditNoteViewModel viewModel)
        {
            return new UpdateNoteDto
            {
                NoteId = viewModel.NoteId,
                NoteType = viewModel.NoteType,
                Title = viewModel.Title,
                Content = viewModel.Content
            };
        }

        public static EditNoteViewModel MapToEditViewModel(NoteDto dto)
        {
            return new EditNoteViewModel
            {
                NoteId = dto.NoteId,
                NoteType = dto.NoteType,
                Title = dto.Title,
                Content = dto.Content
            };
        }

        public static DeleteNoteViewModel MapToDeleteViewModel(NoteDto dto)
        {
            return new DeleteNoteViewModel
            {
                NoteId = dto.NoteId,
                EmployeeId = dto.EmployeeId,
                NoteType = dto.NoteType,
                Title = dto.Title,
                Content = dto.Content,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate
            };
        }

        public static NoteFilterDto MapToFilterDto(NoteFilterViewModel viewModel)
        {
            return new NoteFilterDto
            {
                EmployeeId = viewModel.EmployeeId,
                NoteType = viewModel.NoteType,
                CreatedBy = viewModel.CreatedBy,
                FromDate = viewModel.FromDate,
                ToDate = viewModel.ToDate,
                PageNumber = viewModel.PageNumber,
                PageSize = viewModel.PageSize
            };
        }
    }
}
