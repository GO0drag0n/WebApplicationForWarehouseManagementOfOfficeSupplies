using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    [Authorize(Roles = "Company Owner,Company Worker")]
    public class OrderStatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderStatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /OrderStatistics/CompanyStatistics
        public async Task<IActionResult> CompanyStatistics()
        {
            // Get current user id from the claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Retrieve the company associated with the current user
            var company = await _context.Companies
                .Include(c => c.UserCompanies)
                .FirstOrDefaultAsync(c =>
                    c.OwnerId == userId || c.UserCompanies.Any(uc => uc.UserId == userId));

            if (company == null)
            {
                return NotFound("No associated company found for this user.");
            }

            // Retrieve finished orders for the company (FinishedOrderDate is not null)
            var finishedOrders = await _context.Requests
                .Where(r => r.CompanyId == company.CompanyId && r.FinishedOrderDate != null)
                .Include(r => r.RequestProducts)
                    .ThenInclude(rp => rp.Product)
                .ToListAsync();

            // Aggregate product statistics from the finished orders:
            // Group RequestProducts for this company by Product and sum the Quantity ordered.
            var productStats = await _context.RequestProducts
                .Where(rp => rp.Request.CompanyId == company.CompanyId && rp.Request.FinishedOrderDate != null)
                .Include(rp => rp.Product)
                .GroupBy(rp => new { rp.ProductID, rp.Product.Brand, rp.Product.Model })
                .Select(g => new CompanyProductStat
                {
                    ProductID = g.Key.ProductID,
                    ProductBrand = g.Key.Brand,
                    ProductModel = g.Key.Model,
                    TotalOrderedQuantity = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            // Build the view model with the aggregated information.
            var viewModel = new CompanyStatisticsViewModel
            {
                CompanyName = company.CompanyName,
                FinishedOrders = finishedOrders,
                ProductStats = productStats
            };

            return View(viewModel);
        }
    }

    // View model to pass data to the CompanyStatistics view
    public class CompanyStatisticsViewModel
    {
        public string CompanyName { get; set; }
        // List of finished orders for the company
        public List<Request> FinishedOrders { get; set; }
        // Aggregated statistics for each product ordered
        public List<CompanyProductStat> ProductStats { get; set; }
    }

    // Simple DTO for individual product statistics
    public class CompanyProductStat
    {
        public int ProductID { get; set; }
        public string ProductBrand { get; set; }
        public string ProductModel { get; set; }
        public int TotalOrderedQuantity { get; set; }
    }

}