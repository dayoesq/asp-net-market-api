using Microsoft.AspNetCore.Identity;

namespace Market.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    //public string Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public int? LoginCount { get; set; }
    public ActivityStatus ActivityStatus { get; set; } = ActivityStatus.InActive;
    public bool? IsVerified { get; set; }
    public string? VerificationCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}

public enum ActivityStatus
{
    Active = 1,
    InActive = 0
}