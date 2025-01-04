using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Role : IdentityRole
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}