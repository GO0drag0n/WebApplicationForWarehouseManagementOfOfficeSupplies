using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class RequestHistory
    {
        [Key]
        public int HistoryID { get; set; }
        public int RequestID { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("RequestID")]
        public Request Request { get; set; }
    }
}