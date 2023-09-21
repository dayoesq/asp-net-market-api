namespace Market.Models.DTOS;

public class UserDto : BaseEntity
{
    
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public bool IsVerified { get; set; }
    public string? VerificationCode { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiration { get; set; }
    public DateTime? LastLogin { get; set; }
    
}