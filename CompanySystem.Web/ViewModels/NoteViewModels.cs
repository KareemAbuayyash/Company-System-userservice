using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Web.ViewModels
{
    public class NoteViewModel
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

    public class CreateNoteViewModel
    {
        [Required(ErrorMessage = "Employee ID is required")]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Note type is required")]
        [Display(Name = "Note Type")]
        public string NoteType { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }

    public class EditNoteViewModel
    {
        public int NoteId { get; set; }

        [Required(ErrorMessage = "Note type is required")]
        [Display(Name = "Note Type")]
        public string NoteType { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }

    public class NoteFilterViewModel
    {
        [Display(Name = "Employee ID")]
        public int? EmployeeId { get; set; }

        [Display(Name = "Note Type")]
        public string? NoteType { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class NotesIndexViewModel
    {
        public IEnumerable<NoteViewModel> Notes { get; set; } = new List<NoteViewModel>();
        public NoteFilterViewModel Filter { get; set; } = new NoteFilterViewModel();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class DeleteNoteViewModel
    {
        public int NoteId { get; set; }
        public int EmployeeId { get; set; }
        public string NoteType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
