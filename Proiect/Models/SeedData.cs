using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using System.Collections.Generic;

namespace Proiect.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider
        serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService
            <DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Roles.Any())
                {
                    return; 
                }
                context.Roles.AddRange(
                new IdentityRole
                {
                    Id = "24eb7ea3-4205-4eb9-afc8-d17a00bb55e0", Name = "Admin", NormalizedName = "Admin".ToUpper() },
             
                new IdentityRole
                {
                    Id = "24eb7ea3-4205-4eb9-afc8-d17a00bb55e1", Name = "User", NormalizedName = "User".ToUpper() }
                );
                var hasher = new PasswordHasher<ApplicationUser>();
                context.Users.AddRange(
                new ApplicationUser

                {
                    Id = "06559aa8-74f2-4bbb-9ff1-ddace3fde780",
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },

                new ApplicationUser
                {

                    Id = "06559aa8-74f2-4bbb-9ff1-ddace3fde781",
                    UserName = "user@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@TEST.COM",
                    Email = "user@test.com",
                    NormalizedUserName = "USER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,"User1!")
                }
                );
                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    //admin
                    RoleId = "24eb7ea3-4205-4eb9-afc8-d17a00bb55e0",
                    UserId = "06559aa8-74f2-4bbb-9ff1-ddace3fde780"
                },

                new IdentityUserRole<string>
                {
                    //user
                    RoleId = "24eb7ea3-4205-4eb9-afc8-d17a00bb55e1",
                    UserId = "06559aa8-74f2-4bbb-9ff1-ddace3fde781"
                }
                );
                context.SaveChanges();
            }
        }
    }
}
