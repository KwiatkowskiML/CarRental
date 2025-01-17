namespace WebAPI.filters;

public class UserFilter
{
    public int? UserId { get; set; }

    public string? Email { get; set; } = null!;

    public string? FirstName { get; set; } = null!;

    public string? LastName { get; set; } = null!;

    public int? Age { get; set; }

    public string? Location { get; set; }
}