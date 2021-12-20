namespace VrRetreat.WebApp.Models;

public class DashboardViewModel
{
    public UserDashboardModel? CurrentUser { get; set; }
    public IEnumerable<UserDashboardModel> FollowedPeople { get; set; } = Array.Empty<UserDashboardModel>();
}

public class UserDashboardModel
{
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public DateTime LastVrChatLogin { get; set; }
    public TimeSpan OfflineDuration => DateTime.UtcNow - LastVrChatLogin;
    public bool Failed { get; set; }
}
