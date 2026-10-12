using StoreApp.Models;

namespace StoreApp.Services
{
    public interface IEmailService
    {
        Task SendContactEmailAsync(ContactViewModel model);
    }
}
