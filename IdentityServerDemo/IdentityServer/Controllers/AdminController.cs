using System.Linq;
using System.Threading.Tasks;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var adminRole = await _context.Roles.SingleAsync(r => r.Name == "Admin");

            var queryable = _context.Users.Select(user => new AdminViewModel
            {
                Email = user.UserName,
                IsAdmin = _context.UserRoles.Any(userRole =>
                    userRole.RoleId == adminRole.Id && userRole.UserId == user.Id)
            });

            var adminViewModels = await queryable.ToListAsync();
            return View(adminViewModels);
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserName == id);

            if (user == null)
            {
                return NotFound();
            }

            var adminRole = await _context.Roles.SingleAsync(r => r.Name == "Admin");
            var isAdmin = _context.UserRoles.Any(userRole =>
                userRole.RoleId == adminRole.Id && userRole.UserId == user.Id);

            return View(new AdminViewModel
            {
                Email = user.Email,
                IsAdmin = isAdmin,
            });
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,IsAdmin,Password")] CreateViewModel createViewModel)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    Email = createViewModel.Email,
                    UserName = createViewModel.Email
                };

                await _userManager.CreateAsync(applicationUser, createViewModel.Password);

                if (createViewModel.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(applicationUser, "Admin");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(createViewModel);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var adminRole = await _context.Roles.SingleAsync(r => r.Name == "Admin");
            var isAdmin = _context.UserRoles.Any(userRole =>
                userRole.RoleId == adminRole.Id && userRole.UserId == user.Id);

            return View(new AdminViewModel
            {
                Email = user.Email,
                IsAdmin = isAdmin
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email,IsAdmin")] AdminViewModel adminViewModel)
        {
            if (id != adminViewModel.Email)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(id);
                await _userManager.UpdateAsync(user);

                if (adminViewModel.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(adminViewModel);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}