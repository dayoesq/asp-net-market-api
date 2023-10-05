using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Email), IsUnique = true)]
[PrimaryKey(nameof(Email))]
public class PasswordResetToken
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

}