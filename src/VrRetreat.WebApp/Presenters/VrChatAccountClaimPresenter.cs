using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VrRetreat.Core.Boundaries.VrChatAccountClaim;

namespace VrRetreat.WebApp.Presenters
{
    public class VrChatAccountClaimPresenter : IVrChatAccountClaimOutputPort
    {
        public bool Success { get; set; }
        public ModelStateDictionary? ModelState { get; internal set; }

        public IActionResult? Result { get; private set; }

        public void SuccessfulClaim()
        {
            Success = true;
        }

        public void UnknownLoggedInUser()
        {
            Result = new RedirectToActionResult("Index", "Home", null);
        }

        public void UnknownVrChatUsername(string username)
        {
            ModelState?.AddModelError("Username", "This VRChat display name doesn't exist.");
        }

        public void UserHasCooldown()
        {
            ModelState?.AddModelError("Cooldown", "You have to wait a while before trying again.");
        }
    }
}
