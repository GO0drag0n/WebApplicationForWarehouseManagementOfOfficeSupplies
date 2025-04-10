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

        public string Brand { get; set; }

        public string Model { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedOrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        // Navigation property for request history
        public ICollection<RequestHistory> RequestHistories { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<RequestProduct> RequestProducts { get; set; }
    }
}
