using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public Guid UniqueNumber { get; set; } = Guid.NewGuid();
    }
}