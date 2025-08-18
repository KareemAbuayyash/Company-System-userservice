using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Data.Models
{
    public class MainPageContent : TrackingEntity
    {
        [Key]
        public int ContentId { get; set; }

        [Required]
        [StringLength(50)]
        public string SectionName { get; set; } = string.Empty;  // Overview, AboutUs, Services

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty; 

        [Required]
        public string Content { get; set; } = string.Empty; 
    }
}
