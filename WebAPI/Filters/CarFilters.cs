namespace WebAPI.filters
{
    public class CarFilter
    {
        public int? CarId { get; set; }
        public string? Location { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public string? FuelType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}