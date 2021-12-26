using BenjaminAbt.HCaptcha;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Infrastructure.Entities;
using VrRetreat.WebApp.Models;
using VrRetreat.WebApp.Models.Response;

namespace VrRetreat.WebApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<VrRetreatUser> _userManager;
    private readonly SignInManager<VrRetreatUser> _signInManager;
    private readonly IUserRepository _userRepository;

    public AccountController(UserManager<VrRetreatUser> userManager, SignInManager<VrRetreatUser> signInManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegistrationModel userModel, HCaptchaVerifyResponse hCaptcha)
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        if (!hCaptcha.Success)
        {
            ModelState.AddModelError("Hostname", "hCaptcha was not successfully solved.");
            return View(userModel);
        }

        if (!ModelState.IsValid)
        {
            return View(userModel);
        }

        var user = new VrRetreatUser
        {
            UserName = userModel.Username
        };

        var result = await _userManager.CreateAsync(user, userModel.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return View(userModel);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginModel userModel)
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            return View(userModel);
        }

        var result = await _signInManager.PasswordSignInAsync(userModel.Username, userModel.Password, userModel.RememberMe, false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError("", "Invalid UserName or Password");
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet]
    [Authorize]
    public IActionResult Settings()
    {        
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(UserSettingsModel settingsModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Settings));
        }

        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        var result = await _userManager.ChangePasswordAsync(currentUser, settingsModel.CurrentPassword, settingsModel.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return View(nameof(Settings));
        }

        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [Authorize]
    public IActionResult RelinkAccount()
    {
        return RedirectToAction(nameof(AccountLinkController.ClaimVrChatName), "AccountLink");
    }

    [Authorize]
    public async Task<IActionResult> DownloadAccount()
    {
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);

        var dataToDownload = new DownloadData
        {
            VrChatId = currentUser.VrChatId,
            VrChatName = currentUser.VrChatName,
            VrChatAvatarUrl = currentUser.VrChatAvatarUrl,
            VrChatLastLogin = currentUser.VrChatLastLogin,
            UserName = currentUser.UserName,
        };

        string jsonString = JsonSerializer.Serialize(dataToDownload, new JsonSerializerOptions { WriteIndented = true });

        return File(Encoding.UTF8.GetBytes(jsonString), "application/json;charset=UTF-8");
    }

    [Authorize]
    public async Task<IActionResult> UnlinkAccount()
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Settings));
        }

        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        currentUser.ClearVrChatLink();
        await _userRepository.UpdateUserAsync(currentUser);

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Settings));
        }

        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        var result = await _userManager.DeleteAsync(currentUser);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return View(nameof(Settings));
        }

        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

}
