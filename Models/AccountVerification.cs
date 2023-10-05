using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Email), IsUnique = true)]
[PrimaryKey(nameof(Email))]
public class AccountVerification
{
    public string Email { get; set; } = null!;
    public int Code { get; set; }
}