using System.ComponentModel.DataAnnotations;

namespace LunaTestTask.Models;

public class TokenModel
{
    [Key]
    public string Token { get; set; }
    public Guid UserId { get; set; }
}