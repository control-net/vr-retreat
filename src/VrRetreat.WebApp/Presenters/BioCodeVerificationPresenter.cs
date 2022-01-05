using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VrRetreat.Core.Boundaries.BioCodeVerification;

namespace VrRetreat.WebApp.Presenters
{
    public class BioCodeVerificationPresenter : IBioCodeVerificationOutputPort
    {
        public bool Success { get; set; }
        public ModelStateDictionary? ModelState { get; internal set; }

        public IActionResult? Result { get; private set; }

        public void BioCodeNotFound()
        {
            ModelState?.AddModelError("BioCode", "We couldn't find the code in your bio");
        }

        public void BioCodeVerified()
        {
            Success = true;
        }

        public void LoggedInUserNotFound()
        {
            Result = new RedirectToActionResult("Index", "Home", null);
        }

        public void NoClaimedVrChatAccount()
        {
            Result = new RedirectToActionResult("ClaimVrChatName", "Home", null);
        }

        public void UserHasCooldown()
        {
            ModelState?.AddModelError("Cooldown", "You have to wait a while before trying again.");
        }
    }
}
