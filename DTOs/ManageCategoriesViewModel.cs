using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class ManageCategoriesViewModel
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Unique Number")]
        public string UniqueNumber { get; set; }

        [Display(Name = "Product Count")]
        public int ProductCount { get; set; }
    }

}
