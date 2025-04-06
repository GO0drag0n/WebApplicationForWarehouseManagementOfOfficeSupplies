using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
    [Authorize(Roles = "Admin,Storage Worker")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProductViewModel
            {
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryID.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ProductViewModel
                {
                    Categories = _context.Categories.Select(c => new SelectListItem
                    {
                        Value = c.CategoryID.ToString(),
                        Text = c.Name
                    }).ToList()
                };

                return View(viewModel);
            }

            var product = new Product
            {
                Brand = model.ProductBrand,
                Model = model.ProductModel,
                CategoryID = model.ProductCategoryID,
                Quantity = model.ProductQuantity,
                DeliveryPrice = model.ProductDeliveryPrice,
                Price = model.ProductPrice,
                Row = model.ProductRow,
                Section = model.ProductSection,
                UniqueNumber = Guid.NewGuid()
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index(string? searchByBrand, string? searchByModel, int? categoryId)
        {
            // Start with all products and include their category information.
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Apply the brand filter if provided.
            if (!string.IsNullOrEmpty(searchByBrand))
            {
                query = query.Where(p => EF.Functions.Like(p.Brand, $"%{searchByBrand}%"));
            }

            // Apply the model filter if provided.
            if (!string.IsNullOrEmpty(searchByModel))
            {
                query = query.Where(p => EF.Functions.Like(p.Model, $"%{searchByModel}%"));
            }

            // Apply the category filter if provided.
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            // Prepare the categories for the dropdown.
            var categories = _context.Categories
                 .Select(c => new SelectListItem
                 {
                     Value = c.CategoryID.ToString(),
                     Text = c.Name
                 }).ToList();

            // Insert the "All Categories" option at the top.
            categories.Insert(0, new SelectListItem { Value = "", Text = "All Categories" });
            ViewBag.Categories = categories;

            // Execute the query and return the result.
            var products = query.ToList();
            return View(products);
        }
    }
}
