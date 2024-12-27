using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}