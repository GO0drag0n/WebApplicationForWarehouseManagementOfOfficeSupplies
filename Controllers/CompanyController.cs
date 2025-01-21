using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using WebApplicationForWarehouseManagementOfOfficeSupplies.ViewModels;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Company
        public IActionResult Index()
        {
            var companies = _context.Companies.ToList();
            return View(companies);
        }

        public IActionResult Create()
        {
            return View("CreateCompany"); // Explicitly specify the view name
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyCreateViewModel model,
                                                [FromServices] UserManager<User> userManager,
                                                [FromServices] RoleManager<IdentityRole> roleManager)
        {
            if (ModelState.IsValid)
            {
                // Get the current user's ID
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(); // Ensure the user is authenticated
                }

                // Ensure the "Company Owner" role exists
                if (!await roleManager.RoleExistsAsync("Company Owner"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Company Owner"));
                }

                // Retrieve the user
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Assign the user to the "Company Owner" role if not already assigned
                if (!await userManager.IsInRoleAsync(user, "Company Owner"))
                {
                    var result = await userManager.AddToRoleAsync(user, "Company Owner");
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to assign role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                        return View("CreateCompany", model);
                    }
                }

                // Create the company
                var company = new Company
                {
                    CompanyName = model.CompanyName,
                    CompanyAddress = model.CompanyAddress,
                    CompanyPhone = model.CompanyPhone,
                    OwnerId = userId // Set the OwnerId to the current user
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                return RedirectToAction("Manage", "Account");
            }

            return View("CreateCompany", model);
        }


        // GET: Company/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _context.Companies.Find(id);
            if (company == null)
            {
                return NotFound();
            }

            var model = new CompanyCreateViewModel
            {
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress,
                CompanyPhone = company.CompanyPhone
            };

            return View(model);
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CompanyCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = _context.Companies.Find(id);
                if (company == null)
                {
                    return NotFound();
                }

                company.CompanyName = model.CompanyName;
                company.CompanyAddress = model.CompanyAddress;
                company.CompanyPhone = model.CompanyPhone;

                _context.Update(company);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Company/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _context.Companies.Find(id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var company = _context.Companies.Find(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Company/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _context.Companies.Find(id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }
    }
}
