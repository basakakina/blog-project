using ApplicationCore.Entities.Concrete;
using ApplicationCore.UserEntites.Concrete;
using Microsoft.AspNetCore.Identity;

namespace WEB.ServiceExtensions
{
    public static class SeedExtensions
    {
        public static async Task SeedIdentityAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

           
            var adminRole = config["Admin:Role"] ?? "Admin";
            string[] roles = new[] { adminRole, "Author", "Reader" };
            foreach (var r in roles)
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new AppRole { Name = r });

            
            var email = config["Admin:Email"] ?? "admin@metu.edu.tr";
            var userName = config["Admin:UserName"] ?? "admin";
            var password = config["Admin:Password"] ?? "Admin123";

            var admin = await userManager.FindByNameAsync(userName)
                        ?? await userManager.FindByEmailAsync(email);

            if (admin is null)
            {
                admin = new AppUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true
                };
                var create = await userManager.CreateAsync(admin, password);
                if (!create.Succeeded)
                    throw new Exception("Admin user create failed: " +
                        string.Join(", ", create.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, adminRole))
                await userManager.AddToRoleAsync(admin, adminRole);
        }

       
        public static async Task SeedDemoDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<DataAccess.DbContext.BlogDbContext>();

            if (!ctx.Categories.Any())
            {
                var cat1 = new Category { Name = "News", Slug = "news" };
                var cat2 = new Category { Name = "Guides", Slug = "guides" };
                ctx.Categories.AddRange(cat1, cat2);

                var tag1 = new Tag { Name = "EFCore", Slug = "efcore" };
                var tag2 = new Tag { Name = "Identity", Slug = "identity" };
                ctx.Tags.AddRange(tag1, tag2);

                var admin = ctx.Users.FirstOrDefault(u => u.UserName == "admin");
                var post = new Post
                {
                    Title = "Deneme Postu",
                    Slug = "deneme-postu",
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    AuthorId = admin!.Id,
                    Category = cat1,
                    IsPublished = true,
                    CreatedDate = DateTime.UtcNow
                };
                post.PostTags = new List<PostTag> { new PostTag { Tag = tag1 } };

                ctx.Posts.Add(post);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
