using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("Challenges")]
    public class Challenge
    {
        [Key]
        public int ChallengeId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public double TargetValue { get; set; }

        [MaxLength(50)]
        public string? MaterialType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int RewardPoints { get; set; }

        public bool IsGroupChallenge { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();
    }
}
