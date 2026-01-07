using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Models
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Completed, Cancelled

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
      //  [RegularExpression("^[a-zA-Z\u0600-\u06FF ]+$", ErrorMessage = "Customer name must be letters only.")]
        public string CustomerName { get; set; }

        [Required]
    //    [Phone(ErrorMessage = "Invalid phone number.")]
        public string CustomerPhone { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<SaleItem> SaleItems { get; set; }
    }

    public class SaleItem
    {
        [Key]
        public int SaleItemId { get; set; }

        [Required]
        public int SaleId { get; set; }

        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

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