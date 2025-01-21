using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.ViewModels
{
    public class CompanyCreateViewModel
    {
        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(100, ErrorMessage = "Company Name cannot exceed 100 characters.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Company Address is required.")]
        [StringLength(100, ErrorMessage = "Company Address cannot exceed 100 characters.")]
        public string CompanyAddress { get; set; }

        [Required(ErrorMessage = "Company Phone is required.")]
        [StringLength(100, ErrorMessage = "Company Phone cannot exceed 100 characters.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string CompanyPhone { get; set; }
    }
}
