using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VrRetreat.Core.Boundaries.BioCodeVerification;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.VrChatAccountClaim;
using VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;
using VrRetreat.Infrastructure.Entities;
using VrRetreat.WebApp.Models;
using VrRetreat.WebApp.Presenters;

namespace VrRetreat.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<VrRetreatUser> _signInManager;
        private readonly IUserRepository _userRepository;

        private readonly IVrChatAccountClaimUseCase _accountClaimUseCase;
        private readonly VrChatAccountClaimPresenter _accountClaimPresenter;

        private readonly IVrChatVerifyFriendStatusUseCase _friendStatusUseCase;
        private readonly VrChatVerifyFriendStatusPresenter _friendStatusPresenter;

        private readonly IBioCodeVerificationUseCase _bioVerificationUseCase;
        private readonly BioCodeVerificationPresenter _bioVerificationPresenter;

        public HomeController(SignInManager<VrRetreatUser> signInManager, IUserRepository userRepository,
            IVrChatAccountClaimUseCase accountClaimUseCase, VrChatAccountClaimPresenter accountClaimPresenter,
            IVrChatVerifyFriendStatusUseCase friendStatusUseCase, VrChatVerifyFriendStatusPresenter friendStatusPresenter,
            IBioCodeVerificationUseCase bioVerificationUseCase, BioCodeVerificationPresenter bioVerificationPresenter)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;

            _accountClaimUseCase = accountClaimUseCase;
            _accountClaimPresenter = accountClaimPresenter;

            _friendStatusUseCase = friendStatusUseCase;
            _friendStatusPresenter = friendStatusPresenter;

            _bioVerificationUseCase = bioVerificationUseCase;
            _bioVerificationPresenter = bioVerificationPresenter;
        }

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User))
                return View();

            if (await _userRepository.HasLinkedAccountByUsername(User.Identity?.Name ?? string.Empty))
            {
                var user = await _userRepository.GetUserByUsernameAsync(User.Identity.Name);
                // NOTE(Peter): Once Dashboard is finished, we'll construct the model here.
                return View("Dashboard", new DashboardViewModel
                {
                    CurrentUser = new()
                    {
                        AvatarUrl = user.VrChatAvatarUrl,
                        Failed = user.FailedChallenge,
                        Username = user.VrChatName
                    }
                });
            }

            return View("AccountLinking");
        }

        [Authorize]
        public IActionResult ClaimVrChatName()
        {
            return View(new VrChatNameClaimModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ClaimVrChatName(VrChatNameClaimModel model)
        {
            _accountClaimPresenter.ModelState = ModelState;

            await _accountClaimUseCase.ExecuteAsync(new(User.Identity.Name, model.VrChatName));

            if(_accountClaimPresenter.Result is not null)
                return _accountClaimPresenter.Result;

            if(_accountClaimPresenter.Success)
                return RedirectToAction(nameof(VrChatFriendRequestValidation));

            return View(model);
        }

        [Authorize]
        public IActionResult VrChatFriendRequestValidation()
        {
            return View(new VrChatFriendRequestModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VrChatFriendRequestValidation(VrChatFriendRequestModel model)
        {
            _friendStatusPresenter.ModelState = ModelState;

            await _friendStatusUseCase.ExecuteAsync(new(User.Identity.Name));

            if (_friendStatusPresenter.Result is not null)
                return _friendStatusPresenter.Result;

            if (_friendStatusPresenter.Success)
                return RedirectToAction(nameof(VrChatBioCodeValidation));

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> VrChatBioCodeValidation()
        {
            // TODO(Peter): improve null checking
            var user = await _userRepository.GetUserByUsernameAsync(User.Identity.Name);
            return View(new VrChatBioCodeValidationModel()
            {
                BioCode = user?.BioCode
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VrChatBioCodeValidation(VrChatBioCodeValidationModel model)
        {
            _bioVerificationPresenter.ModelState = ModelState;

            await _bioVerificationUseCase.ExecuteAsync(new(User.Identity.Name));

            if (_bioVerificationPresenter.Result is not null)
                return _bioVerificationPresenter.Result;

            if (_bioVerificationPresenter.Success)
                return RedirectToAction(nameof(Index));

            return View(model);
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
}
