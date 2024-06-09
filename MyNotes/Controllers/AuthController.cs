using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyNotes.Data;
using MyNotes.Models.Entities;
using MyNotes.Models.ViewModels;

namespace MyNotes.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if(User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(nameof(NoteController.Index), "Note");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(nameof(NoteController.Index), "Note");
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.Username.Trim(),
                    Email = model.Email.Trim(),
                };

                var result = await _userManager.CreateAsync(user, model.Password.Trim());
                if(result.Succeeded)
                {
                    UserInformation userInformation = new UserInformation()
                    {
                        ApplicationUserId = user.Id
                    };
                    await _context.UserInformations.AddAsync(userInformation);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(NoteController.Index), "Note");
                }

                foreach (var error in result.Errors)
                {
                ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(nameof(NoteController.Index), "Note");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(nameof(NoteController.Index), "Note");
            }
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username.Trim(), model.Password.Trim(), model.RememberMe, lockoutOnFailure: false);
                if(result.Succeeded)
                {
                    return RedirectToAction(nameof(NoteController.Index), "Note");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditAccount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var editAccountViewModel = new EditAccountViewModel
            {
                Id = currentUser!.Id,
                Username = currentUser.UserName!.Trim(),
                Email = currentUser.Email!.Trim()
            };
            return View(editAccountViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount([FromForm] EditAccountViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (!await _userManager.CheckPasswordAsync(user!, model.Password.Trim()))
                {
                    ModelState.AddModelError(string.Empty, "Current password is incorrect");
                    return View(model);
                }
                if (!model.NewPassword.IsNullOrEmpty())
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user!, model.Password.Trim() , model.NewPassword!.Trim());
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
                user!.Email = model.Email.Trim();
                var updateResult = await _userManager.UpdateAsync(user);
                if(updateResult.Succeeded)
                {
                    return View(model);
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
