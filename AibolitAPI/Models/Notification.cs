namespace AibolitAPI.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public string Message { get; set; }
    public DateTime NotificationDate { get; set; }
    public bool IsRead { get; set; }
    public bool IsActive { get; set; } 
}