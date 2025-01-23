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
            if (!_Context.Categories.Any())
            {
                var categories = new[]
                {
            new Category { Name = "Electronics" },
            new Category { Name = "Furniture" },
            new Category { Name = "Stationery" },
            new Category { Name = "Office Supplies" }
        };

                _Context.Categories.AddRange(categories);
            }

            // Seed users
            if (!_Context.Users.Any())
            {
                User admin = new User
                {
                    LastName = "Admin",
                    FirstName = "Admin"
                };

                await _UserStore.SetUserNameAsync(admin, "admin@gmail.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailAsync(admin, "admin@gmail.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailConfirmedAsync(admin, true, default);
                await _UserManager.CreateAsync(admin, "Pr0toty1pe@1");
                await _UserManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed products
            // Seed products
            if (!_Context.Products.Any())
            {
                var electronicsCategory = _Context.Categories.FirstOrDefault(c => c.Name == "Electronics");
                var furnitureCategory = _Context.Categories.FirstOrDefault(c => c.Name == "Furniture");
                var stationeryCategory = _Context.Categories.FirstOrDefault(c => c.Name == "Stationery");
                var officeSuppliesCategory = _Context.Categories.FirstOrDefault(c => c.Name == "Office Supplies");

                if (electronicsCategory != null && furnitureCategory != null && stationeryCategory != null && officeSuppliesCategory != null)
                {
                    var products = new[]
                    {
                        // Electronics
                        new Product { Brand = "Apple", Model = "iPhone 14", CategoryID = electronicsCategory.CategoryID, Quantity = 50, DeliveryPrice = 700.00m, Price = 999.99m, Row = 1, Section = 1 },
                        new Product { Brand = "Sony", Model = "PlayStation 5", CategoryID = electronicsCategory.CategoryID, Quantity = 25, DeliveryPrice = 400.00m, Price = 499.99m, Row = 2, Section = 2 },
                        new Product { Brand = "Samsung", Model = "Galaxy Tab S8", CategoryID = electronicsCategory.CategoryID, Quantity = 30, DeliveryPrice = 500.00m, Price = 649.99m, Row = 3, Section = 3 },
                        new Product { Brand = "LG", Model = "OLED TV C1", CategoryID = electronicsCategory.CategoryID, Quantity = 10, DeliveryPrice = 1200.00m, Price = 1499.99m, Row = 4, Section = 4 },
                        new Product { Brand = "Bose", Model = "QuietComfort 45", CategoryID = electronicsCategory.CategoryID, Quantity = 35, DeliveryPrice = 200.00m, Price = 299.99m, Row = 5, Section = 1 },

                        // Furniture
                        new Product { Brand = "Ikea", Model = "Billy Bookcase", CategoryID = furnitureCategory.CategoryID, Quantity = 80, DeliveryPrice = 30.00m, Price = 59.99m, Row = 6, Section = 2 },
                        new Product { Brand = "Ikea", Model = "Malm Bed Frame", CategoryID = furnitureCategory.CategoryID, Quantity = 40, DeliveryPrice = 200.00m, Price = 249.99m, Row = 7, Section = 3 },
                        new Product { Brand = "Herman Miller", Model = "Aeron Chair", CategoryID = furnitureCategory.CategoryID, Quantity = 15, DeliveryPrice = 800.00m, Price = 1199.99m, Row = 8, Section = 4 },
                        new Product { Brand = "Wayfair", Model = "Dresser", CategoryID = furnitureCategory.CategoryID, Quantity = 20, DeliveryPrice = 150.00m, Price = 199.99m, Row = 9, Section = 1 },
                        new Product { Brand = "West Elm", Model = "Mid-Century Table", CategoryID = furnitureCategory.CategoryID, Quantity = 10, DeliveryPrice = 300.00m, Price = 399.99m, Row = 10, Section = 2 },

                        // Stationery
                        new Product { Brand = "Pilot", Model = "G2 Pen", CategoryID = stationeryCategory.CategoryID, Quantity = 200, DeliveryPrice = 0.50m, Price = 1.99m, Row = 11, Section = 3 },
                        new Product { Brand = "Moleskine", Model = "Classic Notebook", CategoryID = stationeryCategory.CategoryID, Quantity = 150, DeliveryPrice = 8.00m, Price = 12.99m, Row = 12, Section = 4 },
                        new Product { Brand = "3M", Model = "Post-It Notes", CategoryID = stationeryCategory.CategoryID, Quantity = 300, DeliveryPrice = 5.00m, Price = 7.99m, Row = 13, Section = 1 },
                        new Product { Brand = "Staples", Model = "Stapler", CategoryID = stationeryCategory.CategoryID, Quantity = 100, DeliveryPrice = 10.00m, Price = 14.99m, Row = 14, Section = 2 },
                        new Product { Brand = "Faber-Castell", Model = "Colored Pencils", CategoryID = stationeryCategory.CategoryID, Quantity = 50, DeliveryPrice = 15.00m, Price = 19.99m, Row = 15, Section = 3 }
                    };

                    _Context.Products.AddRange(products);
                }
            }

            await _Context.SaveChangesAsync();


            await _Context.SaveChangesAsync();
        }

    }
}
