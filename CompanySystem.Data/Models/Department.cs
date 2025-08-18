using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Data.Models
{
    public class Department : TrackingEntity
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(200)]
        public string DepartmentName { get; set; }
    }
}
