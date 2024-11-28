namespace LunaTestTask.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string EMail { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}