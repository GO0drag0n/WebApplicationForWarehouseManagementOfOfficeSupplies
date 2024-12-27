using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class UserRole
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("RoleID")]
        public Role Role { get; set; }
    }
}