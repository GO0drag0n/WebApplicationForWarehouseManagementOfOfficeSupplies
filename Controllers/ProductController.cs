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

        public IActionResult Index(string? searchByBrand, string? searchByModel, int? categoryId, int page = 1)
        {
            int pageSize = 10;

            // Start with all products and include category information.
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Apply filters if provided.
            if (!string.IsNullOrEmpty(searchByBrand))
            {
                query = query.Where(p => EF.Functions.Like(p.Brand, $"%{searchByBrand}%"));
            }
            if (!string.IsNullOrEmpty(searchByModel))
            {
                query = query.Where(p => EF.Functions.Like(p.Model, $"%{searchByModel}%"));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            // Calculate pagination values.
            int totalProducts = query.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.CurrentPage = page;

            // Retrieve only the products for the current page.
            var products = query
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            // Prepare categories for filtering in the view (if needed).
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryID.ToString(),
                    Text = c.Name
                }).ToList();
            categories.Insert(0, new SelectListItem { Value = "", Text = "All Categories" });
            ViewBag.Categories = categories;

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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Map the entity to your view model.
            var viewModel = new EditProductViewModel
            {
                ProductID = product.ProductID,
                Brand = product.Brand,
                Model = product.Model,
                CategoryID = product.CategoryID,
                Quantity = product.Quantity,
                DeliveryPrice = product.DeliveryPrice,
                Price = product.Price,
                Row = product.Row,
                Section = product.Section
            };

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "Name", product.CategoryID);
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "Name", editModel.CategoryID);
                return View(editModel);
            }

            // Retrieve the existing product using ProductID from the model.
            var productToUpdate = await _context.Products.FindAsync(editModel.ProductID);
            if (productToUpdate == null)
            {
                return NotFound();
            }

            // Map the fields from the view model.
            productToUpdate.Brand = editModel.Brand;
            productToUpdate.Model = editModel.Model;
            productToUpdate.CategoryID = editModel.CategoryID;
            productToUpdate.Quantity = editModel.Quantity;
            productToUpdate.DeliveryPrice = editModel.DeliveryPrice;
            productToUpdate.Price = editModel.Price;
            productToUpdate.Row = editModel.Row;
            productToUpdate.Section = editModel.Section;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.ProductID == editModel.ProductID))
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}

