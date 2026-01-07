using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
       
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[a-zA-Z\u0600-\u06FF ]+$", ErrorMessage = "First name must be letters only.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[a-zA-Z\u0600-\u06FF ]+$", ErrorMessage = "Last name must be letters only.")]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; } // Admin, Manager, Employee

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 