using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Models;
using System.Security.Claims;
namespace StoreApp.Controllers
{
    public class AuthController : Controller
    {
        // GET: Kullanıcı /cms/login adresine gittiğinde çalışır, boş formu gösterir
        [HttpGet("cms/login")]
        public IActionResult Login()
        {
            return View();
        }
        // POST: Kullanıcı form doldurup "Giriş Yap" dediğinde çalışır
        [HttpPost("cms/login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // ÖRNEK KONTROL: Burada veritabanındaki (DbContext) kullanıcı tablon ile 
            // model.Username ve model.Password verilerini karşılaştırmalısın.
            if (model.username == "admin" && model.password == "123456")
            {
                // 1. Kullanıcının kimlik rozetini (Claim) hazırlıyoruz
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.username)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 2. Çerezi tarayıcıya yazdırıp oturumu başlatıyoruz (SignInAsync)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // 3. Giriş başarılı olunca yönetim paneline yönlendiriyoruz
                return RedirectToAction("Index", "CmsProducts"); // Kendi yönetim paneli controller adın neyse
            }

            // Bilgiler yanlışsa hata mesajı verip formu tekrar gösteriyoruz
            ModelState.AddModelError("", "Wrong username or password!");
            return View(model);
        }
        [HttpGet("cms/logout")]
        public async Task<IActionResult> Logout()
        {
            // Oturum çerezini siliyoruz
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Anasayfaya yönlendiriyoruz
        }
    }
}
