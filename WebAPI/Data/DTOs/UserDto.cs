using System.Text.Json;

namespace WebAPI.Data.DTOs;

public class UserDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}