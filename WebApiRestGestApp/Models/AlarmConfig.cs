namespace WebApiRestGestApp.Models
{
    public class AlarmConfig
    {
        public int IdAlar { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Sound { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
    }
}