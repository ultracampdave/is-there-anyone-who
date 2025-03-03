using IsThereAnyoneWho.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IsThereAnyoneWho.Data.Migrations
{
    public static class DbInitializer
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Person>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Apply migrations
                context.Database.Migrate();

                // Seed roles
                if (!await roleManager.RoleExistsAsync("Consumer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Consumer"));
                }

                if (!await roleManager.RoleExistsAsync("Provider"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Provider"));
                }

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Seed services if none exist
                if (!context.Services.Any())
                {
                    context.Services.AddRange(
                        new Service
                        {
                            Name = "House Cleaning",
                            Description = "Professional house cleaning service",
                            Category = "Home",
                            BasePrice = 75.00M,
                            CreatedDate = DateTime.Now
                        },
                        new Service
                        {
                            Name = "Lawn Mowing",
                            Description = "Lawn mowing and trimming service",
                            Category = "Garden",
                            BasePrice = 50.00M,
                            CreatedDate = DateTime.Now
                        },
                        new Service
                        {
                            Name = "Computer Repair",
                            Description = "PC and laptop repair service",
                            Category = "Technology",
                            BasePrice = 85.00M,
                            CreatedDate = DateTime.Now
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}
