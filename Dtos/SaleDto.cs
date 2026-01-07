using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationFES.Dtos
{
    public class SaleDto
    {
        public string PaymentMethod { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        public string Notes { get; set; }

    }
} 