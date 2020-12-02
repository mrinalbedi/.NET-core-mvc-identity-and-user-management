using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MBPatients.Data;
using MBPatients.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Session;

namespace MBPatients.Controllers
{
    [Authorize(Roles ="administrators")]
    public class RoleController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public RoleController(RoleManager<IdentityRole> _roleManager, UserManager<IdentityUser> _userManager)
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
        }
        public IActionResult Index()
        {
            var roles = roleManager.Roles.OrderBy(r => r.Name);
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRole model)
        {
            try
            {
                ViewBag.RoleName = model.RoleName;
                if (string.IsNullOrEmpty(model.RoleName) || model.RoleName == " ")
                {
                    //ModelState.AddModelError("", "Role name cannot be empty or just blanks");
                    TempData["message"] = "Role name cannot be empty or just blanks";
                }
                var IsDuplicate = _context.Roles.Where(m => m.Name == model.RoleName);
                if (IsDuplicate.Any())
                {
                    // ModelState.AddModelError("", "The specified role name is already on file");
                    TempData["message"] = "The specified role name is already on file";
                }
                if (ModelState.IsValid)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = model.RoleName.Trim();
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        TempData["message"] = "Role added successfully: " + ViewBag.RoleName;
                        return RedirectToAction("Index");
                    }
                    foreach (var e in result.Errors)
                    {
                        ModelState.AddModelError("", e.Description);
                    }
                    //return RedirectToAction(nameof(Index));
                }

            }
            catch (Exception e)
            {
                TempData["message"] = e.GetBaseException().Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UsersInRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                // Error Mesage
            }
            ViewData["RoleId"] = role.Id;
            Response.Cookies.Append("RoleId", role.Id);
            HttpContext.Session.SetString("RoleId", id);

            var l = new List<UsersInRole>();
            var NotInRole = new List<UsersInRole>();
            

            foreach (var user in userManager.Users)
            {
                var model = new UsersInRole();
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Id = user.Id;
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    model.NormalizedEmail = user.NormalizedEmail;
                    l.Add(model);
                }
                else
                {
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    NotInRole.Add(model);
                    ViewBag.NotInRole = new SelectList(NotInRole.ToList().OrderBy(m=>m.UserName),"UserName","UserName");
                }
                ViewData["roleName"] = role.Name;
                
            }   
            return View(l.ToList().OrderBy(m=>m.UserName));
        }


        [HttpPost]
        public async Task<IActionResult> UsersInRole(string userName,string UserId)
        {
            try
            {
                if(Request.Cookies["RoleId"]!=null)
                    UserId = Request.Cookies["RoleId"];
                else if(HttpContext.Session.GetString("RoleId")!=null)
                {
                    UserId = HttpContext.Session.GetString("RoleId");
                }
                var role = await roleManager.FindByIdAsync(UserId);
                var users = await userManager.GetUsersInRoleAsync(role.Name);
                var m = await userManager.FindByNameAsync(userName);

                var result = await userManager.AddToRoleAsync(m, role.Name);
                if (result.Succeeded)
                {
                    TempData["message"] = m.UserName + " Successfully added to role :" + role.Name;
                    return RedirectToAction("UsersInRole");
                }
               
            }
            catch(Exception e)
            {
                TempData["message"] = e.GetBaseException().Message;
            }
            return RedirectToAction("UsersInRole");
        }

        public async Task<IActionResult> DeleteUser(string UserId, string RoleName)
        {
            try
            {
                var userName = await userManager.FindByIdAsync(UserId);
                if (RoleName == "administrators")
                {
                    TempData["message"] = "You cannot remove yourselves from the administrator role";
                    return RedirectToAction("Index");
                }
                IdentityResult result = await userManager.RemoveFromRoleAsync(userName, RoleName);
                var role = await roleManager.FindByNameAsync(RoleName);
                if (result.Succeeded)
                {
                    TempData["message"] = userName + " Removed Successfully from the corresponding role: " + RoleName;
                    return RedirectToAction("Index", "Role");
                }
            }
            catch (Exception e)
            {
                TempData["message"] = e.GetBaseException().Message;
            }
            return RedirectToAction("Index");    
            
        }
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var role = await roleManager.FindByIdAsync(id);
                ViewBag.roleId = role.Id;
                ViewData["roleName"] = role.Name;
                if (role.Name == "administrators")
                {
                    TempData["message"] = "Administrators cannot be deleted";
                    return RedirectToAction("Index");
                }
                var m = _context.UserRoles.Where(a => a.RoleId == role.Id);
                if (!m.Any())
                {
                    //var result = _context.Roles.Remove(role);
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        TempData["message"] = "Role: " + ViewBag.RoleName + " Deleted successfully as It had no users assigned ";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var e in result.Errors)
                        {
                            ModelState.AddModelError("", e.Description);
                        }
                    }
                }
                else
                {
                    var l = new List<UsersInRole>();
                    foreach (var user in userManager.Users)
                    {
                        var model = new UsersInRole();
                        if (await userManager.IsInRoleAsync(user, role.Name))
                        {
                            model.Id = user.Id;
                            model.UserName = user.UserName;
                            model.Email = user.Email;
                            model.NormalizedEmail = user.NormalizedEmail;
                            l.Add(model);
                        }

                    }
                    return View(l.ToList().OrderBy(m => m.UserName));
                }
                
            }
            catch(Exception e)
            {
                TempData["message"] = e.GetBaseException().Message;
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id, string rname)
        {
            
            try
            {
                var role = await roleManager.FindByIdAsync(id);
                rname = role.Name;
                if (await roleManager.RoleExistsAsync(rname))
                {
                    
                        IdentityRole identityRole = await roleManager.FindByIdAsync(role.Id);
                        await roleManager.DeleteAsync(identityRole);
                }
            }
            catch (Exception e)
            {
                TempData["message"] =   e.GetBaseException().Message;
            }
            return RedirectToAction("Index");
        }
        //[HttpGet]
        //public async Task<IActionResult> EditUsersInRole(string roleId)
        //{
        //    ViewBag.roleId = roleId;

        //    var role = await roleManager.FindByIdAsync(roleId);
        //    if (role == null)
        //    {
        //        // Error meesage
        //        ViewBag.ErrorMessage = $"Role with id = {roleId} cannot be found";
        //    }

        //    var model = new List<UserRole>();

        //    foreach (var user in userManager.Users)
        //    {
        //        var userrole = new UserRole
        //        {
        //            UserId = user.Id,
        //            UserName = user.UserName
        //        };

        //        if (await userManager.IsInRoleAsync(user, role.Name))
        //            userrole.IsSelected = true;
        //        else
        //            userrole.IsSelected = false;
        //        model.Add(userrole);
        //    }

        //    return View(model);
        //}


        //[HttpPost]
        //public async Task<IActionResult> EditUsersInRole(List<UserRole> model, string roleId)
        //{
        //    var role = await roleManager.FindByIdAsync(roleId);
        //    if (role == null)
        //    {
        //        // Error meesage
        //        ViewBag.ErrorMessage = $"Role with id = {roleId} cannot be found";
        //    }

        //    for (int i = 0; i < model.Count; i++)
        //    {
        //        var user = await userManager.FindByIdAsync(model[i].UserId);
        //        IdentityResult result = null;

        //        if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
        //        {
        //            result = await userManager.AddToRoleAsync(user, role.Name);
        //        }
        //        else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
        //        {
        //            result = await userManager.RemoveFromRoleAsync(user, role.Name);
        //        }
        //        else
        //            continue;

        //        if (result.Succeeded)
        //        {
        //            TempData["message"] = "User added successfully to specified role";
        //            if (i < (model.Count - 1))
        //               continue;
        //            else
        //                return RedirectToAction("Index", new { id = roleId });
        //        }
        //        else
        //            TempData["message"] = "Some Error Occurred";

        //    }

        //    return RedirectToAction("Index", new { id = roleId });
        //}

    }
}
