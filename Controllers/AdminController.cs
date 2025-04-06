using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> AddStorageWorker(string email)
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
}
