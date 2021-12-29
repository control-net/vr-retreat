using Microsoft.AspNetCore.Mvc;
using VrRetreat.WebApp.Models.Components;

namespace VrRetreat.WebApp.Components;

public class PopupButtonViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(
        string modalTitle,
        string modalDescription,
        string popupButtonText,
        string popupId,
        string confirmButtonAction,
        string popupButtonClass = "btn btn-danger",
        string confirmButtonClass = "btn btn-danger",
        string confirmButtonText = "Confirm")
    {
        var viewModel = new PopupButtonViewModel
        {
            Title = modalTitle,
            Description = modalDescription,

            PopupId = popupId,
            PopupButtonClass = popupButtonClass,
            PopupButtonText = popupButtonText,

            ConfirmButtonAction = confirmButtonAction,
            ConfirmButtonClass = confirmButtonClass,
            ConfirmButtonText = confirmButtonText
        };
        return View(viewModel);
    }


}

