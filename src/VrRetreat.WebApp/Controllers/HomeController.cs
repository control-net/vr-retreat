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

            if(!user.IsParticipating)
                return View("StartChallenge");

            return View("Dashboard", GetDashboardModelFor(user));
        }

        return View("UnlinkedAccount");
    }

    private DashboardViewModel GetDashboardModelFor(VrRetreatUser user)
    {
        var participants = _userManager.Users.Where(u => u.IsParticipating && u.Id != user.Id && u.VrChatLastLogin != null).ToList();

        return new()
        {
            CalendarWeeks = GetCalendarFor(user),
            CurrentUser = UserToDashboardModel(user),
            FollowedPeople = participants.Select(UserToDashboardModel),
            RemainingChallengeTime = (DateTime.Now - new DateTime(2022, 2, 1)).Duration()
        };
    }

    private UserDashboardModel UserToDashboardModel(VrRetreatUser user)
    {
        return new()
        {
            AvatarUrl = user.VrChatAvatarUrl,
            LastVrChatLogin = user.VrChatLastLogin ?? DateTime.MinValue,
            Username = user.VrChatName,
            Failed = user.FailedChallenge
        };
    }

    [Authorize]
    public async Task<IActionResult> InitChallenge()
    {
        _startChallegePresenter.ModelState = ModelState;

        if (!ModelState.IsValid)
        {
            return View(nameof(Index));
        }

        var currentUser = await _userManager.GetUserAsync(HttpContext.User);

        await _startChallengeUseCase.ExecuteAsync(new(User.Identity?.Name!));

        if (_startChallegePresenter.Result is not null)
        {
            return _startChallegePresenter.Result;
        }

        if (_startChallegePresenter.Success)
        {
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Dashboard()
    {
        var model = new DashboardViewModel
        {
            CalendarWeeks = GetCalendarFor(new VrRetreatUser() { VrChatLastLogin = new DateTime(2022, 1, 12, 14, 23, 59), FailedChallenge = true }),
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

    private DateTime Now => DateTime.UtcNow;

    private IEnumerable<CalendarWeek> GetCalendarFor(VrRetreatUser user)
    {
        const int DaysInChallengeCalendar = 42;
        var calendarStartDate = new DateTime(2021, 12, 26);
        var challengeStartDate = new DateTime(2022, 1, 1);
        var challengeEndDate = new DateTime(2022, 1, 31, 23, 59, 59);

        var result = new List<CalendarWeek>();
        var activeWeek = new List<CalendarDayType>();
        for (var dayMod = 0; dayMod < DaysInChallengeCalendar; dayMod++)
        {
            var currentDay = calendarStartDate.AddDays(dayMod);

            if (currentDay < challengeStartDate || currentDay > challengeEndDate)
            {
                activeWeek.Add(CalendarDayType.OutsideOfBounds);
            }
            else
            {
                activeWeek.Add(GetCalendarDayTypeFor(user, currentDay));
            }

            if (currentDay.DayOfWeek == DayOfWeek.Saturday)
            {
                result.Add(new CalendarWeek { Days = activeWeek.ToArray() });
                activeWeek.Clear();
            }
        }

        return result;
    }

    private CalendarDayType GetCalendarDayTypeFor(VrRetreatUser user, DateTime currentDay)
    {
        if (user.VrChatLastLogin is null)
            return CalendarDayType.TBD;
        
        if (DateOnly.FromDateTime(currentDay) == DateOnly.FromDateTime(Now))
        {
            return user.FailedChallenge ? CalendarDayType.Failure : CalendarDayType.TBD;
        }

        if (DateOnly.FromDateTime(currentDay) > DateOnly.FromDateTime(Now))
            return CalendarDayType.TBD;

        if (DateOnly.FromDateTime(currentDay) < DateOnly.FromDateTime(user.VrChatLastLogin.Value))
            return CalendarDayType.TBD;

        return user.FailedChallenge ? CalendarDayType.Failure : CalendarDayType.Success;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
