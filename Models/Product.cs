using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be positive.")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be positive.")]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative.")]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock level must be non-negative.")]
        public int MinimumStockLevel { get; set; }

        [Required]
        public string Category { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 