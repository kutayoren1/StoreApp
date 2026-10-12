using Microsoft.AspNetCore.Mvc;
using MyAspNetCoreApp.Web.Models;

namespace StoreApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.Where(p => p.Stock > 0).ToList();      // Get products with stock greater than 0
            return View(products);
        }
    }
}
