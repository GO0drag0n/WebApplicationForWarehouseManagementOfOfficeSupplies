using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; }
        public int CategoryID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "Pending";
        public string UserID { get; set; }

        public ICollection<RequestHistory> RequestHistories { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}