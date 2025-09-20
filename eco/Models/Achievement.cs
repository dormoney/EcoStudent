using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("Achievements")]
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? IconPath { get; set; }

        [Required]
        public int RequiredPoints { get; set; }

        [MaxLength(50)]
        public string? MaterialType { get; set; }

        // Navigation properties
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
