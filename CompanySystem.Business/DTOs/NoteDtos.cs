using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Business.DTOs
{
    public class NoteDto
    {
        public int NoteId { get; set; }
        public int EmployeeId { get; set; }
        public string NoteType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateNoteDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(20)]
        public string NoteType { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }

    public class UpdateNoteDto
    {
        [Required]
        public int NoteId { get; set; }

        [Required]
        [StringLength(20)]
        public string NoteType { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }

    public class NoteFilterDto
    {
        public int? EmployeeId { get; set; }
        public string? NoteType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
