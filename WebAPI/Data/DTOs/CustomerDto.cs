using System.Text.Json;

namespace WebAPI.Data.DTOs;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public int DrivingLicenseYears { get; set; }
    public required UserDto UserDto { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}