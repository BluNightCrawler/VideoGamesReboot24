using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;

namespace VideoGamesReboot24.Controllers
{
    [Authorize(Policy = "AdminRestricted")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
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
            return View(await getAllUsersWithRoles());
        }

        [HttpGet]
        [Route("Admin/EditUser/{id?}")]
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null) return View("ManageUsers", getAllUsersWithRoles());

            IdentityUser retrievedUser = await userManager.FindByIdAsync(id);

            if (retrievedUser == null) return View("ManageUsers", getAllUsersWithRoles());

            IList<string> userRoles = await userManager.GetRolesAsync(retrievedUser);

            UserWithRoles user = new UserWithRoles();
            user.Id = retrievedUser.Id;
            user.UserName = retrievedUser.UserName;
            user.Email = retrievedUser.Email;
            user.RoleNames = userRoles;

            List<IdentityRole> AvailableRoles = roleManager.Roles.ToList();
            List<(string, bool)> AvailableRolesWithUserCheck = new List<(string, bool)>();
            foreach (var role in AvailableRoles)
            {
                bool hasRole = userRoles.Contains(role.ToString());
                AvailableRolesWithUserCheck.Add((role.ToString(),hasRole));
            }
            ViewBag.AvailableRoles = AvailableRolesWithUserCheck;

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserWithRoles)
        {

        }

        //helper method
        public async Task<List<UserWithRoles>> getAllUsersWithRoles()
        {
            List<UserWithRoles> allUsers = new List<UserWithRoles>();

            foreach (IdentityUser user in userManager.Users)
            {

                UserWithRoles userModel = new UserWithRoles();
                userModel.Id = user.Id;
                userModel.UserName = user.UserName;
                userModel.Email = user.Email;
                userModel.RoleNames = await userManager.GetRolesAsync(user);

                allUsers.Add(userModel);
            }
            return allUsers;
        }
    }
}
