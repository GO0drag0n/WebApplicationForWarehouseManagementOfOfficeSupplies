using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        [Required]
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        // Navigation property for request history
        public ICollection<RequestHistory> RequestHistories { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<RequestProduct> RequestProducts { get; set; }
    }
}
