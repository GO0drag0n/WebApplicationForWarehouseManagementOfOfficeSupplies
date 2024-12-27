using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Worker
    {
        [Key]
        public int WorkerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int BirthYear { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime JoinDate { get; set; }
    }
}