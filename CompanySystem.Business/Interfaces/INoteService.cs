using CompanySystem.Business.DTOs;

namespace CompanySystem.Business.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<NoteDto>> GetAllNotesAsync();
        Task<(IEnumerable<NoteDto> Notes, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage)> GetNotesByFilterAsync(NoteFilterDto filter);
        Task<NoteDto?> GetNoteByIdAsync(int noteId);
        Task<IEnumerable<NoteDto>> GetNotesByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<NoteDto>> GetNotesByTypeAsync(string noteType);
        Task<IEnumerable<NoteDto>> GetNotesByCreatedByAsync(string createdBy);
        Task<(bool Success, NoteDto? Note, string? ErrorMessage)> CreateNoteAsync(CreateNoteDto createNoteDto, string createdBy);
        Task<(bool Success, NoteDto? Note, string? ErrorMessage)> UpdateNoteAsync(UpdateNoteDto updateNoteDto, string updatedBy);
        Task<(bool Success, string? ErrorMessage)> DeleteNoteAsync(int noteId, string deletedBy);
        Task<bool> NoteExistsAsync(int noteId);
    }
}
