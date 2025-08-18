using Microsoft.EntityFrameworkCore;
using CompanySystem.Business.DTOs;
using CompanySystem.Business.Interfaces;
using CompanySystem.Data.Context;
using CompanySystem.Data.Models;

namespace CompanySystem.Business.Services
{
    public class NoteService : INoteService
    {
        private readonly CompanySystemDbContext _context;

        public NoteService(CompanySystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NoteDto>> GetAllNotesAsync()
        {
            var notes = await _context.Notes
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return notes.Select(MapToDto);
        }

        public async Task<(IEnumerable<NoteDto> Notes, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage)> GetNotesByFilterAsync(NoteFilterDto filter)
        {
            var query = _context.Notes.AsQueryable();

            if (filter.EmployeeId.HasValue)
                query = query.Where(n => n.EmployeeId == filter.EmployeeId.Value);

            if (!string.IsNullOrEmpty(filter.NoteType))
                query = query.Where(n => n.NoteType == filter.NoteType);

            if (!string.IsNullOrEmpty(filter.CreatedBy))
                query = query.Where(n => n.CreatedBy.Contains(filter.CreatedBy));

            if (filter.FromDate.HasValue)
                query = query.Where(n => n.CreatedDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(n => n.CreatedDate <= filter.ToDate.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            var notes = await query
                .OrderByDescending(n => n.CreatedDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var hasPreviousPage = filter.PageNumber > 1;
            var hasNextPage = filter.PageNumber < totalPages;

            return (notes.Select(MapToDto), totalCount, totalPages, hasPreviousPage, hasNextPage);
        }

        public async Task<NoteDto?> GetNoteByIdAsync(int noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            return note == null ? null : MapToDto(note);
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByEmployeeIdAsync(int employeeId)
        {
            var notes = await _context.Notes
                .Where(n => n.EmployeeId == employeeId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return notes.Select(MapToDto);
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByTypeAsync(string noteType)
        {
            var notes = await _context.Notes
                .Where(n => n.NoteType == noteType)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return notes.Select(MapToDto);
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByCreatedByAsync(string createdBy)
        {
            var notes = await _context.Notes
                .Where(n => n.CreatedBy == createdBy)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return notes.Select(MapToDto);
        }

        public async Task<(bool Success, NoteDto? Note, string? ErrorMessage)> CreateNoteAsync(CreateNoteDto createNoteDto, string createdBy)
        {
            // Validate note type
            if (createNoteDto.NoteType != NoteTypes.Technical && createNoteDto.NoteType != NoteTypes.Behavioral)
            {
                return (false, null, "Invalid note type. Must be Technical or Behavioral.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(createNoteDto.Title))
            {
                return (false, null, "Title is required.");
            }

            if (string.IsNullOrWhiteSpace(createNoteDto.Content))
            {
                return (false, null, "Content is required.");
            }

            try
            {
                var note = new Note
                {
                    EmployeeId = createNoteDto.EmployeeId,
                    NoteType = createNoteDto.NoteType,
                    Title = createNoteDto.Title.Trim(),
                    Content = createNoteDto.Content.Trim(),
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Notes.Add(note);
                await _context.SaveChangesAsync();

                return (true, MapToDto(note), null);
            }
            catch (Exception ex)
            {
                return (false, null, $"An error occurred while creating the note: {ex.Message}");
            }
        }

        public async Task<(bool Success, NoteDto? Note, string? ErrorMessage)> UpdateNoteAsync(UpdateNoteDto updateNoteDto, string updatedBy)
        {
            // Validate note type
            if (updateNoteDto.NoteType != NoteTypes.Technical && updateNoteDto.NoteType != NoteTypes.Behavioral)
            {
                return (false, null, "Invalid note type. Must be Technical or Behavioral.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(updateNoteDto.Title))
            {
                return (false, null, "Title is required.");
            }

            if (string.IsNullOrWhiteSpace(updateNoteDto.Content))
            {
                return (false, null, "Content is required.");
            }

            try
            {
                var note = await _context.Notes.FindAsync(updateNoteDto.NoteId);
                if (note == null)
                    return (false, null, "Note not found.");

                note.NoteType = updateNoteDto.NoteType;
                note.Title = updateNoteDto.Title.Trim();
                note.Content = updateNoteDto.Content.Trim();
                note.UpdatedBy = updatedBy;
                note.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return (true, MapToDto(note), null);
            }
            catch (Exception ex)
            {
                return (false, null, $"An error occurred while updating the note: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteNoteAsync(int noteId, string deletedBy)
        {
            try
            {
                var note = await _context.Notes.FindAsync(noteId);
                if (note == null)
                    return (false, "Note not found.");

                note.IsDeleted = true;
                note.UpdatedBy = deletedBy;
                note.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while deleting the note: {ex.Message}");
            }
        }

        public async Task<bool> NoteExistsAsync(int noteId)
        {
            return await _context.Notes.AnyAsync(n => n.NoteId == noteId);
        }

        private static NoteDto MapToDto(Note note)
        {
            return new NoteDto
            {
                NoteId = note.NoteId,
                EmployeeId = note.EmployeeId,
                NoteType = note.NoteType,
                Title = note.Title,
                Content = note.Content,
                CreatedBy = note.CreatedBy,
                CreatedDate = note.CreatedDate,
                UpdatedBy = note.UpdatedBy,
                UpdatedDate = note.UpdatedDate
            };
        }
    }
}
