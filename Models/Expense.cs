using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string ReceiptNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 