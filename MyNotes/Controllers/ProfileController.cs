using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNotes.Data;
using MyNotes.Models.Entities;
using MyNotes.Models.ViewModels;

namespace MyNotes.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public ProfileController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Edit()
        {
            // Get current user logged in id
            var user = await _userManager.GetUserAsync(User);
            string userId = user!.Id;

            // Get Get current user logged in profile
            var userInformation = await _dbContext.UserInformations
                .FirstOrDefaultAsync(ui => ui.ApplicationUserId == userId);

            var viewModel = new UserInformationViewModel
            {
                Id = userInformation!.Id,
                FullName = userInformation.FullName,
                Gender = userInformation.Gender
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UserInformationViewModel model)
        {
            var userInformation = await _dbContext.UserInformations.FindAsync(model.Id);
            userInformation!.FullName = model.FullName;
            userInformation.Gender = model.Gender;
            await _dbContext.SaveChangesAsync();
            return View(new UserInformationViewModel
            {
                Id = userInformation.Id,
                FullName = userInformation.FullName,
                Gender = model.Gender
            });
        }
    }
}
