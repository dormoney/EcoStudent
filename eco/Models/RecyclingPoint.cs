using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("RecyclingPoints")]
    public class RecyclingPoint
    {
        [Key]
        public int PointId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? WorkHours { get; set; }

        [MaxLength(255)]
        public string? AcceptedTypes { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        // Navigation properties
        public virtual ICollection<RecyclingSubmission> RecyclingSubmissions { get; set; } = new List<RecyclingSubmission>();
    }
}
