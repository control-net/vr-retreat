using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VrRetreat.Core.Boundaries.BioCodeVerification;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.VrChatAccountClaim;
using VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;
using VrRetreat.Infrastructure.Entities;
using VrRetreat.WebApp.Models;
using VrRetreat.WebApp.Presenters;

namespace VrRetreat.WebApp.Controllers;

[Authorize]
public class AccountLinkController : Controller
{
    private readonly UserManager<VrRetreatUser> _userManager;
    private readonly IUserRepository _userRepository;

    private readonly IVrChatAccountClaimUseCase _accountClaimUseCase;
    private readonly VrChatAccountClaimPresenter _accountClaimPresenter;

    private readonly IVrChatVerifyFriendStatusUseCase _friendStatusUseCase;
    private readonly VrChatVerifyFriendStatusPresenter _friendStatusPresenter;

    private readonly IBioCodeVerificationUseCase _bioVerificationUseCase;
    private readonly BioCodeVerificationPresenter _bioVerificationPresenter;

    public AccountLinkController(UserManager<VrRetreatUser> userManager, IUserRepository userRepository,
            IVrChatAccountClaimUseCase accountClaimUseCase, VrChatAccountClaimPresenter accountClaimPresenter,
            IVrChatVerifyFriendStatusUseCase friendStatusUseCase, VrChatVerifyFriendStatusPresenter friendStatusPresenter,
            IBioCodeVerificationUseCase bioVerificationUseCase, BioCodeVerificationPresenter bioVerificationPresenter)
    {
        _userManager = userManager;
        _userRepository = userRepository;

        _accountClaimUseCase = accountClaimUseCase;
        _accountClaimPresenter = accountClaimPresenter;

        _friendStatusUseCase = friendStatusUseCase;
        _friendStatusPresenter = friendStatusPresenter;

        _bioVerificationUseCase = bioVerificationUseCase;
        _bioVerificationPresenter = bioVerificationPresenter;
    }

    public IActionResult ClaimVrChatName() => View(new VrChatNameClaimModel());

    [HttpPost]
    public async Task<IActionResult> ClaimVrChatName(VrChatNameClaimModel model)
    {
        _accountClaimPresenter.ModelState = ModelState;

        await _accountClaimUseCase.ExecuteAsync(new(User.Identity?.Name!, model.VrChatName));

        if (_accountClaimPresenter.Result is not null)
            return _accountClaimPresenter.Result;

        if (_accountClaimPresenter.Success)
            return RedirectToAction(nameof(VrChatFriendRequestValidation));

        model.IsValid = false;
        return View(model);
    }

    public IActionResult VrChatFriendRequestValidation() => View(new VrChatFriendRequestModel());

    [HttpPost]
    public async Task<IActionResult> VrChatFriendRequestValidation(VrChatFriendRequestModel model)
    {
        _friendStatusPresenter.ModelState = ModelState;

        await _friendStatusUseCase.ExecuteAsync(new(User.Identity?.Name!));

        if (_friendStatusPresenter.Result is not null)
            return _friendStatusPresenter.Result;

        if (_friendStatusPresenter.Success)
            return RedirectToAction(nameof(VrChatBioCodeValidation));

        model.IsValid = false;
        return View(model);
    }

    public async Task<IActionResult> VrChatBioCodeValidation()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return RedirectToAction("Index", "Home");

        // NOTE(Peter): It would be better to also display a message after this redirect
        if (user.BioCode is null)
            return RedirectToAction(nameof(ClaimVrChatName));

        return View(new VrChatBioCodeValidationModel { BioCode = user.BioCode });
    }

    [HttpPost]
    public async Task<IActionResult> VrChatBioCodeValidation(VrChatBioCodeValidationModel model)
    {
        _bioVerificationPresenter.ModelState = ModelState;

        await _bioVerificationUseCase.ExecuteAsync(new(User.Identity?.Name!));

        if (_bioVerificationPresenter.Result is not null)
            return _bioVerificationPresenter.Result;

        if (_bioVerificationPresenter.Success)
            return RedirectToAction("Index", "Home");

        model.IsValid = false;
        return View(model);
    }
}
