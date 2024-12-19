namespace WebAPI.Requests;

public class AcceptReturnRequest
{
    public int RentalId { get; set; }
    public int EmployeeId { get; set; }
    public string ConditionDescription { get; set; } = "";
    public string PhotoUrl { get; set; } = "";
    public DateOnly ReturnDate { get; set; }
}