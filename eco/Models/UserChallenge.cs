using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("UserChallenges")]
    public class UserChallenge
    {
        [Key]
        public int UserChallengeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ChallengeId { get; set; }

        public double CurrentValue { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletionDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Challenge Challenge { get; set; } = null!;
    }
}
