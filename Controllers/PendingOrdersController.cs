using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using System.Linq;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    [Authorize(Roles = "Admin,Storage Worker")]
    public class PendingOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PendingOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PendingOrders
        public IActionResult Pending()
        {
            var pendingRequests = _context.Requests
                .Include(r => r.User)
                .Include(r => r.RequestProducts)
                .ThenInclude(rp => rp.Product)
                .Where(r => r.Status == "Pending")
                .ToList();

            return View(pendingRequests);
        }

        public async Task<IActionResult> Details(int id)
        {
            // Fetch the request with related data
            var request = await _context.Requests
                .Include(r => r.Company) // Include related Company entity
                .Include(r => r.RequestProducts) // Include related RequestProducts
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestID == id);

            if (request == null)
            {
                return NotFound(); // Handle case where the request is not found
            }

            // Map data to the view model
            var viewModel = new PendingOrdersDetailsViewModel
            {
                RequestID = request.RequestID,
                Status = request.Status,
                CompanyName = request.Company?.CompanyName, // Use null-safe navigation
                CompanyAddress = request.Company?.CompanyAddress,
                CompanyPhone = request.Company?.CompanyPhone,
                UserName = request.User?.UserName,
                RequestProducts = request.RequestProducts?.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductBrand = p.ProductBrand,
                    ProductModel = p.ProductModel,
                    ProductQuantity = p.Quantity
                }).ToList()
            };

            return View(viewModel);
        }

    }
}
