using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Data.Models
{
    public abstract class TrackingEntity
    {
        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
