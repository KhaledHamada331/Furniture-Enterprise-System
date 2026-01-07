using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression("^[a-zA-Z\u0600-\u06FF ]+$", ErrorMessage = "Company name must be letters only.")]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression("^[a-zA-Z\u0600-\u06FF ]+$", ErrorMessage = "Contact person must be letters only.")]
        public string ContactPerson { get; set; }

        [Required]
        [StringLength(20)]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        public string TaxNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 