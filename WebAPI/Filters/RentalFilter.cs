namespace WebAPI.filters
{
    public class RentalFilter
    {
        public int? RentalId { get; set; }
        public int? CustomerId { get; set; }
        public int? RentalStatus { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
    }
}