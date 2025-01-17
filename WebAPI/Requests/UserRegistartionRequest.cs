namespace WebAPI.Requests;

public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string GoogleToken { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public int DrivingLicenseYears { get; set; }
}
