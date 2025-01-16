using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class UserCompany
    {

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}