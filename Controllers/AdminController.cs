using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using WebApplicationForWarehouseManagementOfOfficeSupplies.ViewModels;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // ----- Storage Worker Methods -----

    // Displays the list of storage workers and allows adding new ones
    [HttpGet]
    public async Task<IActionResult> ManageStorageWorkers()
    {
        var storageWorkers = await _userManager.GetUsersInRoleAsync("Storage Worker");

        var model = new ManageStorageWorkersViewModel
        {
            StorageWorkers = storageWorkers.Select(user => new StorageWorkerViewModel
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email
            }).ToList()
        };

        return View(model);
    }

    // Adds a user as a storage worker, ensuring role conflicts are avoided
    [HttpPost]
    public async Task<IActionResult> AddStorageWorker(string email, [FromServices] SignInManager<User> signInManager)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("ManageStorageWorkers");
        }

        // Check if user already holds conflicting roles
        bool isCompanyOwner = await _userManager.IsInRoleAsync(user, "Company Owner");
        bool isCompanyWorker = await _userManager.IsInRoleAsync(user, "Company Worker");

        if (isCompanyOwner)
        {
            TempData["ErrorMessage"] = "User is a Company Owner and cannot be assigned as a Storage Worker.";
            return RedirectToAction("ManageStorageWorkers");
        }

        if (isCompanyWorker)
        {
            TempData["ErrorMessage"] = "User is a Company Worker and must leave the company before becoming a Storage Worker.";
            return RedirectToAction("ManageStorageWorkers");
        }

        // Check if user is already a Storage Worker
        if (await _userManager.IsInRoleAsync(user, "Storage Worker"))
        {
            TempData["ErrorMessage"] = "User is already a Storage Worker.";
            return RedirectToAction("ManageStorageWorkers");
        }

        var currentUserId = _userManager.GetUserId(User);
        if (currentUserId == user.Id)
        {
            await signInManager.RefreshSignInAsync(user);
        }

        // Assign the "Storage Worker" role
        await _userManager.AddToRoleAsync(user, "Storage Worker");
        TempData["SuccessMessage"] = "User has been successfully assigned as a Storage Worker.";

        return RedirectToAction("ManageStorageWorkers");
    }

    // Allows an admin to remove a storage worker
    [HttpPost]
    public async Task<IActionResult> RemoveStorageWorker(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("ManageStorageWorkers");
        }

        // Ensure the user is actually a Storage Worker before removing the role
        if (await _userManager.IsInRoleAsync(user, "Storage Worker"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Storage Worker");
            TempData["SuccessMessage"] = "Storage Worker removed successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "User is not a Storage Worker.";
        }

        return RedirectToAction("ManageStorageWorkers");
    }

    // ----- Category Management Methods -----

    // GET: Display a list of categories
    [HttpGet]
    public async Task<IActionResult> ManageCategories()
    {
        var categories = await _context.Categories
            .Select(c => new ManageCategoriesViewModel
            {
                CategoryID = c.CategoryID,
                CategoryName = c.Name,
                UniqueNumber = c.UniqueNumber.ToString(),
                ProductCount = _context.Products.Count(p => p.CategoryID == c.CategoryID)
            })
            .ToListAsync();

        // The view is strongly typed to IEnumerable<ManageCategoriesViewModel>
        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenameCategory(ManageCategoriesViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _context.Categories
                .Select(c => new ManageCategoriesViewModel
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.Name,
                    UniqueNumber = c.UniqueNumber.ToString(),
                    ProductCount = _context.Products.Count(p => p.CategoryID == c.CategoryID)
                })
                .ToListAsync();
            return View("ManageCategories", categories);
        }

        var category = await _context.Categories.FindAsync(model.CategoryID);
        if (category == null)
        {
            return NotFound();
        }

        // Update only the CategoryName
        category.Name = model.CategoryName;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ManageCategories));
    }

    // POST: Delete a category
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageCategories));
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (category == null || string.IsNullOrWhiteSpace(category.Name))
        {
            return BadRequest("Category name cannot be empty.");
        }

        try
        {
            // Save the new category
            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageCategories));

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> OrderHistory()
    {
        var orders = await _context.Requests
            .Include(r => r.Company)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View("OrderHistory", orders);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> OrderStatistics()
    {
        var requests = await _context.Requests
            .Include(r => r.Company)
            .Where(r => r.CreatedAt != null)
            .ToListAsync();

        var totalOrders = requests.Count;
        var deliveredOrders = requests.Count(r => r.Status == "Delivered");
        var pendingOrders = requests.Count(r => r.Status == "Pending");
        var rejectedOrders = requests.Count(r => r.Status == "Rejected");
        var inProgressOrders = requests.Count(r => r.Status == "Pending" || r.Status == "Sent");

        var now = DateTime.UtcNow;
        var ordersThisWeek = requests.Count(r => r.CreatedAt >= now.AddDays(-7));
        var ordersThisMonth = requests.Count(r => r.CreatedAt >= now.AddDays(-30));
        var ordersThisYear = requests.Count(r => r.CreatedAt >= now.AddDays(-365));

        var weeklyTrend = requests
            .GroupBy(r => r.CreatedAt.Date)  // Group by each unique day
            .Select(g => new { Week = g.Key, Count = g.Count() })
            .OrderBy(g => g.Week)
            .ToList();




        var topCompaniesByOrders = requests
            .GroupBy(r => r.Company.CompanyName)
            .Select(g => new CompanyOrderStat
            {
                Company = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToList();

        var topCompaniesBySpending = requests
            .GroupBy(r => r.Company.CompanyName)
            .Select(g => new CompanySpendingStat
            {
                Company = g.Key,
                Total = g.Sum(r => r.TotalPrice)
            })
            .OrderByDescending(g => g.Total)
            .Take(5)
            .ToList();


        var fulfillmentTimes = requests
            .Where(r => r.Status == "Delivered" && r.FinishedOrderDate.HasValue)
            .Select(r => (r.FinishedOrderDate.Value - r.CreatedAt).TotalDays)
            .ToList();

        var avgFulfillmentTime = fulfillmentTimes.Any() ? fulfillmentTimes.Average() : 0;

        var model = new AdminOrderStatisticsViewModel
        {
            TotalOrders = totalOrders,
            DeliveredOrders = deliveredOrders,
            PendingOrders = pendingOrders,
            RejectedOrders = rejectedOrders,
            InProgressOrders = inProgressOrders,
            OrdersThisWeek = ordersThisWeek,
            OrdersThisMonth = ordersThisMonth,
            OrdersThisYear = ordersThisYear,
            WeeklyOrderTrend = weeklyTrend.Select(t => new WeeklyOrderStat { Week = t.Week, OrderCount = t.Count }).ToList(),
            TopCompaniesByOrders = topCompaniesByOrders,
            TopCompaniesBySpending = topCompaniesBySpending,
            AvgFulfillmentTimeDays = Math.Round(avgFulfillmentTime, 1)
        };

        return View(model);
    }




}
