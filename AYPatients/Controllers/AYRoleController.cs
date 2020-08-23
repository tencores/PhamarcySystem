using AYPatients.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AYPatients.Controllers
{
    [Authorize(Roles = "administrators")]
    public class AYRoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AYRoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        public IActionResult Index()
        {
            var model = new IndexAndCreateRole();
            model.RolesList = roleManager.Roles.OrderBy(a => a.Name).ToList();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(IndexAndCreateRole model)
        {
            model.RoleName = model.RoleName.Trim();
            if (string.IsNullOrEmpty(model.RoleName))
            {
                TempData["message"] = "Role Name cannot be empty";
                return RedirectToAction("Index");
            }

            var role = await roleManager.FindByIdAsync(model.RoleName);
            if (role != null)
            {
                TempData["message"] = "This role is existing already";
            }
            else
            {
                try
                {
                    IdentityRole newRole = new IdentityRole { Name = model.RoleName };
                    var result = await roleManager.CreateAsync(newRole);
                    if (result.Succeeded)
                    {
                        TempData["message"] = $"Role added: {model.RoleName} ";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {

                    TempData["message"] = ex.GetBaseException().Message;
                }
            }

            return RedirectToAction("Index");
        }

        //**********************************************************************//
        public async Task<IActionResult> UsersInRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            var model = new RoleAndUsers(); //include users in this role

            if (role == null)
            {
                ModelState.AddModelError("", $"Role does not exist");
            }
            else
            {
                ViewBag.roleName = role.Name;
                ViewBag.roleId = role.Id;

                var usersNotInRole = new List<IdentityUser>();

                foreach (var user in userManager.Users)
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        model.UsersInThisRole.Add(new IdentityUser { UserName = user.UserName, Email = user.Email });
                    }
                    else
                    {
                        usersNotInRole.Add(new IdentityUser { UserName = user.UserName, Email = user.Email });
                    }
                    ViewBag.UsersNotInRole = new SelectList(usersNotInRole.OrderBy(a => a.UserName).ToList());
                }

                model.UsersInThisRole = model.UsersInThisRole.OrderBy(a => a.UserName).ToList();
            }

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> AddUser(RoleAndUsers model)
        {

            var role = await roleManager.FindByIdAsync(model.RoleId);
            var user = await userManager.FindByNameAsync(model.UserName);


            IdentityResult result = await userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                TempData["message"] = $"User: {user.UserName} added to Role: '{role.Name}' ";
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("UsersInRole", new { id = model.RoleId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(RoleAndUsers model)
        {

            var role = await roleManager.FindByIdAsync(model.RoleId);
            var user = await userManager.FindByNameAsync(model.UserName);

            IdentityResult result = await userManager.RemoveFromRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                TempData["message"] = $"User: {user.UserName} removed from: '{role.Name}' ";
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("UsersInRole", new { id = model.RoleId });
        }


        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            ViewBag.roleName = role.Name;
            ViewBag.roleID = role.Id;

            var model = new List<IdentityUser>(); //users in the list

            if (role.Name == "administrator")
            {
                return RedirectToAction("index");
            }

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Add(new IdentityUser { UserName = user.UserName });
                }
            }
            if (model.Any())
            {
                model = model.OrderBy(u => u.UserName).ToList();
                return View(model);
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["message"] = $"{role.Name} was successfully deleted";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        TempData["message"] = TempData["message"] + error.Description + "\n";
                    }
                }
            }

            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelte(List<IdentityUser> model, string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            ViewBag.roleName = role.Name;
            ViewBag.roleID = role.Id;

            try
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["message"] = $"{role.Name} was successfully deleted";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        TempData["message"] = TempData["message"] + error.Description + "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var user in userManager.Users)
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        model.Add(new IdentityUser { UserName = user.UserName });
                    }
                }
                TempData["message"] = $"Exception occured: {ex.GetBaseException().Message}";
            }
            return RedirectToAction("Index");
        }




    }
}
