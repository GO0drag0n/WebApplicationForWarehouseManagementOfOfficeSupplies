using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // === Registration Step 1 ===
        [HttpGet]
        public IActionResult RegisterStep1()
            => View(new RegisterStep1ViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterStep1(RegisterStep1ViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["FirstName"] = model.FirstName;
            TempData["LastName"] = model.LastName;
            return RedirectToAction(nameof(RegisterStep2));
        }

        // === Registration Step 2 ===
        [HttpGet]
        public IActionResult RegisterStep2()
        {
            var firstName = TempData["FirstName"] as string;
            var lastName = TempData["LastName"] as string;
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                return RedirectToAction(nameof(RegisterStep1));

            TempData.Keep("FirstName");
            TempData.Keep("LastName");

            return View(new RegisterStep2ViewModel
            {
                FirstName = firstName,
                LastName = lastName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep2(RegisterStep2ViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // === Login / Logout ===
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // === Manage Account ===
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            // 1) Pull any TempData‐stored errors into ModelState
            if (TempData["PwdErrors"] is string[] pwdErrors)
            {
                foreach (var err in pwdErrors)
                    ModelState.AddModelError(string.Empty, err);
            }
            if (TempData["ErrorMessage"] is string errMsg)
                ModelState.AddModelError(string.Empty, errMsg);

            // SuccessMessage stays in TempData to be rendered by the view

            // 2) Build the standard ManageAccountViewModel
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction(nameof(Login));

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isCompanyOwner = await _userManager.IsInRoleAsync(user, "Company Owner");
            var isCompanyWorker = await _userManager.IsInRoleAsync(user, "Company Worker");

            string companyName = null;
            Guid companyId = Guid.Empty;
            if (isCompanyOwner || isCompanyWorker)
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.OwnerId == user.Id ||
                                              c.UserCompanies.Any(uc => uc.UserId == user.Id));
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
                IsAdmin = isAdmin,
                IsCompanyOwner = isCompanyOwner,
                IsCompanyWorker = isCompanyWorker,
                CompanyName = companyName,
                CompanyId = companyId
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
            => RedirectToAction(nameof(Manage));

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ManageAccountViewModel form)
        {
            var errors = new List<string>();

            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values
                                   .SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage));
                TempData["PwdErrors"] = errors.ToArray();
                return RedirectToAction(nameof(Manage));
            }

            var user = await _userManager.GetUserAsync(User);
            IdentityResult result;
            if (!await _userManager.HasPasswordAsync(user))
                result = await _userManager.AddPasswordAsync(user, form.NewPassword);
            else
                result = await _userManager.ChangePasswordAsync(
                             user, form.CurrentPassword, form.NewPassword);

            if (!result.Succeeded)
            {
                errors.AddRange(result.Errors.Select(e => e.Description));
                TempData["PwdErrors"] = errors.ToArray();
                return RedirectToAction(nameof(Manage));
            }


            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ManageAccountViewModel model)
        {
            ModelState.Remove(nameof(model.CurrentPassword));
            ModelState.Remove(nameof(model.NewPassword));
            ModelState.Remove(nameof(model.ConfirmPassword));

            if (!ModelState.IsValid)
                return View("Manage", model);

            var user = await _userManager.GetUserAsync(User);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return View("Manage", model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyCode(Guid companyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return BadRequest(new { message = "User not authenticated." });

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.OwnerId == userId);
            if (company == null)
                return BadRequest(new { message = "Invalid company or unauthorized." });

            return Json(new { company.CompanyName, company.CompanyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinCompany(string companyCode)
        {
            if (string.IsNullOrWhiteSpace(companyCode))
            {
                TempData["ErrorMessage"] = "Company code is required.";
                return RedirectToAction(nameof(Manage));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction(nameof(Login));
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId.ToString() == companyCode);
            if (company == null)
            {
                TempData["ErrorMessage"] = "Invalid company code.";
                return RedirectToAction(nameof(Manage));
            }

            var already = await _context.UserCompanies
                .AnyAsync(uc => uc.UserId == user.Id && uc.CompanyId == company.CompanyId);
            if (already)
            {
                TempData["ErrorMessage"] = "You are already a member of this company.";
                return RedirectToAction(nameof(Manage));
            }

            _context.UserCompanies.Add(new UserCompany
            {
                UserId = user.Id,
                CompanyId = company.CompanyId
            });

            if (!await _userManager.IsInRoleAsync(user, "Company Worker"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Company Worker");
                if (!roleResult.Succeeded)
                {
                    TempData["ErrorMessage"] =
                        "Failed to assign role: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Manage));
                }
                await _signInManager.RefreshSignInAsync(user);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Joined company: {company.CompanyName}.";
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveCompany()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction(nameof(Login));
            }

            if (await _userManager.IsInRoleAsync(user, "Company Owner"))
            {
                TempData["ErrorMessage"] = "Company owners cannot leave directly.";
                return RedirectToAction(nameof(Manage));
            }

            var membership = await _context.UserCompanies
                .FirstOrDefaultAsync(uc => uc.UserId == user.Id);
            if (membership == null)
            {
                TempData["ErrorMessage"] = "You are not a member of any company.";
                return RedirectToAction(nameof(Manage));
            }

            _context.UserCompanies.Remove(membership);
            await _context.SaveChangesAsync();

            if (await _userManager.IsInRoleAsync(user, "Company Worker"))
            {
                var roleRemove = await _userManager.RemoveFromRoleAsync(user, "Company Worker");
                if (!roleRemove.Succeeded)
                {
                    TempData["ErrorMessage"] =
                        "Error removing your company worker role.";
                    return RedirectToAction(nameof(Manage));
                }
                await _signInManager.RefreshSignInAsync(user);
            }

            TempData["SuccessMessage"] = "You have successfully left the company.";
            return RedirectToAction(nameof(Manage));
        }

    }
}
