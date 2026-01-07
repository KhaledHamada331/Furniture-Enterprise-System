using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be positive.")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Completed, Cancelled

        [Required]
        public string PaymentMethod { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<PurchaseItem> PurchaseItems { get; set; }
    }

    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemId { get; set; }

        [Required]
        public int PurchaseId { get; set; }

        [ForeignKey("PurchaseId")]
        public Purchase Purchase { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
    }
} 