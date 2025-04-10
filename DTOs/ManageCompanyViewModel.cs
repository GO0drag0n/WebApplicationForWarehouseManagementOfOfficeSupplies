using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class ManageCompanyViewModel
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string VATNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuarterOrderValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountLevel { get; set; }

        public List<WorkerViewModel> Workers { get; set; } = new List<WorkerViewModel>();

        public List<CompanyProductStatViewModel> OrderStats { get; set; } = new List<CompanyProductStatViewModel>();
    }

    // DTO representing the aggregated stats for each product in finished orders.
    public class CompanyProductStatViewModel
    {
        public int ProductID { get; set; }
        public string ProductBrand { get; set; }
        public string ProductModel { get; set; }
        public int TotalOrderedQuantity { get; set; }
    }

}