using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("RecyclingSubmissions")]
    public class RecyclingSubmission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PointId { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaterialType { get; set; } = string.Empty;

        [Required]
        public double WeightKg { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? PhotoPath { get; set; }

        public bool IsVerified { get; set; } = false;

        [Required]
        public int PointsAwarded { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual RecyclingPoint RecyclingPoint { get; set; } = null!;
    }
}
