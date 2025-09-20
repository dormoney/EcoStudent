using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eco.Models
{
    [Table("News")]
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? ImagePath { get; set; }
    }
}
