using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Controllers
{
    public class CreateViewModel
    {
        [Required]
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        [Required]
        public string Password { get; set; }
    }
}