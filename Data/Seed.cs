using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Data
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _Context;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IUserStore<User> _UserStore;

        public DbInitializer(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IUserStore<User> userStore)
        {
            _Context = context;
            _UserManager = userManager;
            _RoleManager = roleManager;
            _UserStore = userStore;
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

            if(!_Context.Users.Any())
            {
                User admin = new User();

                admin.LastName = "Admin";
                admin.FirstName = "Admin";
                await _UserStore.SetUserNameAsync(admin, "admin@gmail.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailAsync(admin, "admin@gmail.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailConfirmedAsync(admin, true, default);
                await _UserManager.CreateAsync(admin, "Pr0toty1pe@1");
                await _UserManager.AddToRoleAsync(admin, "admin");
            }

           
            


            await _Context.SaveChangesAsync();
        }
    }
}
