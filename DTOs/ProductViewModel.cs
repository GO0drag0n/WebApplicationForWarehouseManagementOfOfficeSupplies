using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Brand is required.")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters.")]
        public string ProductBrand { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters.")]
        public string ProductModel { get; set; }
        [Required]
        public int ProductCategoryID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int ProductQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Delivery Price must be a positive value.")]
        public decimal ProductDeliveryPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Row is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Row must be greater than 0.")]
        public int ProductRow { get; set; }

        [Required(ErrorMessage = "Section is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Section must be greater than 0.")]
        public int ProductSection { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
