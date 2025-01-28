using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    [Authorize(Roles = "Company Owner, Company Worker")]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Request/Create
        public IActionResult Create(string searchTerm = "", int? categoryId = null)
        {
            // Initialize the queryable object
            var query = _context.Products.AsQueryable();

            // Apply filtering for search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Include(x => x.RequestProducts).Include(x => x.Category).Where(p => p.Brand.Contains(searchTerm) || p.Model.Contains(searchTerm));
            }

            // Apply filtering for category ID
            if (categoryId.HasValue)
            {
                query = query.Include(x => x.RequestProducts).Include(x => x.Category).Where(p => p.CategoryID == categoryId.Value);
            }

            // Execute the filtered query
            var filteredProducts = query.Include(x => x.RequestProducts).Include(x => x.Category).ToList();

            // Pass the products and categories to the view model
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
                    ProductQuantity = p.Quantity // Default quantity
                }).ToList(),
                Categories = categories
            };

            return View(viewModel);
        }

        // POST: Request/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reinitialize non-editable fields for redisplay
                foreach (var product in model.RequestProducts)
                {
                    var dbProduct = _context.Products.Include(x => x.RequestProducts).FirstOrDefault(p => p.ProductID == product.ProductID);
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

            // Filter selected products based on quantity > 0
            var selectedProducts = model.RequestProducts
                .Where(p => p.ProductQuantity > 0)
                .Select(p => new RequestProduct
                {
                    ProductBrand = p.ProductBrand,
                    ProductModel = p.ProductModel,
                    ProductID = p.ProductID,
                    Quantity = p.ProductQuantity
                })
                .ToList();

            if (!selectedProducts.Any())
            {
                // Add error if no products are selected
                ModelState.AddModelError(string.Empty, "You must select at least one product with a quantity greater than 0.");
                return View(model);
            }

            // Populate RequestProductBrand and RequestProductModel from the first selected product
            var firstSelectedProduct = selectedProducts.FirstOrDefault();
            if (firstSelectedProduct != null)
            {
                model.RequestProductBrand = firstSelectedProduct.ProductBrand;
                model.RequestProductModel = firstSelectedProduct.ProductModel;
            }

            // Get the logged-in user's ID
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "User is not logged in.");
                return View(model);
            }

            // Fetch the company for the logged-in user
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.OwnerId == userId);
            if (company == null)
            {
                ModelState.AddModelError(string.Empty, "No associated company found for the logged-in user.");
                return View(model);
            }

            // Create a new request
            var request = new Request
            {
                UserID = userId,
                Status = model.RequestStatus,
                Brand = model.RequestProductBrand,
                Model = model.RequestProductModel,
                CompanyId = company.CompanyId, // Assign the valid CompanyId
                RequestProducts = selectedProducts
            };

            // Add and save the request to the database
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            // Provide feedback and redirect
            TempData["SuccessMessage"] = "Request created successfully!";
            return View(model);
        }






    }
}