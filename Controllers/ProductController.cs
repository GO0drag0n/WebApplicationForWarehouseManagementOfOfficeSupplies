using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Controllers
{
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
                ProductBrand = "chep",
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
        public IActionResult Index(string? searchByBrand, string? searchByModel)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchByBrand))
            {
                query = query.Where(p => EF.Functions.Like(p.Brand, $"%{searchByBrand}%"));
            }

            if (!string.IsNullOrEmpty(searchByModel))
            {
                query = query.Where(p => EF.Functions.Like(p.Model, $"%{searchByModel}%"));
            }

            var products = query.ToList();
            return View(products);
        }
    }
}
