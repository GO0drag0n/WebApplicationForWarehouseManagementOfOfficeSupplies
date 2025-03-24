using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class RequestProduct
    {
        public int RequestID { get; set; }
        [ForeignKey("RequestID")]
        public Request Request { get; set; }

        public int ProductID { get; set; }

        public string ProductBrand { get; set; }

        public string ProductModel { get; set; }

        public int ProductRow { get; set; }

        public int ProductSection { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        [Required]
        [Range(-1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
