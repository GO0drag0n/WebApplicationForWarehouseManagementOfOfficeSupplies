using System.ComponentModel.DataAnnotations;

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

        public List<WorkerViewModel> Workers { get; set; } = new List<WorkerViewModel>();
    }
}
