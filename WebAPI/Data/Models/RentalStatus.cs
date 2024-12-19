namespace WebAPI.Data.Models;

public class RentalStatus
{
    public int RentalStatusId { get; set; }
    public string Description { get; set; } = null!;

    public static int GetCompletedId()
    {
        return 3;
    }
    
    public static int GetPendingId()
    {
        return 2;
    }
    
    public static int GetConfirmedId()
    {
        return 1;
    }
}