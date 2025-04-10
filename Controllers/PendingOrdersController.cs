using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using System.Linq;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    public class PendingOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PendingOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Storage Worker")]
        // GET: PendingOrders
        public IActionResult Pending()
        {
            var pendingRequests = _context.Requests
                .Include(r => r.User)
                .Include(r => r.RequestProducts)
                    .ThenInclude(rp => rp.Product)
                // Exclude finished orders by ensuring FinishedOrderDate is null
                .Where(r => r.Status != "Sent" && r.FinishedOrderDate == null)
                .ToList();

            return View(pendingRequests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.Requests
                .Include(r => r.Company)
                .Include(r => r.RequestProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestID == id);

            if (request == null)
            {
                return NotFound();
            }

            var viewModel = new PendingOrdersDetailsViewModel
            {
                RequestID = request.RequestID,
                Status = request.Status,
                CompanyName = request.Company?.CompanyName,
                CompanyAddress = request.Company?.CompanyAddress,
                CompanyPhone = request.Company?.CompanyPhone,
                UserName = request.User?.UserName,
                CreatedAt = request.CreatedAt,
                TotalPrice = request.TotalPrice,
                VATNumber = request.Company?.VATNumber,
                DiscountLevel = request.Company.DiscountLevel,
                RequestProducts = request.RequestProducts?.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductBrand = p.ProductBrand,
                    ProductModel = p.ProductModel,
                    ProductQuantity = p.Quantity,
                    ProductRow = p.ProductRow,
                    ProductSection = p.ProductSection
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int requestId, string newStatus)
        {
            // Fetch the request
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.RequestID == requestId);
            if (request == null)
            {
                return NotFound("Order not found.");
            }

            // Update the status
            request.Status = newStatus;

            // If the new status indicates that the order is finished, set the FinishedOrderDate.
            if (newStatus == "Delivered" || newStatus == "Finished")
            {
                request.FinishedOrderDate = DateTime.UtcNow;
            }
            else
            {
                // Optionally, reset FinishedOrderDate if the status reverts to a non-finished state.
                request.FinishedOrderDate = null;
            }

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order status updated to {newStatus}.";
            return RedirectToAction(nameof(Details), new { id = requestId });
        }

        [Authorize(Roles = "Company Owner,Company Worker")]
        public async Task<IActionResult> CompanyPendingOrders()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var company = await _context.Companies
                .Include(c => c.UserCompanies)
                .FirstOrDefaultAsync(c => c.OwnerId == userId || c.UserCompanies.Any(uc => uc.UserId == userId));

            if (company == null)
            {
                return NotFound("No associated company found for this user.");
            }

            var pendingRequests = await _context.Requests
                .Include(r => r.User)
                .Include(r => r.RequestProducts)
                    .ThenInclude(rp => rp.Product)
                // Only pending orders (no FinishedOrderDate)
                .Where(r => r.CompanyId == company.CompanyId && r.FinishedOrderDate == null)
                .ToListAsync();

            return View(pendingRequests);
        }

        public async Task<IActionResult> CompanyOrderDetails(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var request = await _context.Requests
                .Include(r => r.Company)
                .Include(r => r.RequestProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestID == id);

            if (request == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.UserCompanies)
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId);

            if (company == null || (company.OwnerId != userId && !company.UserCompanies.Any(uc => uc.UserId == userId)))
            {
                return Forbid();
            }

            var viewModel = new PendingOrdersDetailsViewModel
            {
                RequestID = request.RequestID,
                Status = request.Status,
                CompanyName = request.Company?.CompanyName,
                CompanyAddress = request.Company?.CompanyAddress,
                CompanyPhone = request.Company?.CompanyPhone,
                UserName = request.User?.UserName,
                CreatedAt = request.CreatedAt,
                TotalPrice = request.TotalPrice,
                RequestProducts = request.RequestProducts?.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductBrand = p.ProductBrand,
                    ProductModel = p.ProductModel,
                    ProductQuantity = p.Quantity,
                    ProductRow = p.ProductRow,
                    ProductSection = p.ProductSection
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Company Owner,Company Worker")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Fetch the order with related products
            var request = await _context.Requests
                .Include(r => r.Company)
                .Include(r => r.RequestProducts)
                    .ThenInclude(rp => rp.Product)
                .FirstOrDefaultAsync(r => r.RequestID == id);

            if (request == null)
            {
                return NotFound();
            }

            // Ensure user is part of this company
            var company = await _context.Companies
                .Include(c => c.UserCompanies)
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId);

            if (company == null || (company.OwnerId != userId && !company.UserCompanies.Any(uc => uc.UserId == userId)))
            {
                return Forbid();
            }

            // Ensure the order is still pending before canceling
            if (request.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Only pending orders can be canceled.";
                return RedirectToAction("CompanyOrderDetails", new { id });
            }

            // Restore product stock
            foreach (var requestProduct in request.RequestProducts)
            {
                var product = requestProduct.Product;
                if (product != null)
                {
                    product.Quantity += requestProduct.Quantity;
                    _context.Products.Update(product);
                }
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order has been successfully canceled, and product stock has been restored.";
            return RedirectToAction("CompanyPendingOrders");
        }

        [Authorize(Roles = "Company Owner,Company Worker")]
        public async Task<IActionResult> ConfirmDelivery(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Fetch the order
            var request = await _context.Requests
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.RequestID == id);

            if (request == null)
            {
                return NotFound();
            }

            // Ensure user is part of this company
            var company = await _context.Companies
                .Include(c => c.UserCompanies)
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId);

            if (company == null || (company.OwnerId != userId && !company.UserCompanies.Any(uc => uc.UserId == userId)))
            {
                return Forbid();
            }

            // Ensure the order status is "Sent" before confirming delivery
            if (request.Status != "Sent")
            {
                TempData["ErrorMessage"] = "Only orders with 'Sent' status can be confirmed.";
                return RedirectToAction("CompanyOrderDetails", new { id });
            }

            // Update the order status to delivered and set FinishedOrderDate
            request.Status = "Delivered";
            request.FinishedOrderDate = DateTime.UtcNow;

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order has been successfully confirmed and marked as delivered.";
            return RedirectToAction("CompanyPendingOrders");
        }
    }

}