namespace WebAPI.Data.DTOs;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public int DrivingLicenseYears { get; set; }
    public required UserDto UserDto { get; set; }
}