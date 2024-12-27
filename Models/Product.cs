using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int CategoryID { get; set; }
        public int Quantity { get; set; }
        public Guid UniqueNumber { get; set; } = Guid.NewGuid();

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Row { get; set; }
        public int Section { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; }
    }
}