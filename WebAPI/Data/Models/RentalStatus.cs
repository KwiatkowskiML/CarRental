namespace WebAPI.Data.Models;

public class RentalStatus
{
    public int RentalStatusId { get; set; }
    public string Description { get; set; } = null!;
    
    // Rental status
    private static readonly int CompletedId = 3;
    private static readonly int PendingId = 2;
    private static readonly int ConfirmedId = 1;
    
    public static int GetCompletedId()
    {
        return CompletedId;
    }
    
    public static int GetPendingId()
    {
        return PendingId;
    }
    
    public static int GetConfirmedId()
    {
        return ConfirmedId;
    }
}