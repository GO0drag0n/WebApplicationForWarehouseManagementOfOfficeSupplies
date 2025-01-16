using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }

        public string CompanyName { get; set; }

        // Navigation property for the many-to-many relationship
        public ICollection<UserCompany> UserCompanies { get; set; }
    }
}
