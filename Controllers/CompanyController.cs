using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;
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
                                                [FromServices] RoleManager<IdentityRole> roleManager,
                                                [FromServices] SignInManager<User> signInManager)
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

                if (!await userManager.IsInRoleAsync(user, "Company Owner"))
                {
                    var result = await userManager.AddToRoleAsync(user, "Company Owner");
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to assign role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                        return View("CreateCompany", model);
                    }
                    // Refresh the sign-in so that the updated role is reflected in the user's claims
                    await signInManager.RefreshSignInAsync(user);
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

        [Authorize(Roles = "Company Owner")]
        public async Task<IActionResult> Manage()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var company = await _context.Companies
                .Include(c => c.UserCompanies)
                .ThenInclude(uc => uc.User)
                .FirstOrDefaultAsync(c => c.OwnerId == userId);

            if (company == null)
            {
                return NotFound("No company found for the logged-in owner.");
            }

            var model = new ManageCompanyViewModel
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Address = company.CompanyAddress,
                PhoneNumber = company.CompanyPhone,
                Workers = company.UserCompanies
                    .Select(uc => new WorkerViewModel
                    {
                        WorkerId = uc.User.Id,
                        Name = uc.User.UserName,
                        Email = uc.User.Email
                    }).ToList()
            };

            return View("ManageCompany", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company Owner")]
        public async Task<IActionResult> UpdateCompanyInfo(ManageCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ManageCompany", model);
            }

            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == model.CompanyId);
            if (company == null)
            {
                return NotFound("Company not found.");
            }

            company.CompanyAddress = model.Address;
            company.CompanyPhone = model.PhoneNumber;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Company details updated successfully.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [Authorize(Roles = "Company Owner")]
        public async Task<IActionResult> RemoveWorker(string workerId, Guid companyId)
        {

            var userCompany = await _context.UserCompanies
                .FirstOrDefaultAsync(uc => uc.UserId == workerId && uc.CompanyId == companyId);

            if (userCompany == null)
            {
                return NotFound("Worker not found in the company.");
            }

            _context.UserCompanies.Remove(userCompany);

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == workerId);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole); 
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Worker removed and role reset successfully.";
            return RedirectToAction("ManageCompany", new { id = companyId });
        }


    }
}
