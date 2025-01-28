    using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class CreateRequestViewModel
    {
        [Required]
        public string RequestStatus { get; set; } = "Pending"; // Default status

        public string? RequestUserID { get; set; } // Auto-filled by the controller

        [Required(ErrorMessage = "At least one product is required.")]

        public string RequestProductBrand { get; set; }

        public string RequestProductModel { get; set; }

        public List<ProductViewModel> RequestProducts { get; set; } = new List<ProductViewModel>();

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
