using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VrRetreat.Core.Boundaries.StartChallenge;

namespace VrRetreat.WebApp.Presenters;

public class StartChallengePresenter : IStartChallengeOutputPort
{
    public bool Success { get; set; }
    public ModelStateDictionary? ModelState { get; internal set; }
    public IActionResult? Result { get; private set; }

    public void LoggedInUserNotFound()
    {
        Result = new RedirectToActionResult("Index", "Home", null);
    }

    public void NoClaimedVrChatAccount()
    {
        Result = new RedirectToActionResult("ClaimVrChatName", "Home", null);
    }

    public void RedirectToIndex()
    {
        Result = new RedirectToActionResult("Index", "Home", null);
    }
    public void ChallengeFailed()
    {
        ModelState?.AddModelError("ChallengeFailed", "You have already failed the challenge. *Nyah*");
    }

    public void SuccessfulStart()
    {
        Success = true;
    }

    public void UnknownVrChatUsername(string username)
    {
        ModelState?.AddModelError("Username", "This VRChat display name doesn't exist.");
    }
}

