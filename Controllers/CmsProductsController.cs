using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAspNetCoreApp.Web.Models;
using StoreApp.Models;

namespace StoreApp.Controllers
{
    [Authorize]
    public class CmsProductsController : Controller
    {
        private readonly AppDbContext _context;
        public CmsProductsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();      // Get all products
            return View(products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product newproduct)
        {
            // 1. Kullanıcı bir dosya seçmiş mi kontrol ediyoruz
            if (newproduct.ImageFile != null && newproduct.ImageFile.Length > 0)
            {
                // 2. Uzantı kontrolü (.jpg, .jpeg veya .png mi?)
                var extension = Path.GetExtension(newproduct.ImageFile.FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    // 3. Benzersiz bir dosya adı oluşturuyoruz (Çakışmayı önlemek için GUID ile)
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;

                    // 4. Dosyanın kaydedileceği fiziksel yolu belirliyoruz (wwwroot/ProductImages)
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");

                    // Eğer klasör yoksa otomatik oluşturuyoruz
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 5. Dosyayı sunucuya kopyalıyoruz
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await newproduct.ImageFile.CopyToAsync(fileStream);
                    }

                    // 6. Veritabanına kaydedilecek olan string yolu newproduct.ImageUrl'e atıyoruz
                    newproduct.ImageUrl = "/ProductImages/" + uniqueFileName;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Sadece .jpg, .jpeg veya .png uzantılı dosyalar yüklenebilir.");
                    return View(newproduct);
                }
            }
            // 7. Normal ürün verileriyle birlikte veritabanına kaydediyoruz
            if (ModelState.IsValid)
            {
                _context.Products.Add(newproduct);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            return View(newproduct);
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.Find(id);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Product updateProduct)
        {
            // Kullanıcı yeni görsel seçmediyse ImageFile hatasını validation'dan çıkarıyoruz
            if (updateProduct.ImageFile == null || updateProduct.ImageFile.Length == 0)
            {
                ModelState.Remove("ImageFile");
            }

            var existingProduct = await _context.Products.FindAsync(updateProduct.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            if (updateProduct.ImageFile != null && updateProduct.ImageFile.Length > 0)
            {
                var extension = Path.GetExtension(updateProduct.ImageFile.FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateProduct.ImageFile.CopyToAsync(fileStream);
                    }

                    existingProduct.ImageUrl = "/ProductImages/" + uniqueFileName;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Sadece .jpg, .jpeg veya .png uzantılı dosyalar yüklenebilir.");
                    return View(updateProduct);
                }
            }
            else
            {
                existingProduct.ImageUrl = updateProduct.ImageUrl;
            }

            existingProduct.Name = updateProduct.Name;
            existingProduct.Price = updateProduct.Price;
            existingProduct.Description = updateProduct.Description;
            existingProduct.Stock = updateProduct.Stock;

            if (ModelState.IsValid)
            {
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            return View(updateProduct);
        }
        public IActionResult Delete(int id)
        { 
            var product = _context.Products.Find(id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            TempData["Message"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }

    }
}
