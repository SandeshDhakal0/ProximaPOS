namespace TheHighInnovation.POS.Web.Model.Response.User;

public class UserLockResponseDto
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public string Name { get; set; }
    
    public DateTime Date { get; set; }

    public TimeSpan LockTime { get; set; }

    public TimeSpan? UnlockTime { get; set; }
}