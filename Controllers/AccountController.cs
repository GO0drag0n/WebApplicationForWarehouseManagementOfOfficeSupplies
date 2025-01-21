using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using System.Threading.Tasks;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using System.Security.Claims;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Check if the user is a company owner
            var isCompanyOwner = await _userManager.IsInRoleAsync(user, "Company Owner");

            string companyName = null;
            Guid companyId = Guid.Empty;

            if (isCompanyOwner)
            {
                // Retrieve the company details
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.OwnerId == user.Id);
                if (company != null)
                {
                    companyName = company.CompanyName;
                    companyId = company.CompanyId;
                }
            }

            // Create the view model
            var model = new ManageAccountViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsCompanyOwner = isCompanyOwner,
                CompanyName = companyName,
                CompanyId = companyId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ManageAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Manage", model);
            }

            var user = await _userManager.GetUserAsync(User);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email; // Ensure the username matches the email if needed

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Manage", model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ManageAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Manage", model);
            }

            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Manage", model);
            }

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyCode(Guid companyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return BadRequest(new { message = "User is not authenticated." });
            }

            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == companyId && c.OwnerId == userId);

            if (company == null)
            {
                return BadRequest(new { message = "Invalid company or unauthorized access." });
            }

            return Json(new { CompanyName = company.CompanyName, CompanyId = company.CompanyId });
        }



    }
}
