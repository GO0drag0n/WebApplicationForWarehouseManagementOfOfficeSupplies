using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class AuditLog
    {
        [Key]
        public int LogID { get; set; }
        public int UserID { get; set; }
        public string ActionType { get; set; }
        public string EntityAffected { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}