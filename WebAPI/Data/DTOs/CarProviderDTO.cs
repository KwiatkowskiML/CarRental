namespace WebAPI.Data.DTOs;

public class CarProviderDto
{
    public int CarProviderId { get; set; }
    public string Name { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string? ContactPhone { get; set; }
}