using System.ComponentModel.DataAnnotations;

namespace VrRetreat.WebApp.Models;

public class VrChatNameClaimModel
{
    [Required]
    [MinLength(2, ErrorMessage = "VRChat names must be more than 2 characters long.")]
    public string VrChatName { get; set; } = string.Empty;

    public bool IsValid { get; set; } = true;
}
