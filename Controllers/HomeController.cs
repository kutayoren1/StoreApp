using Microsoft.AspNetCore.Mvc;
using MyAspNetCoreApp.Web.Models;
using StoreApp.Models;
using StoreApp.Services;
using System.Diagnostics;

namespace StoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(IEmailService emailService, ILogger<HomeController> logger, AppDbContext context)
        {
            _emailService = emailService;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Language Integrated Query (LINQ) to get the latest 5 featured products
            var featuredProducts = _context.Products
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToList();
            return View(featuredProducts);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _emailService.SendContactEmailAsync(model);
                TempData["SuccessMessage"] = "Your message has been sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the contact message.");
                TempData["ErrorMessage"] = "An error occurred while sending your message. Please try again later.";
            }

            return RedirectToAction("Contact");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
