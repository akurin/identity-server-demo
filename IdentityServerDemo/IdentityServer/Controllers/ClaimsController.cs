using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [Route("claims")]
    [Authorize(Roles = "Admin")]
    public class ClaimsController : ControllerBase
    {
        public IActionResult Get()
        {
            if (User.IsInRole("Admin"))
            {
                Console.WriteLine("Admin");
            }
            
            return new JsonResult(from c in User.Claims select new {c.Type, c.Value});
        }
    }
}