using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Business.DTOs
{
    public class MainPageContentDto
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

    public class CreateMainPageContentDto
    {
        [Required(ErrorMessage = "Section name is required")]
        [StringLength(50, ErrorMessage = "Section name must not exceed 50 characters")]
        public string SectionName { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
    }

    public class UpdateMainPageContentDto
    {
        [Required(ErrorMessage = "Section name is required")]
        [StringLength(50, ErrorMessage = "Section name must not exceed 50 characters")]
        public string SectionName { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
    }

    public class MainPageContentListDto
    {
        public int ContentId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class PublicMainPageDto
    {
        public string SectionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
