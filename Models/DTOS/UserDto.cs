namespace Market.Models.DTOS;

public class UserDto : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public int LoginCount { get; set; }
    public ActivityStatus ActivityStatus { get; set; } = ActivityStatus.InActive;
    public bool IsVerified { get; set; }
    public string VerificationCode { get; set; }
    
}