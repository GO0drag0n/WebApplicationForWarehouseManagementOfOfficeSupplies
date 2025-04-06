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

/*        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }*/

        [HttpGet]
        public IActionResult RegisterStep1()
        {
            return View(new RegisterStep1ViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterStep1(RegisterStep1ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Store first and last name in TempData for use in Step 2.
            TempData["FirstName"] = model.FirstName;
            TempData["LastName"] = model.LastName;
            return RedirectToAction("RegisterStep2");
        }

        // STEP 2: Collect Email, Password, and Confirm Password
        [HttpGet]
        public IActionResult RegisterStep2()
        {
            // Retrieve first and last name from TempData.
            var firstName = TempData["FirstName"] as string;
            var lastName = TempData["LastName"] as string;
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                // If values are missing, redirect back to Step 1.
                return RedirectToAction("RegisterStep1");
            }

            // Keep TempData values so they're available for the POST.
            TempData.Keep("FirstName");
            TempData.Keep("LastName");

            var model = new RegisterStep2ViewModel
            {
                FirstName = firstName,
                LastName = lastName
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep2(RegisterStep2ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Create the new user using values from both steps.
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
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin"); // Check if the user is an Admin
            var isCompanyOwner = await _userManager.IsInRoleAsync(user, "Company Owner");
            var isCompanyWorker = await _userManager.IsInRoleAsync(user, "Company Worker");

            string companyName = null;
            Guid companyId = Guid.Empty;

            if (isCompanyOwner || isCompanyWorker)
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.OwnerId == user.Id || c.UserCompanies.Any(uc => uc.UserId == user.Id));

                if (company != null)
                {
                    companyName = company.CompanyName;
                    companyId = company.CompanyId;
                }
            }

            var model = new ManageAccountViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsAdmin = isAdmin, // Set IsAdmin to true if the user is in the Admin role
                IsCompanyOwner = isCompanyOwner,
                IsCompanyWorker = isCompanyWorker,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinCompany(string companyCode)
        {
            if (string.IsNullOrWhiteSpace(companyCode))
            {
                TempData["ErrorMessage"] = "Company code is required.";
                return RedirectToAction("Manage");
            }

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login");
            }

            // Validate the company code
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId.ToString() == companyCode);
            if (company == null)
            {
                TempData["ErrorMessage"] = "Invalid company code.";
                return RedirectToAction("Manage");
            }

            // Check if the user is already a member of this company
            var existingMembership = await _context.UserCompanies
                .AnyAsync(uc => uc.UserId == user.Id && uc.CompanyId == company.CompanyId);

            if (existingMembership)
            {
                TempData["ErrorMessage"] = "You are already a member of this company.";
                return RedirectToAction("Manage");
            }

            // Add the user as a worker to the company
            var userCompany = new UserCompany
            {
                UserId = user.Id,
                CompanyId = company.CompanyId
            };
            _context.UserCompanies.Add(userCompany);

            // Add user to the "Company Worker" role
            if (!await _userManager.IsInRoleAsync(user, "Company Worker"))
            {
                await _userManager.AddToRoleAsync(user, "Company Worker");
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"You have successfully joined the company: {company.CompanyName}.";
            return RedirectToAction("Manage");
        }




    }
}
