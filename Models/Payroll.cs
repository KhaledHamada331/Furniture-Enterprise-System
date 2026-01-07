using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public DateTime PayPeriodStart { get; set; }

        [Required]
        public DateTime PayPeriodEnd { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base salary must be positive.")]
        public decimal BaseSalary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Overtime pay must be non-negative.")]
        public decimal OvertimePay { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bonuses must be non-negative.")]
        public decimal Bonuses { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Deductions must be non-negative.")]
        public decimal Deductions { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Net salary must be positive.")]
        public decimal NetSalary { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Paid

        public DateTime? PaymentDate { get; set; }

        public string PaymentMethod { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 