using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;
using VideoGamesReboot24.Infrastructure;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace VideoGamesReboot24.Controllers
{
    [Authorize(Policy = "AdminRestricted")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly GameStoreDbContext gameStoreDbContext;
        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, GameStoreDbContext gameStoreDbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.gameStoreDbContext = gameStoreDbContext;
        }

        //[Route("Admin/Index")]
        [Route("Admin")]
        public IActionResult Index()
        {
            return getIndexView();
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
            if (id == null) return View("ManageUsers", await getAllUsersWithRoles());

            AppUser retrievedUser = await userManager.FindByIdAsync(id);

            if (retrievedUser == null) return View("ManageUsers", await getAllUsersWithRoles());

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
        public async Task<IActionResult> EditUser(IFormCollection formData, UserWithRoles userWRole)
        {
            AppUser user = await userManager.FindByIdAsync(userWRole.Id);

            IList<string> rolesForUser = await userManager.GetRolesAsync(user);
            
            foreach (var kvp in formData)
            {
                string keyName = kvp.Key;
                //check if keyname a role
                if (await roleManager.RoleExistsAsync(keyName))
                {
                    bool roleUpdated = kvp.Value.Contains("true");
                    //box is checked
                    if (roleUpdated == true)
                    {
                        //if role doesnt exist already add it
                        if (!rolesForUser.Contains(keyName))
                        {
                            await userManager.AddToRoleAsync(user, keyName);
                        }
                    }
                    //box is unchecked
                    else
                    {
                        //if role does exist already remove it
                        if (rolesForUser.Contains(keyName))
                        {
                            await userManager.RemoveFromRoleAsync(user, keyName);
                        }
                    }
                }
            }
            return View("ManageUsers", await getAllUsersWithRoles());
        }
        [HttpGet]
        public ActionResult UpdateAccessToken()
        {
            ApiHelper apiHelper = new ApiHelper(gameStoreDbContext, "IGDB");
            if (!apiHelper.accessTokenValid())
            {
                var jObject = JObject.Parse(System.IO.File.ReadAllText("credentials.json"));
                JObject twitchCredentials = (JObject)jObject["twitchCredentials"];
                string clientID = (string)twitchCredentials["clientID"];
                string clientSecret = (string)twitchCredentials["clientSecret"];
                apiHelper.updateAccessToken(clientID, clientSecret);
            }
            return getIndexView();
        }

        [HttpGet]
        [Route("Admin/OrderHistory")]
        public ActionResult OrderHistory()
        {
            List<OrderWithTotal> ordersWithTotals = new List<OrderWithTotal>();
            List<Order> allOrders = gameStoreDbContext.Orders.Include(o => o.Lines).ThenInclude(v => v.VideoGame).ToList();
            allOrders.ForEach(o => {
                ordersWithTotals.Add(new OrderWithTotal
                    { Order = o, Total = o.Lines.Sum(e => e.VideoGame.Price * e.Quantity) });
            });

            return View(ordersWithTotals);
        }

        //helper methods
        private async Task<List<UserWithRoles>> getAllUsersWithRoles()
        {
            List<UserWithRoles> allUsers = new List<UserWithRoles>();

            foreach (AppUser user in userManager.Users)
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

        private ViewResult getIndexView()
        {
            ApiHelper apiHelper = new ApiHelper(gameStoreDbContext, "IGDB");
            if (apiHelper.accessTokenValid())
            {
                AccessTokenExpireDate expireDate = new AccessTokenExpireDate();
                expireDate.expirationDate = apiHelper.getApiAccessToken().ExpirationTime;
                return View("Index", expireDate);
            }
            return View("Index", null);
        }
    }
}
