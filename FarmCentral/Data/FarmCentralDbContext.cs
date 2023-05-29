using FarmCentral.Models.ModelsDB;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FarmCentral.Data
{
    public class FarmCentralDbContext : DbContext
    {
        public FarmCentralDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Farmer> Farmers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        //Hash for employees and farmers (check location)
        public static string Hash(string value)
        {
            //Hash value obtained from
            //https://stackoverflow.com/questions/16999361/obtain-sha-256-string-of-a-string/17001289#17001289
            //User answered:
            //https://stackoverflow.com/users/14608904/samuel-johnson
            //Accessed 25 May 2023
            using var hash = System.Security.Cryptography.SHA256.Create();
            var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(byteArray);
        }
    }
}
