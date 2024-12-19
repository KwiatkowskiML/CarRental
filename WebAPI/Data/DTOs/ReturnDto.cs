namespace WebAPI.Data.DTOs;

public class ReturnDto
{
    public int ReturnId { get; set; }

    public int? RentalId { get; set; }

    public DateOnly ReturnDate { get; set; }

    public string? ConditionDescription { get; set; }

    public string? PhotoUrl { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
}