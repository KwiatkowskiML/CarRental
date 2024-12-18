namespace WebAPI.Data.Models;

public class Insurance
{
    public int InsuranceId { get; set; }

    public decimal Price { get; set; }

    public string name { get; set; } = null!;
}
