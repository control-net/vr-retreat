using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.StartChallenge;
using VrRetreat.Infrastructure.Entities;
using VrRetreat.WebApp.Models;
using VrRetreat.WebApp.Presenters;

namespace VrRetreat.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly SignInManager<VrRetreatUser> _signInManager;
    private readonly UserManager<VrRetreatUser> _userManager;
    private readonly IUserRepository _userRepository;

    private readonly IStartChallengeUseCase _startChallengeUseCase;
    private readonly StartChallengePresenter _startChallegePresenter;

    public HomeController(SignInManager<VrRetreatUser> signInManager, UserManager<VrRetreatUser> userManager, IUserRepository userRepository, IStartChallengeUseCase startChallengeUseCase, StartChallengePresenter startChallengePresenter)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userRepository = userRepository;

        _startChallengeUseCase = startChallengeUseCase;
        _startChallegePresenter = startChallengePresenter;
    }

    public async Task<IActionResult> Index()
    {
        if (!_signInManager.IsSignedIn(User))
            return View();

        if (await _userRepository.HasLinkedAccountByUsername(User.Identity?.Name ?? string.Empty))
        {
            var user = await _userManager.GetUserAsync(User);
            // NOTE(Peter): After Milestone 1.1 this should point to a real Dashboard instead
            return View("StartChallenge");
        }

        return View("UnlinkedAccount");
    }

    [Authorize]
    public async Task<IActionResult> InitChallenge()
    {
        _startChallegePresenter.ModelState = ModelState;

        if (!ModelState.IsValid)
        {
            return View(nameof(Index));
        }

        await _startChallengeUseCase.ExecuteAsync(new(User.Identity?.Name!));

        if (_startChallegePresenter.Result is not null)
            return _startChallegePresenter.Result;

        if(_startChallegePresenter.Success)
            return RedirectToAction("Index", "Home");

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Dashboard()
    {
        var model = new DashboardViewModel
        {
            CurrentUser = new()
            {
                Username = "voxie",
                AvatarUrl = "https://api.vrchat.cloud/api/1/image/file_bcad5b12-95e8-4d3b-9393-4941dd9c9567/1/1280",
                LastVrChatLogin = DateTime.Now.AddDays(-2).AddHours(-10),
                Failed = false
            },
            FollowedPeople = new[]
            {
                    new UserDashboardModel()
                    {
                        Username = "CallMeSalad",
                        AvatarUrl = "https://api.vrchat.cloud/api/1/image/file_6eec5ee9-8096-4b8f-9410-54045c0b8221/2/256",
                        LastVrChatLogin = DateTime.Now.AddDays(-4),
                        Failed = false
                    },
                    new UserDashboardModel()
                    {
                        Username = "Pusheenmon",
                        AvatarUrl = "https://api.vrchat.cloud/api/1/image/file_ac8dba90-5c83-49e1-b84c-bfd5aeb4bd99/1/256",
                        LastVrChatLogin = DateTime.Now.AddDays(-1).AddHours(-3),
                        Failed = false
                    },
                    new UserDashboardModel()
                    {
                        Username = "273BeLow",
                        AvatarUrl = "https://api.vrchat.cloud/api/1/image/file_351dcee8-0aec-40ae-a255-9faf16dfdda4/1/256",
                        LastVrChatLogin = DateTime.Now.AddHours(-10),
                        Failed = true
                    }
                }
        };

        return View("Dashboard", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
