namespace Market.Models.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Address { get; set; }
    public string? Telephone { get; set; }
    public string? IPAddress { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}