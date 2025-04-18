using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    public class AdminManageCompanyClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminManageCompanyClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies.ToListAsync();

            var viewModel = companies.Select(c => new AdminManageCompanyClientsViewModel
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,
                CompanyAddress = c.CompanyAddress,
                CompanyPhone = c.CompanyPhone,
                VATNumber = c.VATNumber,
                QuarterOrderValue = c.QuarterOrderValue,
                DiscountLevel = c.DiscountLevel
            }).ToList();

            return View("~/Views/Admin/AdminManageCompanyClients.cshtml", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }

            var viewModel = new AdminManageCompanyClientsViewModel
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress,
                CompanyPhone = company.CompanyPhone,
                VATNumber = company.VATNumber,
                QuarterOrderValue = company.QuarterOrderValue,
                DiscountLevel = company.DiscountLevel
            };

            return View("~/Views/Admin/EditCompanyClients.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AdminManageCompanyClientsViewModel viewModel)
        {
            if (id != viewModel.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var company = await _context.Companies.FindAsync(id);
                    if (company == null)
                    {
                        return NotFound();
                    }

                    // Update the company's details
                    company.CompanyName = viewModel.CompanyName;
                    company.CompanyAddress = viewModel.CompanyAddress;
                    company.CompanyPhone = viewModel.CompanyPhone;
                    company.VATNumber = viewModel.VATNumber;
                    company.QuarterOrderValue = viewModel.QuarterOrderValue;
                    company.DiscountLevel = viewModel.DiscountLevel;

                    // Save changes
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(viewModel.CompanyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));  
            }
            return View(viewModel);
        }


        private bool CompanyExists(Guid id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyDetails(Guid id)
        {
            var company = await _context.Companies
            .Where(c => c.CompanyId == id)
            .Select(c => new AdminManageCompanyClientsViewModel
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,
                CompanyAddress = c.CompanyAddress,
                CompanyPhone = c.CompanyPhone,
                VATNumber = c.VATNumber,
                QuarterOrderValue = c.QuarterOrderValue,
                DiscountLevel = c.DiscountLevel
            })
            .FirstOrDefaultAsync();

            if (company == null)
            {
                return NotFound();
            }

            return Json(company);
        }

    }
}
