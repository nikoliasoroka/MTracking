using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MTracking.Core.Entities;

namespace MTracking.DAL.Seeder
{
    public static class SeedExtension
    {
        public static void Seed(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            scope.ServiceProvider.GetRequiredService<MTrackingDbContext>().Database.Migrate();

            var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

            //if (env.IsDevelopment()) 
            //    app.SeedDataAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task SeedDataAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MTrackingDbContext>();

            if (context.Users.Any())
                return;

            CreatePasswordHash("--------", out var passwordHash, out var passwordSalt);
            
            var user = new User
            {
                CommitId = 305998,
                EnglishName = "Ben Levkowitz",
                UserName = "Ben Levkowitz",
                HebrewName = "בן לבקוביץ עו^^ד",
                EmployeeRunCommit = true,
                EmployeeSoftwareInvention = true,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsPasswordChanged = false,
                CreatedOn = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
