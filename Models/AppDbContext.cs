using Microsoft.EntityFrameworkCore;
using StoreApp.Models;

namespace MyAspNetCoreApp.Web.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; } //DB'deki table ile aynı isimde olmak zorunda.
        public DbSet<User> Users { get; set; } //DB'deki table ile aynı isimde olmak zorunda.
    }
}
