using System.ComponentModel.DataAnnotations;

namespace WebApplicationFES.Models
{
    public class ProductDisplay
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
} 