using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}