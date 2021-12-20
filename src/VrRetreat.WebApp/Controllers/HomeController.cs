using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VrRetreat.WebApp.Models;

namespace VrRetreat.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
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
                FollowedPeople = new []
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}