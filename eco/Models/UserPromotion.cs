using System.ComponentModel.DataAnnotations;

namespace eco.Models
{
    public class UserPromotion
    {
        [Key]
        public int UserPromotionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PromotionId { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required]
        public bool IsUsed { get; set; } = false;

        public DateTime? UsedDate { get; set; }

        // Навигационные свойства
        public virtual User User { get; set; } = null!;
        public virtual Promotion Promotion { get; set; } = null!;
    }
}
