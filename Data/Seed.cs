using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Data
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _Context;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _Context = context;
            _UserManager = userManager;
            _RoleManager = roleManager;
        }

        public async Task Seed()
        {
            // Seed roles
            string[] roles = { "Admin", "Storage Worker", "Company Owner", "Company Worker" };

            foreach (var role in roles)
            {
                if (!await _RoleManager.RoleExistsAsync(role))
                {
                    await _RoleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed categories
            var categories = new[]
            {
                new Category { CategoryID = 1, Name = "Electronics" },
                new Category { CategoryID = 2, Name = "Furniture" },
                new Category { CategoryID = 3, Name = "Stationery" },
                new Category { CategoryID = 4, Name = "Office Supplies" }
            };

            foreach (var category in categories)
            {
                if (!_Context.Categories.Any(c => c.CategoryID == category.CategoryID))
                {
                    _Context.Categories.Add(category);
                }
            }

            await _Context.SaveChangesAsync();
        }
    }
}
