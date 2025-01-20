using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
