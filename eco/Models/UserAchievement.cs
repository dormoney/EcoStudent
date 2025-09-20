using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("UserAchievements")]
    public class UserAchievement
    {
        [Key]
        public int UserAchievementId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AchievementId { get; set; }

        public DateTime DateEarned { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Achievement Achievement { get; set; } = null!;
    }
}
