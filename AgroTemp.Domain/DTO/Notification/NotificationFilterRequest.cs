namespace AgroTemp.Domain.DTO.Notification
{
    public class NotificationFilterRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsRead { get; set; }
    }
}
