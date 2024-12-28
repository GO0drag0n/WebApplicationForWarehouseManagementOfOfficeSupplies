using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class RequestHistory
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }
        public string UpdatedByUserId { get; set; }
        public User UpdatedBy { get; set; }
    }
}