namespace VrRetreat.WebApp.Models.Response
{
    public class DownloadData
    {
        public string VrChatId { get; set; } = string.Empty;
        public string VrChatName { get; set; } = string.Empty;
        public string VrChatAvatarUrl { get; set; } = string.Empty;
        public DateTime? VrChatLastLogin { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
