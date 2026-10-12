using System.ComponentModel.DataAnnotations;

namespace StoreApp.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Lütfen kullanıcı adınızı / isminizi girin.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen e-posta adresinizi girin.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen mesajınızı girin.")]
        public string Message { get; set; } = string.Empty;
    }
}
