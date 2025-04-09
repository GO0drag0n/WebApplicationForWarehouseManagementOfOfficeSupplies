using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class PendingOrdersDetailsViewModel
    {
        public int RequestID { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; } // Ensure order creation date is included
        public decimal TotalPrice { get; set; }
        public string NewStatus { get; set; }
        [Required]
        [StringLength(50)]
        public string VATNumber { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuarterOrderValue { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountLevel { get; set; }
        public List<ProductViewModel> RequestProducts { get; set; }

    }
}
