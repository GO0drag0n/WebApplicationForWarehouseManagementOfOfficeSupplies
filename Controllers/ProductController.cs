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

        public IActionResult Create()
        {
            var model = new ProductViewModel
            {
                Product = new Product(), // Initialize an empty Product object
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
        public IActionResult Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload categories if validation fails
                model.Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryID.ToString(),
                    Text = c.Name
                }).ToList();
            }
                var product = new Product
                {
                    Brand = model.Product.Brand,
                    Model = model.Product.Model,
                    CategoryID = model.Product.CategoryID,
                    Quantity = model.Product.Quantity,
                    DeliveryPrice = model.Product.DeliveryPrice,
                    Price = model.Product.Price,
                    Row = model.Product.Row,
                    Section = model.Product.Section,
                    UniqueNumber = Guid.NewGuid()
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
        

    }
}
