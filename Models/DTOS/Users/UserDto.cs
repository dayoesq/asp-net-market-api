namespace Market.Models.DTOS.Users;

public class UserDto
{
    
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    
}