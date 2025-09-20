using System.ComponentModel.DataAnnotations;

namespace eco.Models
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int PointsCost { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string? DiscountValue { get; set; } // Например: "30%", "50 руб", "Бесплатно"

        // Навигационные свойства
        public virtual ICollection<UserPromotion> UserPromotions { get; set; } = new List<UserPromotion>();
    }
}
