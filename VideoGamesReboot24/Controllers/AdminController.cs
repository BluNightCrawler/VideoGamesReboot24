using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VideoGamesReboot24.Models.ViewModels;

namespace VideoGamesReboot24.Controllers
{
    [Authorize(Policy = "AdminRestricted")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        public AdminController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        //[Route("Admin/Index")]
        [Route("Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Admin/ManageUsers")]
        public async Task<IActionResult> ManageUsers()
        {
            List<UserWithRoles> allUsers = new List<UserWithRoles>();

            foreach (var user in userManager.Users)
            {

                UserWithRoles userModel = new UserWithRoles();
                userModel.Id = user.Id;
                userModel.UserName = user.UserName;
                userModel.Email = user.Email;
                userModel.RoleNames = await userManager.GetRolesAsync(user);

                allUsers.Add(userModel);
            }
            return View(allUsers);
        }
    }
}
