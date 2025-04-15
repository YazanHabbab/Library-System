using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using business_logic.DTOs;
using business_logic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        // For admin only
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> AllUsers(string searchTerm, bool activeOnly)
        {
            var users = await _accountService.SearchUsers(searchTerm, activeOnly);
            return View(users);
        }

        public IActionResult Register()
        {
            UserDto userRegisterModel = new();
            return View(userRegisterModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserDto userRegisterModel)
        {
            if (ModelState.IsValid)
            {
                var createResult = await _accountService.CreateNewAccount(userRegisterModel);
                if (createResult.Result)
                {
                    //if the creation of the account succeded then login directly
                    await Login(new UserLoginDto
                    {
                        Name = userRegisterModel.Name,
                        Password = userRegisterModel.Password
                    });
                    return RedirectToAction("AllBooks", "Library");
                }
                else
                {
                    // Return the error message if the creation failed
                    ModelState.AddModelError("", createResult.Message!);
                    return View(userRegisterModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please check the name and password fields!");
                return View(userRegisterModel);
            }
        }

        public IActionResult Login()
        {
            UserLoginDto userLoginModel = new();
            return View(userLoginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userLoginModel)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _accountService.LoginAccount(userLoginModel);
                if (loginResult.Result)
                {
                    // Logging in the user and saving his credentials
                    var claims = new List<Claim>
                    {
                        new Claim("UserId", loginResult.UserId.ToString()!),
                        new Claim(ClaimTypes.Name, loginResult.Name!)
                    };

                    // Signning in the user
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = false });

                    TempData["Notification"] = "Logged in successfully!";
                    TempData["NotificationType"] = "success";

                    return RedirectToAction("AllBooks", "Library");
                }
                else
                {
                    ModelState.AddModelError("", loginResult.Message!);
                    return View(userLoginModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please check your credentials!");
                return View(userLoginModel);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            TempData["Notification"] = "Logged out successfully!";
            TempData["NotificationType"] = "success";
            
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Update()
        {
            UpdatedUserDto updatedUser = new();
            return View(updatedUser);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(UpdatedUserDto updatedUser)
        {
            if (ModelState.IsValid)
            {
                var updateResult = await _accountService.UpdateUserInfo(updatedUser, int.Parse(User.FindFirst("UserId")!.Value));
                if (updateResult.Result)
                {
                    //For loginng in again after update
                    await Login(new UserLoginDto
                    {
                        Name = updatedUser.NewName!,
                        Password = updatedUser.NewPassword!
                    });

                    TempData["Notification"] = "Updated info successfully!";
                    TempData["NotificationType"] = "success";
                    return RedirectToAction("AllBooks", "Library");
                }
                else
                {
                    ModelState.AddModelError("", updateResult.Message!);
                    return View(updatedUser);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please check the name, password and confirm password fields!");
                return View(updatedUser);
            }
        }

        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            // Get the logged user Id
            var UserId = int.Parse(User.FindFirst("UserId")!.Value);

            var deleteResult = await _accountService.DeleteAccount(UserId);
            if (!deleteResult.Result)
            {
                return Content($"Could not delete the account because: {deleteResult.Message}");
            }
            else
            {
                TempData["Notification"] = "Your account is deleted!";
                TempData["NotificationType"] = "error";
            }

            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // For admin only
        // For activating a specific user account
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ActivateAccount(int UserId)
        {
            var deleteResult = await _accountService.ActivateAccount(UserId);
            if (!deleteResult.Result)
            {
                return Content($"Could not delete the account because: {deleteResult.Message}");
            }
            else
            {
                TempData["Notification"] = "Account is activated!";
                TempData["NotificationType"] = "success";
            }

            return RedirectToAction("AllUsers", "Account");
        }

        // For admin only
        // For deactivating a specific user account
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeactivateAccount(int UserId)
        {
            var deleteResult = await _accountService.DeleteAccount(UserId);
            if (!deleteResult.Result)
            {
                return Content($"Could not delete the account because: {deleteResult.Message}");
            }
            else
            {
                TempData["Notification"] = "Account is deactivated!";
                TempData["NotificationType"] = "error";
            }

            return RedirectToAction("AllUsers", "Account");
        }
    }
}