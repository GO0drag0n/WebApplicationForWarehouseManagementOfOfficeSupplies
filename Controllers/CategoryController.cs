using Microsoft.AspNetCore.Mvc;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }



    // POST: Category/Create
    [HttpPost]
    public IActionResult Create([FromBody] Category category)
    {
        if (category == null || string.IsNullOrWhiteSpace(category.Name))
        {
            return BadRequest("Category name cannot be empty.");
        }

        try
        {
            // Save the new category
            _context.Categories.Add(category);
            _context.SaveChanges();

            // Return success response
            return Ok(new
            {
                message = "Category created successfully.",
                categoryId = category.CategoryID,
                uniqueNumber = category.UniqueNumber
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
