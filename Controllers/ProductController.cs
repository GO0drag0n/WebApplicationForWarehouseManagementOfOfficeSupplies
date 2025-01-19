using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Product/Index
    public IActionResult Index(string searchByBrand, string searchByModel)
    {
        var products = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrEmpty(searchByBrand))
        {
            products = products.Where(p => p.Brand.Contains(searchByBrand));
        }

        if (!string.IsNullOrEmpty(searchByModel))
        {
            products = products.Where(p => p.Model.Contains(searchByModel));
        }

        return View(products.ToList());
    }

    // GET: Product/Create
    public IActionResult Create()
    {
        var viewModel = new ProductViewModel
        {
            Product = new Product(),
            Categories = GetCategorySelectList()
        };

        return View(viewModel);
    }

    // POST: Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Products.Add(viewModel.Product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again later.");
            }
        }

        // Repopulate categories in case of validation failure
        viewModel.Categories = GetCategorySelectList();
        return View(viewModel);
    }

    // GET: Product/Edit/{id}
    public IActionResult Edit(int id)
    {
        var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new ProductViewModel
        {
            Product = product,
            Categories = GetCategorySelectList()
        };

        return View(viewModel);
    }

    // POST: Product/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ProductViewModel viewModel)
    {
        if (id != viewModel.Product.ProductID)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(viewModel.Product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again later.");
            }
        }

        // Repopulate categories in case of validation failure
        viewModel.Categories = GetCategorySelectList();
        return View(viewModel);
    }

    // GET: Product/Delete/{id}
    public IActionResult Delete(int id)
    {
        var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Product/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete the product. Please try again later.");
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // Helper method to get category select list
    private IEnumerable<SelectListItem> GetCategorySelectList()
    {
        return _context.Categories
            .Select(c => new SelectListItem
            {
                Value = c.CategoryID.ToString(),
                Text = c.Name
            })
            .ToList();
    }
}
