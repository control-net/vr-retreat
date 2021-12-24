using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;

namespace VrRetreat.WebApp.Presenters
{
    public class VrChatVerifyFriendStatusPresenter : IVrChatVerifyFriendStatusOutputPort
    {
        public bool Success { get; set; }
        public ModelStateDictionary? ModelState { get; internal set; }

        public IActionResult? Result { get; private set; }

        public void FriendshipVerified()
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

        public void UserNotFriended()
        {
            ModelState?.AddModelError("FriendStatus", "It doesn’t look like you’ve accepted our friend request");
        }
    }
}
