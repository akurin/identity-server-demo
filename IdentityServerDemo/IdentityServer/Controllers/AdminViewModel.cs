using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Controllers
{
    public class AdminViewModel
    {
        [Required] public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}