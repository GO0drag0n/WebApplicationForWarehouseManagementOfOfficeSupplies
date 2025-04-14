using Microsoft.AspNetCore.Mvc;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class AdminOrderStatisticsViewModel
    {
        public int TotalOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int PendingOrders { get; set; }
        public int RejectedOrders { get; set; }
        public int InProgressOrders { get; set; }

        public int OrdersThisWeek { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersThisYear { get; set; }

        public List<WeeklyOrderStat> WeeklyOrderTrend { get; set; } = new();
        public List<CompanyOrderStat> TopCompaniesByOrders { get; set; } = new();
        public List<CompanySpendingStat> TopCompaniesBySpending { get; set; } = new();

        public double AvgFulfillmentTimeDays { get; set; }
    }

    public class WeeklyOrderStat
    {
        public DateTime Week { get; set; }
        public int OrderCount { get; set; }
    }

    public class CompanyOrderStat
    {
        public string Company { get; set; }
        public int Count { get; set; }
    }

    public class CompanySpendingStat
    {
        public string Company { get; set; }
        public decimal Total { get; set; }
    }

}
