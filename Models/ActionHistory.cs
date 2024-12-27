using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class ActionHistory
    {
        [Key]
        public int ActionID { get; set; }
        public int WorkerID { get; set; }
        public string ActionType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int ProductID { get; set; }
        public string Details { get; set; }

        [ForeignKey("WorkerID")]
        public Worker Worker { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
    }
}