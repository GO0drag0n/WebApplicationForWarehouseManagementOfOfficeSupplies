namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class PendingOrdersDetailsViewModel
    {
        public int RequestID { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; } // Ensure order creation date is included
        public decimal TotalPrice { get; set; }
        public string NewStatus { get; set; }
        public List<ProductViewModel> RequestProducts { get; set; }

    }
}
