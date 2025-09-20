using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public int? GroupId { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public int TotalPoints { get; set; } = 0;

        [MaxLength(255)]
        public string? AvatarPath { get; set; }

        // Navigation properties
        public virtual Group? Group { get; set; }
        public virtual ICollection<RecyclingSubmission> RecyclingSubmissions { get; set; } = new List<RecyclingSubmission>();
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
        public virtual ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();
        public virtual ICollection<UserPromotion> UserPromotions { get; set; } = new List<UserPromotion>();
    }
}
