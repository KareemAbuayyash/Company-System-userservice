using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanySystem.Data.Models
{
    public class Note : TrackingEntity
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(20)]
        public string NoteType { get; set; } // Technical or Behavioral

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string Content { get; set; }

        // Navigation properties can be added later when User/Employee entity is implemented
    }

    public static class NoteTypes
    {
        public const string Technical = "Technical";
        public const string Behavioral = "Behavioral";
    }
}
