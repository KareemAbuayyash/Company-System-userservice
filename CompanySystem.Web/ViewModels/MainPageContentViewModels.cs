using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Web.ViewModels
{
    public class MainPageContentViewModel
    {
        public int ContentId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }

    public class CreateMainPageContentViewModel
    {
        [Required(ErrorMessage = "Section name is required")]
        [StringLength(50, ErrorMessage = "Section name must not exceed 50 characters")]
        [Display(Name = "Section Name")]
        public string SectionName { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }

    public class EditMainPageContentViewModel
    {
        public int ContentId { get; set; }

        [Required(ErrorMessage = "Section name is required")]
        [StringLength(50, ErrorMessage = "Section name must not exceed 50 characters")]
        [Display(Name = "Section Name")]
        public string SectionName { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }

    public class MainPageContentListViewModel
    {
        public int ContentId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class DeleteMainPageContentViewModel
    {
        public int ContentId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class PublicMainPageViewModel
    {
        public string SectionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class MainPageViewModel
    {
        public List<PublicMainPageViewModel> Sections { get; set; } = new List<PublicMainPageViewModel>();
    }
}
