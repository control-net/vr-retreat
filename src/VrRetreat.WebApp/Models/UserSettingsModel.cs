using System.ComponentModel.DataAnnotations;

namespace VrRetreat.WebApp.Models
{
    public class UserSettingsModel
    {

        [Required(ErrorMessage = "Old Password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


    }
}
