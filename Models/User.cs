using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class User : IdentityUser
    {
        // Additional properties
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigation property for roles
        public ICollection<UserRole> UserRoles { get; set; }
    }
}