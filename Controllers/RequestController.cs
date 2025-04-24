using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    [Authorize(Roles = "Company Owner, Company Worker")]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RequestController(ApplicationDbContext context) => _context = context;

        // GET: Request/Create
        public async Task<IActionResult> Create(string searchTerm = "", int? categoryId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query
                    .Include(x => x.RequestProducts)
                    .Include(x => x.Category)
                    .Where(p => p.Brand.Contains(searchTerm) || p.Model.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query
                    .Include(x => x.RequestProducts)
                    .Include(x => x.Category)
                    .Where(p => p.CategoryID == categoryId.Value);
            }

            var filteredProducts = query
                .Include(x => x.RequestProducts)
                .Include(x => x.Category)
                .ToList();

            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryID.ToString(),
                    Text = c.Name
                })
                .ToList();

            var viewModel = new CreateRequestViewModel
            {
                RequestProducts = filteredProducts.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductBrand = p.Brand,
                    ProductModel = p.Model,
                    ProductCategoryID = p.CategoryID,
                    ProductRow = p.Row > 0 ? p.Row : 1,
                    ProductSection = p.Section > 0 ? p.Section : 1,
                    ProductPrice = p.Price,
                    ProductDeliveryPrice = p.DeliveryPrice,
                    ProductQuantity = p.Quantity
                }).ToList(),
                Categories = categories
            };

            // Populate current discount level & QOV for JS
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var company = await _context.Companies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.OwnerId == userId);
                if (company != null)
                {
                    viewModel.CurrentDiscountLevel = (int)company.DiscountLevel;
                    viewModel.CurrentQuarterOrderValue = company.QuarterOrderValue;
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var product in model.RequestProducts)
                {
                    var dbProduct = _context.Products.FirstOrDefault(p => p.ProductID == product.ProductID);
                    if (dbProduct != null)
                    {
                        product.ProductBrand = dbProduct.Brand;
                        product.ProductModel = dbProduct.Model;
                        product.ProductCategoryID = dbProduct.CategoryID;
                        product.ProductRow = dbProduct.Row > 0 ? dbProduct.Row : 1;
                        product.ProductSection = dbProduct.Section > 0 ? dbProduct.Section : 1;
                        product.ProductPrice = dbProduct.Price;
                        product.ProductDeliveryPrice = dbProduct.DeliveryPrice;
                    }
                }
            }

            var selectedProducts = model.RequestProducts
        .Where(p => p.ProductQuantity > 0)
        .ToList();

            if (!selectedProducts.Any())
            {
                ModelState.AddModelError(string.Empty,
                    "You must select at least one product with a quantity greater than 0.");
                return View(model);
            }

            decimal totalPrice = 0m;
            foreach (var product in selectedProducts)
            {
                var dbProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == product.ProductID);
                if (dbProduct == null)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Product {product.ProductBrand} {product.ProductModel} does not exist.");
                    return View(model);
                }
                if (product.ProductQuantity > dbProduct.Quantity)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Not enough stock for {product.ProductBrand} {product.ProductModel}. " +
                        $"Available: {dbProduct.Quantity}, Requested: {product.ProductQuantity}.");
                    return View(model);
                }

                dbProduct.Quantity -= product.ProductQuantity;
                _context.Entry(dbProduct).State = EntityState.Modified;
                totalPrice += product.ProductQuantity * dbProduct.Price;
            }

            // Fill in brand/model for the Request header
            var first = selectedProducts.First();
            model.RequestProductBrand = first.ProductBrand;
            model.RequestProductModel = first.ProductModel;

            // 1) get current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "User is not logged in.");
                return View(model);
            }

            // 2) resolve Company
            Company companyPost = null;

            if (User.IsInRole("Company Owner"))
            {
                companyPost = await _context.Companies
                    .FirstOrDefaultAsync(c => c.OwnerId == userId);
            }
            else if (User.IsInRole("Company Worker"))
            {
                // look up via UserCompany join‐table
                var uc = await _context.UserCompanies
                    .Include(uc => uc.Company)
                    .FirstOrDefaultAsync(uc => uc.UserId == userId);

                companyPost = uc?.Company;
            }

            if (companyPost == null)
            {
                ModelState.AddModelError(string.Empty,
                    "No associated company found for the logged-in user.");
                return View(model);
            }

            // 3) Discount calculation
            decimal prospectiveQov = companyPost.QuarterOrderValue + totalPrice;
            int newLevel = (int)(prospectiveQov / 1000m);
            newLevel = Math.Min(newLevel, 5);
            decimal discountPct = newLevel * 0.025m;
            decimal discountedTotal = totalPrice * (1 - discountPct);

            // 4) create & save Request
            var request = new Request
            {
                UserID = userId,
                Status = model.RequestStatus,
                Brand = model.RequestProductBrand,
                Model = model.RequestProductModel,
                CompanyId = companyPost.CompanyId,
                CreatedAt = DateTime.UtcNow,
                TotalPrice = discountedTotal,
                RequestProducts = selectedProducts.Select(p => new RequestProduct
                {
                    ProductBrand = p.ProductBrand,
                    ProductModel = p.ProductModel,
                    ProductID = p.ProductID,
                    Quantity = p.ProductQuantity,
                    ProductRow = p.ProductRow,
                    ProductSection = p.ProductSection,
                    UnitPrice = p.ProductPrice
                }).ToList()
            };

            _context.Requests.Add(request);
            companyPost.QuarterOrderValue = prospectiveQov;
            companyPost.DiscountLevel = newLevel;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Request created with {discountPct:P1} discount (level {newLevel})!";
            return RedirectToAction("Create");
        }

    }
}
