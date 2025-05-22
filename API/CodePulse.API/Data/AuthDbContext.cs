using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Data {
    public class AuthDbContext: IdentityDbContext {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { 
        }
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            const string readerRoleID = "bcb26152-1057-48d0-aef8-9b9fe3cb7903";
            const string writerRoleID = "5a705dd7-c2e4-4710-89f3-d48a069c4763";

            //create reader and writer roles
            List<IdentityRole> roles = new List<IdentityRole> {
                new IdentityRole() {
                    Id = readerRoleID,
                    ConcurrencyStamp = readerRoleID,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole() {
                    Id = writerRoleID,
                    ConcurrencyStamp = writerRoleID,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }
            };

            // Stuff the roles into the DB
            builder.Entity<IdentityRole>().HasData(roles);

            // create an admin user
            const string adminUserID = "4e3659bb-e78f-433b-847d-b76aa2ac124";
            IdentityUser admin = new IdentityUser() {
                Id = adminUserID,
                UserName = "admin@codepulse.com",
                NormalizedUserName = "admin@codepulse.com".ToUpper(),
                Email = "admin@codepulse.com",
                NormalizedEmail = "admin@codepulse.com".ToUpper(),
            };

            // give the admin the password of 'MakeItSo'
            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "MakeItSo");

            builder.Entity<IdentityUser>().HasData(admin);

            // gives roles to the admin user
            List<IdentityUserRole<string>> adminRoles = new List<IdentityUserRole<string>>() {
                new() {
                    UserId = adminUserID,
                    RoleId = readerRoleID
                },
                new() {
                    UserId = adminUserID,
                    RoleId = writerRoleID
                },
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);

        }
    }
}
